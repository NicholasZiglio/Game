using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace Game
{
    class Snowball
    {
        //Properties
        #region Properties

        public bool IsAlive { get; set; }
        
        
        private Point3d _Location;
        public Point3d Location { get; set; }
        public double Radius { get; set; }
        private Vector3d _Direction;


        public Sphere Render;

        #endregion Properties

        //Constructor
        public Snowball(Point3d location, Vector3d direction)
        {
            IsAlive = true;
            Radius = 0.15;
            _Direction = direction;
            _Location = new Point3d(location.X, location.Y, location.Z);
            Render = new Sphere(location, Radius);

        }

        //Update
        public void Update(Vector3d gravity, double deltaTime)
        {
            CheckAboveGround();
            if (IsAlive)
            {
                
                Vector3d move = new Vector3d(_Direction * deltaTime);
                _Location += move;
                Render.Translate(move);
                _Direction += gravity * deltaTime;


                Location = new Point3d(Location);
            } 
        }

        private void CheckAboveGround()
        {
            if (_Location.Z <= 0)
            {
                IsAlive = false;
                Render = Sphere.Unset;
            }
        }
    }
}
