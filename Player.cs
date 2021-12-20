using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace Game
{
    class Player
    {
        //Properties
        #region Properties

        //Health
        public int Health { get; set; }
        public bool IsAlive { get; set; }

        //Location
        public double Height { get; set; }
        public Point3d Location { get; set; }
        public Point3d CameraLocation { get; set; }

        //Movement
        public enum SpeedTypes
        {
            CrouchingSpeed,
            ActiveSpeed,
            DefaultSpeed
        }
        public double CrouchingSpeed { get; set; }
        public double ActiveSpeed { get; set; }
        public double DefaultSpeed { get; set; }
        public double RunningSpeed { get; set; }
        public Vector3d MovementForward { get; set; }
        public Vector3d MovementSide { get; set; }

        //Forces
        public bool IsAirbourne { get; set; }
        public Vector3d JumpAcceleration { get; set; }
        public Vector3d Forces { get; set; }

        //Orientation
        public Vector3d LookDirection { get; set; }
        public double LookSpeed { get; set; }
        public double LookStopAngleZ { get; set; }
        #endregion Properties

        //Constructor
        public Player(double height)
        {
            //Health
            Health = 100;
            IsAlive = true;

            //Location
            Height = height;
            Location = new Point3d(0.0, 0.0, 0.0);
            CameraLocation = new Point3d(0.0, 0.0, Height);

            //Movement
            CrouchingSpeed = 5;
            DefaultSpeed = 10;
            RunningSpeed = 20;
            MovementForward = new Vector3d(0.0, 0.0, 0.0);
            MovementSide = new Vector3d(0.0, 0.0, 0.0);

            //Forces
            IsAirbourne = false;
            JumpAcceleration = new Vector3d(0.0, 0.0, 3.5);
            Forces = new Vector3d(0.0, 0.0, 0.0);
           
            //Orientation
            LookDirection = new Vector3d(0.0, 1.0, 0.0);
            LookSpeed = Math.PI;
            LookStopAngleZ = Math.PI / 180.0;
        }
        
        public void TakeDamage(int damage)
        {
            Health -= damage;
            if (Health <= 0)
            {
                IsAlive = false;
            }
        }

        public void Update(GameController gameController, double deltaTime, Vector3d gravity)
        {
            UpdateMovement(gameController, deltaTime, gravity);
            PlaterLookDirection(gameController, deltaTime);
        }

        //Movement
        #region Movement
        //Update movement
        private void UpdateMovement(GameController gameController, double deltaTime, Vector3d gravity)
        {
            GetMovementDirection(gameController, deltaTime);

            SetLocationXY();
            HandleJump(gameController, deltaTime, gravity);

            //Move Player Camera
            CameraLocation = new Point3d(Location.X, Location.Y, Height);
            Rhino.RhinoDoc.ActiveDoc.Views.ActiveView.ActiveViewport.SetCameraLocation(CameraLocation, true);
        }

        //Movement Direction Change
        private void GetMovementDirection(GameController gameController, double deltaTime)
        {
            if (!IsAirbourne)
            { 
                //Forward vector
                MovementForward = new Vector3d(LookDirection.X, LookDirection.Y, 0.0);

                //Side vector unitized
                MovementSide = new Vector3d(MovementForward);
                MovementSide.Rotate(-Math.PI / 2.0, Vector3d.ZAxis);

                //Scaled Forward Vector
                MovementForward.Unitize();
                MovementForward *= gameController.LeftStickY * deltaTime * ActiveSpeed;

                //Scaled side vector
                MovementSide.Unitize();
                MovementSide *= gameController.LeftStickX * deltaTime * ActiveSpeed;
            }
        }

        //Move XY
        private void SetLocationXY()
        {
            //Move forward
            if (MovementForward.Length > 0)
            {
                Location.Transform(Transform.Translation(MovementForward));
            }

            //Move sideways
            if (MovementSide.Length > 0)
            {
                Location.Transform(Transform.Translation(MovementSide));
            }
        }

        //HandleJump
        private void HandleJump(GameController gameController, double deltaTime, Vector3d gravity)
        {
            //Activate Jump
            if (gameController.ButtonA.JustPressed && !IsAirbourne)
            {
                IsAirbourne = true;
                Forces = JumpAcceleration;
            }

            //Apply Gravity
            if (IsAirbourne)
            {
                Location += Forces * deltaTime;
                Forces += gravity * deltaTime;

                //End Jump
                if (Location.Z < 0)
                {
                    IsAirbourne = false;
                    Location = new Point3d(Location.X, Location.Y, 0);
                }
            }
        }
        #endregion Movement

        //Player look direction
        private void PlaterLookDirection(GameController gameController, double deltaTime)
        {
            LookDirection.Rotate(-gameController.RightStickX * LookSpeed * deltaTime, Vector3d.ZAxis);

            double cameraAngleZ = Vector3d.VectorAngle(new Vector3d(0.0, 0.0, -1.0), LookDirection);
            double lookRotationAngle = gameController.RightStickY * LookSpeed * deltaTime;

            if ((cameraAngleZ + lookRotationAngle + LookStopAngleZ < Math.PI && lookRotationAngle > 0) || (cameraAngleZ + lookRotationAngle - LookStopAngleZ > 0 && lookRotationAngle < 0))
            {
                Vector3d lookSideDirection = new Vector3d(LookDirection.X, LookDirection.Y, 0.0);
                lookSideDirection.Rotate(-Math.PI / 2.0, Vector3d.ZAxis);
                LookDirection.Rotate(lookRotationAngle, lookSideDirection);
            }
            Rhino.RhinoDoc.ActiveDoc.Views.ActiveView.ActiveViewport.SetCameraDirection(LookDirection, true);
        }


    }
}
