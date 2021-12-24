using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using SharpDX.XInput;
using System.IO;

namespace Game
{
    public class GameComponent : GH_Component
    {
        #region Persistent Variables

        readonly GameController gameController = new GameController(UserIndex.One, 10000.0, 10000.0);
        Player player = new Player(2.0);

        //Grasshopper
        GH_Document GrasshopperDocument;
        GH_Component GrasshopperComponent;
        readonly int scheduleSolutionMilliseconds = 2;

        //Rhino


        //Game
        bool isGameInitialized = false;
        DateTime startTime = DateTime.Now;
        DateTime endTime = DateTime.Now;
        double deltaTime;
        Vector3d gravity = new Vector3d(0.0, 0.0, -9.81);

        readonly Mesh groundMesh = (Mesh)Rhino.Runtime.CommonObject.FromJSON(Properties.Resources.GroundMeshJson);

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
            pManager.AddMeshParameter("Snowballs", "Snowballs", "Snowballs", GH_ParamAccess.list);
            pManager.AddMeshParameter("SnowmenBodies", "SnowmenBodies", "SnowmenBodies", GH_ParamAccess.list);
            pManager.AddMeshParameter("SnowmenNoses", "SnowmenNoses", "SnowmenNoses", GH_ParamAccess.list);
            pManager.AddMeshParameter("SnowmenArms", "SnowmenArms", "SnowmenArms", GH_ParamAccess.list);
            pManager.AddMeshParameter("SnowmenHatButtons", "SnowmenHatButtons", "SnowmenHatButtons", GH_ParamAccess.list);
            pManager.AddCurveParameter("Snowflakes", "Snowflakes", "Snowflakes", GH_ParamAccess.list);
            pManager.AddMeshParameter("Ground", "Ground", "Ground", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            InitializeComponent();
            

            DA.SetDataList(0, Snowball.SnowballMeshes);
            DA.SetDataList(1, Snowman.SnowmenBodyMeshes);
            DA.SetDataList(2, Snowman.SnowmenNoseMeshes);
            DA.SetDataList(3, Snowman.SnowmenArmsMeshes);
            DA.SetDataList(4, Snowman.SnowmenBlackMeshes);
            DA.SetDataList(5, Snowflake.SnowflakePolylines);
            DA.SetData(6, groundMesh);
            
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
            player = new Player(2.0);
            Snowball.Snowballs.Clear();
            Snowman.Snowmen.Clear();
            Snowman.Snowmen.Add(new Snowman(player.Location));
            Snowman.Snowmen.Add(new Snowman(player.Location));
            Snowman.Snowmen.Add(new Snowman(player.Location));
            Snowman.StartHealth = 300;
            Snowman.StartSpeed = 5;
            Snowflake.GenerateSnowflakes(75, player.Location);
        }

        //Update
        public void Update(GH_Document doc)
        {
            //Expire solution
            GrasshopperComponent.ExpireSolution(false);

            //Updates
            GetDeltaTime();
            Snowflake.UpdateSnowflakes(player.Location, deltaTime, gravity);
            gameController.UpdateState();
            player.Update(gameController, deltaTime, gravity);
            if (!player.IsAlive)
            {
                Start();
            }
            else
            {
                Snowball.Update(deltaTime, gravity);
                Snowman.Update(player, deltaTime);
            }
            

            
        }

        //Get delta time
        private void GetDeltaTime()
        {
            endTime = DateTime.Now;
            deltaTime = (endTime - startTime).TotalMilliseconds / 1000.0;
            startTime = DateTime.Now;
        }


        //Icon
        protected override System.Drawing.Bitmap Icon => Properties.Resources.GameIcon;

        //GUID
        public override Guid ComponentGuid => new Guid("99DB7909-BDD6-4C15-8095-D5494C278A4B");


        //Render Meshes
        public override void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            //Materials
            Rhino.Display.DisplayMaterial whiteMaterial = new Rhino.Display.DisplayMaterial(System.Drawing.Color.White);
            Rhino.Display.DisplayMaterial orangeMaterial = new Rhino.Display.DisplayMaterial(System.Drawing.Color.Orange);
            Rhino.Display.DisplayMaterial brownMaterial = new Rhino.Display.DisplayMaterial(System.Drawing.Color.Brown);
            Rhino.Display.DisplayMaterial blackMaterial = new Rhino.Display.DisplayMaterial(System.Drawing.Color.Black);


            //Args
            base.DrawViewportMeshes(args);

            //Render Meshes
            RenderMeshList(args, Snowball.SnowballMeshes, whiteMaterial);
            RenderMeshList(args, Snowman.SnowmenBodyMeshes, whiteMaterial);
            RenderMeshList(args, Snowman.SnowmenNoseMeshes, orangeMaterial);
            RenderMeshList(args, Snowman.SnowmenArmsMeshes, brownMaterial);
            RenderMeshList(args, Snowman.SnowmenBlackMeshes, blackMaterial);
            args.Display.DrawMeshShaded(groundMesh, whiteMaterial);
        }

        //Render List of Meshes
        public void RenderMeshList(IGH_PreviewArgs args, List<Mesh> meshList, Rhino.Display.DisplayMaterial material)
        {
            foreach (Mesh mesh in meshList)
            {
                args.Display.DrawMeshShaded(mesh, material);
            }
        }

        //Render Curves
        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            base.DrawViewportWires(args);
            RenderPolylineList(args, Snowflake.SnowflakePolylines, System.Drawing.Color.White);
            TextEntity score = new TextEntity();
            score.PlainText = "Score: " + player.Score.ToString();
            args.Display.DrawText(score, System.Drawing.Color.Red);
        }

        //Render List of Curves
        public void RenderPolylineList(IGH_PreviewArgs args, List<Polyline> polylineList, System.Drawing.Color color)
        {
            foreach (Polyline polyline in polylineList)
            {
                args.Display.DrawPolyline(polyline, color);
                
            }
        }
    }
}