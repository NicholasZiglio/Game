using System;
using System.IO;
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

        //Static
        public static List<Snowball> Snowballs = new List<Snowball>();
        public static List<Mesh> SnowballMeshes = new List<Mesh>();
        private static Mesh SnowballMesh = (Mesh) Rhino.Runtime.CommonObject.FromJSON(Properties.Resources.SnowballMeshJson);

        public static int startDamage = 100;

        //Instance
        public bool IsAlive { get; set; }
        public double Radius { get; set; }

        private Point3d _Location;
        public Point3d Location { get; set; }

        private Vector3d _Direction;

        public int damage;

        #endregion Properties

        //Constructor
        public Snowball(Point3d location, Vector3d direction)
        {
            IsAlive = true;
            Radius = 0.15;
            _Direction = direction;
            _Location = new Point3d(location.X, location.Y, location.Z);
            damage = startDamage;
            
        }

        public static void Update(double deltaTime, Vector3d gravity)
        {
            UpdateSnowballs(deltaTime, gravity);
            RenderSnowballs();
        }

        //Update Snowball Instance
        public void UpdateSnowball(double deltaTime, Vector3d gravity)
        {
            CheckAboveGround();
            if (IsAlive)
            {
                _Location += _Direction * deltaTime;
                _Direction += gravity * deltaTime;

                Location = new Point3d(_Location);
            } 
        }

        //Update All Snowballs
        public static void UpdateSnowballs(double deltaTime, Vector3d gravity)
        {
            foreach (Snowball snowball in Snowballs)
            {
                snowball.UpdateSnowball(deltaTime, gravity);
                if (!snowball.IsAlive)
                {
                    Snowballs.Remove(snowball);
                }
            }
        }

        //Render Snowballs
        public static void RenderSnowballs()
        {
            SnowballMeshes.Clear();
            foreach (Snowball snowball in Snowballs)
            {
                Mesh snowballMesh = SnowballMesh.DuplicateMesh();
                snowballMesh.Translate((Vector3d) snowball._Location);
                SnowballMeshes.Add(snowballMesh);
            }
        }

        //Check if Above Ground
        private void CheckAboveGround()
        {
            if (_Location.Z <= 0)
            {
                IsAlive = false;
            }
        }
    }
}
