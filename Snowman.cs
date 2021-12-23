using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace Game
{
    class Snowman
    {
        #region Properties

        //Mesh
        public static List<Mesh> SnowmenBodyMeshes = new List<Mesh>();
        private static Mesh SnowmanBodyMesh = (Mesh)Rhino.Runtime.CommonObject.FromJSON(Properties.Resources.SnowmanBodyMeshJson);

        //Static
        public static List<Snowman> Snowmen = new List<Snowman>();
        public static int StartSpeed = 5;
        public static int StartHealth = 300;
        public static int HeadshotMultiplier = 10;
        private static int Deaths = 0;
        private static Random random = new Random();
        public static double SpawnRadius = 100.0;

        //Instance
        //Health
        public bool IsAlive { get; set; }
        public int health;
        public Point3d bodyCollisionPoint = Point3d.Unset;
        public Point3d headCollisionPoint = Point3d.Unset;

        //Position
        public Point3d Location { get; set; }
        private Point3d _Location;
        private Vector3d _BodyCollisionOffset = new Vector3d(0.0, 0.0, 1.0);
        private Vector3d _HeadCollisionOffset = new Vector3d(0.0, 0.0, 0.6); 

        //Orientation
        public Vector3d Orientation { get; set; }
        private Vector3d _Orientation;
        public double Rotation { get; set; }

        //Movement
        public int Speed { get; set; }

        

        #endregion Properties


        //Constructor
        public Snowman(Point3d location)
        {
            health = StartHealth;
            Speed = StartSpeed;

            IsAlive = true;

            _Location = GetRandomXYPoint(location, SpawnRadius);
            Location = new Point3d(_Location);

            _Orientation = new Vector3d(location - _Location);
            Orientation = new Vector3d(_Orientation);

            Rotation = GetRotation();
        }

        public static void Update(Player player, double deltaTime)
        {
            UpdateSnowmenMovements(player.Location, deltaTime);
            CheckAllSnowmenCollision();
            HandleSnowmenDeaths(player);
            UpdateAllSnowmenMeshes();
        }


        //Collisions
        #region Collisions

        //Check Collisions for all Snowmen
        private static void CheckAllSnowmenCollision()
        {
            foreach (Snowman snowman in Snowmen)
            {
                foreach (Snowball snowball in Snowball.Snowballs)
                {
                    snowman.CheckSnowmanCollisions(snowball.Location, snowball.damage);

                }

            }
        }

        //Check Snowman Collisions
        private void CheckSnowmanCollisions(Point3d snowballPoint, int snowballdamage)
        {
            UpdateCollisionPoints();

            CheckPointPointCollision(snowballPoint, bodyCollisionPoint, _BodyCollisionOffset.Z, snowballdamage);
            int headshotDamage = snowballdamage * HeadshotMultiplier;
            CheckPointPointCollision(snowballPoint, headCollisionPoint, _HeadCollisionOffset.Z, headshotDamage);

            if (health < 0)
            {
                IsAlive = false;
            }
        }

        //Update Body & Head Collision Points
        private void UpdateCollisionPoints()
        {
            bodyCollisionPoint = new Point3d(_Location);
            bodyCollisionPoint += _BodyCollisionOffset;
            headCollisionPoint = new Point3d(bodyCollisionPoint);
            headCollisionPoint += _BodyCollisionOffset;
            headCollisionPoint += _HeadCollisionOffset;

        }

        //Check Point to Point Collision
        private void CheckPointPointCollision (Point3d snowballPoint, Point3d snowmanPoint, double radius, int snowballdamage)
        {
            if (snowballPoint.DistanceTo(snowmanPoint) <= radius)
            {
                health -= snowballdamage;
            }
        }
       
        #endregion Collisions

        //Handle Deaths
        private static void HandleSnowmenDeaths(Player player)
        {
            foreach (Snowman snowman in Snowmen)
            {
                if (!snowman.IsAlive)
                {
                    Deaths++;

                    if (StartSpeed < 15)
                    { 
                        StartSpeed += 1;
                    }

                    StartHealth += 10;

                    Snowmen.Add(new Snowman(player.Location));
                    
                    if (Deaths % 5 == 0)
                    {
                        Snowmen.Add(new Snowman(player.Location));
                    }

                    Snowmen.Remove(snowman);
                }
            }
        }

        //Get XY Point;
        private static Point3d GetPointXY(Point3d point)
        {
            return new Point3d(point.X, point.Y, 0.0);
        }

        //Get Random Point
        private static Point3d GetRandomXYPoint(Point3d location, double radius)
        {
            Vector3d moveVector = Vector3d.XAxis;
            moveVector.Rotate(Math.PI * 2.0 * Snowman.random.NextDouble(), Vector3d.ZAxis);
            moveVector *= radius;

            Point3d point = GetPointXY(location) + (Point3d)moveVector;
            return point;
        }

        //Handle Snowmen Movements
        private static void UpdateSnowmenMovements(Point3d location, double deltaTime)
        {
            foreach (Snowman snowman in Snowmen)
            {
                snowman._Orientation = new Vector3d(GetPointXY(location) - snowman._Location);
                snowman._Orientation.Unitize();
                snowman.Orientation = new Vector3d(snowman._Orientation);
                snowman._Location += snowman._Orientation * snowman.Speed * deltaTime;
                snowman.Location = new Point3d(snowman._Location);
                snowman.Rotation = snowman.GetRotation();
            }    
        }

        //Render All Snowmen
        private static void UpdateAllSnowmenMeshes()
        {
            SnowmenBodyMeshes.Clear();
            foreach (Snowman snowman in Snowmen)
            {
                SnowmenBodyMeshes.Add(snowman.UpdateSnowmanMesh(SnowmanBodyMesh));
            }
        }

        //Update Snowman Meshes
        private Mesh UpdateSnowmanMesh(Mesh meshIn)
        {
            Mesh mesh = meshIn.DuplicateMesh();
            mesh.Translate((Vector3d)_Location);
            mesh.Rotate(Rotation, Vector3d.ZAxis, _Location);
            return mesh;
        }

        //Get Rotation of Snowman
        private double GetRotation()
        {
            return Vector3d.VectorAngle(Vector3d.XAxis, _Orientation, Plane.WorldXY);
        }

    }
}
