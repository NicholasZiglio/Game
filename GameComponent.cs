using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using SharpDX.XInput;

namespace Game
{
    public class GameComponent : GH_Component
    {
        //ADD PLAYERS
        #region Persistent Variables

        GameController gameController = new GameController(UserIndex.One, 10000.0, 10000.0);
        

        //Grasshopper
        GH_Document GrasshopperDocument;
        GH_Component GrasshopperComponent;
        int scheduleSolutionMilliseconds = 10;

        //Rhino


        //Game
        bool isGameInitialized = false;
        DateTime startTime = DateTime.Now;
        DateTime endTime = DateTime.Now;
        double deltaTime;
        Vector3d gravity = new Vector3d(0.0, 0.0, -9.81);

        //Player
        Vector3d cameraDirection = new Vector3d(0.0, 1.0, 0.0);
        double playerHeight = 2.0;
        Point3d playerPoint;
        double playerMovementSpeed = 20.0;
        double playerLookSpeed = Math.PI;
        double cameraStopAngleZ = Math.PI / 180.0;
        bool isPlayerJumping = false;
        Vector3d playerJumpAcceleration = new Vector3d(0.0, 0.0, 3.5);
        Vector3d playerForces = new Vector3d(0.0, 0.0, 0.0);
        Vector3d playerForwardMovementVector = Vector3d.Unset;
        Vector3d playerSideMovementVector = Vector3d.Unset;


        #endregion Persistent Variables

        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public GameComponent()
          : base("Game", "Nickname",
            "Description",
            "Category", "Subcategory")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            InitializeComponent();
        }

        //Initialize Grasshopper component & Rhino document
        private void InitializeComponent()
        {
            //Get Component
            if (GrasshopperComponent == null)
            {
                GrasshopperComponent = this;
            }
            //If Component
            if (GrasshopperComponent != null)
            {
                //Get Document
                if (GrasshopperDocument == null)
                {
                    GrasshopperDocument = this.OnPingDocument();
                }
                //If Document
                if (GrasshopperDocument != null)
                {
                    if (!isGameInitialized)
                    {
                        Start();
                        isGameInitialized = true;
                    }
                    //Schedule Solution
                    GrasshopperDocument.ScheduleSolution(scheduleSolutionMilliseconds, Update);
                }
            }
        }

        //Start
        public void Start()
        {
            playerPoint = new Point3d(0.0, 0.0, playerHeight);
        }

        //Update
        public void Update(GH_Document doc)
        {
            //Expire solution
            GrasshopperComponent.ExpireSolution(false);
            
            //Updates
            gameController.UpdateState();
            GetDeltaTime();

            //Player Control
            PlaterLookDirection();
            PlayerMovement();
        }

        //Get delta time
        private void GetDeltaTime()
        {
            endTime = DateTime.Now;
            deltaTime = (endTime - startTime).TotalMilliseconds / 1000.0;
            startTime = DateTime.Now;
        }


        

        //Player look direction
        private void PlaterLookDirection()
        {
            cameraDirection.Rotate(-gameController.RightStickX * playerLookSpeed * deltaTime, Vector3d.ZAxis);

            double cameraAngleZ = Vector3d.VectorAngle(new Vector3d(0.0, 0.0, -1.0), cameraDirection);
            double lookRotationAngle = gameController.RightStickY * playerLookSpeed * deltaTime;

            if ((cameraAngleZ + lookRotationAngle + cameraStopAngleZ < Math.PI && lookRotationAngle > 0) || (cameraAngleZ + lookRotationAngle - cameraStopAngleZ > 0 && lookRotationAngle < 0))
            {
                Vector3d cameraSideDirection = new Vector3d(cameraDirection.X, cameraDirection.Y, 0.0);
                cameraSideDirection.Rotate(-Math.PI / 2.0, Vector3d.ZAxis);
                cameraDirection.Rotate(lookRotationAngle, cameraSideDirection);
            }
            Rhino.RhinoDoc.ActiveDoc.Views.ActiveView.ActiveViewport.SetCameraDirection(cameraDirection, true);
        }

        //Player movement
        private void PlayerMovement()
        {
            //Allow direction change if on ground
            if (!isPlayerJumping)
            {
                //Forward vector
                playerForwardMovementVector = new Vector3d(cameraDirection.X, cameraDirection.Y, 0.0);

                //Side vector unitized
                playerSideMovementVector = new Vector3d(playerForwardMovementVector);
                playerSideMovementVector.Rotate(-Math.PI / 2.0, Vector3d.ZAxis);

                //Scaled Forward Vector
                playerForwardMovementVector.Unitize();
                playerForwardMovementVector *= gameController.LeftStickY * deltaTime * playerMovementSpeed;

                //Scaled side vector
                playerSideMovementVector.Unitize();
                playerSideMovementVector *= gameController.LeftStickX * deltaTime * playerMovementSpeed;
            }

            //Move forward
            if (playerForwardMovementVector != Vector3d.Unset)
            {
                playerPoint.Transform(Transform.Translation(playerForwardMovementVector));
            }
            
            //Move sideways
            if (playerSideMovementVector != Vector3d.Unset)
            {
                playerPoint.Transform(Transform.Translation(playerSideMovementVector));
            }
           
            //Activate Jump
            if (gameController.ButtonA.JustPressed && !isPlayerJumping)
            {
                isPlayerJumping = true;
                playerForces = playerJumpAcceleration;
            }

            //Apply Gravity
            if (isPlayerJumping)
            {
                playerPoint += playerForces * deltaTime;
                playerForces += gravity * deltaTime;

                //End Jump
                if (playerPoint.Z < playerHeight)
                {
                    isPlayerJumping = false;
                    playerPoint.Z = playerHeight;
                }
            }

            //Move Player Camera
            Rhino.RhinoDoc.ActiveDoc.Views.ActiveView.ActiveViewport.SetCameraLocation(playerPoint, true);
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// You can add image files to your project resources and access them like this:
        /// return Resources.IconForThisComponent;
        /// </summary>
        protected override System.Drawing.Bitmap Icon => null;

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("99DB7909-BDD6-4C15-8095-D5494C278A4B");
    }
}