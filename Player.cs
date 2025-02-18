﻿using System;
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

        public static int Score;

        //Health
        public int Health { get; set; }
        public bool IsAlive { get; set; }

        //Location
        public double Height;
        private double FullHeight { get; set; }
        private double CrouchingHeight { get; set; }


        public Point3d Location { get; set; }
        private Point3d _Location;

        //Movement
        private bool IsAirbourne { get; set; }
        private bool IsCrouching { get; set; }
        private enum SpeedTypes
        {
            CrouchingSpeed,
            ActiveSpeed,
            DefaultSpeed
        }
        private double ActiveSpeed;
        private double CrouchingSpeed { get; set; }
        private double DefaultSpeed { get; set; }
        private double RunningSpeed { get; set; }
        private Vector3d _MovementForward;
        private Vector3d _MovementSide;

        //Forces
        private Vector3d _JumpAcceleration;
        private Vector3d _Forces;

        //Orientation
        public Vector3d LookDirection { get; set; }
        private Vector3d _LookDirection;
        public double LookSpeed { get; set; }
        public double LookStopAngleZ { get; set; }

        #endregion Properties



        //Constructor
        public Player(double height)
        {
            Score = 0;

            //Health
            Health = 100;
            IsAlive = true;

            //Location
            FullHeight = height;
            CrouchingHeight = height / 2.0;
            Height = height;
            _Location = new Point3d(0.0, 0.0, 0.0);
            Location = new Point3d(0.0, 0.0, 0.0);

            //Movement
            CrouchingSpeed = 5;
            DefaultSpeed = 10;
            RunningSpeed = 20;
            ActiveSpeed = DefaultSpeed;
            _MovementForward = new Vector3d(0.0, 0.0, 0.0);
            _MovementSide = new Vector3d(0.0, 0.0, 0.0);

            //Forces
            IsAirbourne = false;
            _JumpAcceleration = new Vector3d(0.0, 0.0, 3.5);
            _Forces = new Vector3d(0.0, 0.0, 0.0);

            //Orientation
            _LookDirection = new Vector3d(0.0, 1.0, 0.0);
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
            CheckCollisions();
            UpdateDirection(gameController, deltaTime);
            ThrowSnowballs(gameController);
            

        }

        private void ThrowSnowballs(GameController gameController)
        {
            if (gameController.LeftShoulder.JustPressed)
            {
                Snowball.Snowballs.Add(new Snowball(GetNewSnowballPosition(true), _LookDirection * 25.0));
            }
            if (gameController.RightShoulder.JustPressed)
            {
                Snowball.Snowballs.Add(new Snowball(GetNewSnowballPosition(false), _LookDirection * 25.0));
            }
        }


        //Movement
        #region Movement
        //Update movement
        private void UpdateMovement(GameController gameController, double deltaTime, Vector3d gravity)
        {
            HandleJump(gameController, deltaTime, gravity);
            HandlePosture(gameController);
            GetMovementDirection(gameController, deltaTime);

            SetLocationXY();


            Location = new Point3d(_Location);

            //Move Player Camera
            Rhino.RhinoDoc.ActiveDoc.Views.ActiveView.ActiveViewport.SetCameraLocation(GetCameraPoint(), true);
        }

        //HandleJump
        private void HandleJump(GameController gameController, double deltaTime, Vector3d gravity)
        {
            //Activate Jump
            if (gameController.ButtonA.JustPressed && !IsAirbourne)
            {
                IsAirbourne = true;
                _Forces = _JumpAcceleration;
            }

            //Apply Gravity
            if (IsAirbourne)
            {
                _Location += _Forces * deltaTime;
                _Forces += gravity * deltaTime;

                //End Jump
                if (_Location.Z < 0)
                {
                    IsAirbourne = false;
                    _Location = new Point3d(_Location.X, _Location.Y, 0);
                }
            }
        }

        //Get Speed Changes
        private void HandlePosture(GameController gameController)
        {
            if (!IsAirbourne)
            {
                if (gameController.LeftThumb.IsPressed)
                {
                    ActiveSpeed = RunningSpeed;
                }
                else if (gameController.ButtonB.IsPressed)
                {
                    ActiveSpeed = CrouchingSpeed;
                    IsCrouching = true;
                    Height = CrouchingHeight;
                }
                else
                {
                    ActiveSpeed = DefaultSpeed;
                }
                if (!gameController.ButtonB.IsPressed)
                {
                    IsCrouching = false;
                    Height = FullHeight;
                }
            }
        }

        //Movement Direction Change
        private void GetMovementDirection(GameController gameController, double deltaTime)
        {
            if (!IsAirbourne)
            {
                //Forward vector
                _MovementForward = new Vector3d(_LookDirection.X, _LookDirection.Y, 0.0);

                //Side vector unitized
                _MovementSide = new Vector3d(_MovementForward);
                _MovementSide.Rotate(-Math.PI / 2.0, Vector3d.ZAxis);

                //Scaled Forward Vector
                _MovementForward.Unitize();
                _MovementForward *= gameController.LeftStickY * deltaTime * ActiveSpeed;

                //Scaled side vector
                _MovementSide.Unitize();
                _MovementSide *= gameController.LeftStickX * deltaTime * ActiveSpeed;
            }
        }

        //Move XY
        private void SetLocationXY()
        {
            //Move forward
            _Location += _MovementForward;
            _Location += _MovementSide;

        }

        #endregion Movement

        //Player look direction
        private void UpdateDirection(GameController gameController, double deltaTime)
        {
            _LookDirection.Rotate(-gameController.RightStickX * LookSpeed * deltaTime, Vector3d.ZAxis);

            double cameraAngleZ = Vector3d.VectorAngle(new Vector3d(0.0, 0.0, -1.0), _LookDirection);
            double lookRotationAngle = gameController.RightStickY * LookSpeed * deltaTime;

            if ((cameraAngleZ + lookRotationAngle + LookStopAngleZ < Math.PI && lookRotationAngle > 0) || (cameraAngleZ + lookRotationAngle - LookStopAngleZ > 0 && lookRotationAngle < 0))
            {
                Vector3d lookSideDirection = new Vector3d(_LookDirection.X, _LookDirection.Y, 0.0);
                lookSideDirection.Rotate(-Math.PI / 2.0, Vector3d.ZAxis);
                _LookDirection.Rotate(lookRotationAngle, lookSideDirection);
            }

            LookDirection = new Vector3d(_LookDirection);
            Rhino.RhinoDoc.ActiveDoc.Views.ActiveView.ActiveViewport.SetCameraDirection(_LookDirection, true);
        }

        //Get Camera Point
        private Point3d GetCameraPoint()
        {
            return new Point3d(_Location.X, _Location.Y, _Location.Z + Height);
        }

        private Point3d GetNewSnowballPosition(bool isLeft)
        {
            double rotation = -Math.PI / 2.5;
            if (isLeft)
            {
                rotation *= -1;
            }
            Vector3d offset = new Vector3d(_LookDirection);
            offset.Unitize();
            offset.Rotate(rotation, Vector3d.ZAxis);
            offset *= 1.25;
            Point3d location = new Point3d(GetCameraPoint());
            location += offset;

            return location;
        }

        //Check Collisions
        private void CheckCollisions()
        {
            foreach (Snowman snowman in Snowman.Snowmen)
            {
                if (new Point3d(_Location.X, _Location.Y, 0.0).DistanceTo(snowman.Location) < 1.0)
                {
                    IsAlive = false;
                }
            }
        }

    }
}
