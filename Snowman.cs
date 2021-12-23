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
        //Body
        public static List<Mesh> SnowmenBodyMeshes = new List<Mesh>();
        private static readonly Mesh SnowmanBodyMesh = (Mesh)Rhino.Runtime.CommonObject.FromJSON(Properties.Resources.SnowmanBodyMeshJson);
        //Nose
        public static List<Mesh> SnowmenNoseMeshes = new List<Mesh>();
        private static readonly Mesh SnowmanNoseMesh = (Mesh)Rhino.Runtime.CommonObject.FromJSON(Properties.Resources.NoseMeshJson);
        //Arms
        public static List<Mesh> SnowmenArmsMeshes = new List<Mesh>();
        private static readonly Mesh SnowmanArm1Mesh = (Mesh)Rhino.Runtime.CommonObject.FromJSON(Properties.Resources.Arm1MeshJson);
        private static readonly Mesh SnowmanArm2Mesh = (Mesh)Rhino.Runtime.CommonObject.FromJSON(Properties.Resources.Arm2MeshJson);
        //Black Meshes
        public static List<Mesh> SnowmenBlackMeshes = new List<Mesh>();
        #region BlackMeshes
        private static readonly Mesh Button1Mesh = (Mesh)Rhino.Runtime.CommonObject.FromJSON(Properties.Resources.Button1MeshJson);
        private static readonly Mesh Button2Mesh = (Mesh)Rhino.Runtime.CommonObject.FromJSON(Properties.Resources.Button2MeshJson);
        private static readonly Mesh Button3Mesh = (Mesh)Rhino.Runtime.CommonObject.FromJSON(Properties.Resources.Button3MeshJson);
        private static readonly Mesh Button4Mesh = (Mesh)Rhino.Runtime.CommonObject.FromJSON(Properties.Resources.Button4MeshJson);
        private static readonly Mesh Button5Mesh = (Mesh)Rhino.Runtime.CommonObject.FromJSON(Properties.Resources.Button5MeshJson);
        private static readonly Mesh Button6Mesh = (Mesh)Rhino.Runtime.CommonObject.FromJSON(Properties.Resources.Button6MeshJson);
        private static readonly Mesh Button7Mesh = (Mesh)Rhino.Runtime.CommonObject.FromJSON(Properties.Resources.Button7MeshJson);
        private static readonly Mesh Button8Mesh = (Mesh)Rhino.Runtime.CommonObject.FromJSON(Properties.Resources.Button8MeshJson);
        private static readonly Mesh Button9Mesh = (Mesh)Rhino.Runtime.CommonObject.FromJSON(Properties.Resources.Button9MeshJson);
        private static readonly Mesh Button10Mesh = (Mesh)Rhino.Runtime.CommonObject.FromJSON(Properties.Resources.Button10MeshJson);
        private static readonly Mesh Button11Mesh = (Mesh)Rhino.Runtime.CommonObject.FromJSON(Properties.Resources.Button11MeshJson);
        private static readonly Mesh Button12Mesh = (Mesh)Rhino.Runtime.CommonObject.FromJSON(Properties.Resources.Button12MeshJson);
        private static readonly Mesh Button13Mesh = (Mesh)Rhino.Runtime.CommonObject.FromJSON(Properties.Resources.Button13MeshJson);
        private static readonly Mesh Button14Mesh = (Mesh)Rhino.Runtime.CommonObject.FromJSON(Properties.Resources.Button14MeshJson);
        private static readonly Mesh Button15Mesh = (Mesh)Rhino.Runtime.CommonObject.FromJSON(Properties.Resources.Button15MeshJson);
        private static readonly Mesh Button16Mesh = (Mesh)Rhino.Runtime.CommonObject.FromJSON(Properties.Resources.Button16MeshJson);
        private static readonly Mesh Button17Mesh = (Mesh)Rhino.Runtime.CommonObject.FromJSON(Properties.Resources.Button17MeshJson);
        private static readonly Mesh Button18Mesh = (Mesh)Rhino.Runtime.CommonObject.FromJSON(Properties.Resources.Button18MeshJson);
        private static readonly Mesh Button19Mesh = (Mesh)Rhino.Runtime.CommonObject.FromJSON(Properties.Resources.Button19MeshJson);
        private static readonly Mesh Button20Mesh = (Mesh)Rhino.Runtime.CommonObject.FromJSON(Properties.Resources.Button20MeshJson);
        private static readonly Mesh Button21Mesh = (Mesh)Rhino.Runtime.CommonObject.FromJSON(Properties.Resources.Button21MeshJson);
        private static readonly Mesh Button22Mesh = (Mesh)Rhino.Runtime.CommonObject.FromJSON(Properties.Resources.Button22MeshJson);
        private static readonly Mesh HatMesh = (Mesh)Rhino.Runtime.CommonObject.FromJSON(Properties.Resources.HatMeshJson);
        private static readonly List<Mesh> SnowmanBlackMeshes = new List<Mesh>()
        {
            Button1Mesh,
            Button2Mesh,
            Button3Mesh,
            Button4Mesh,
            Button5Mesh,
            Button6Mesh,
            Button7Mesh,
            Button8Mesh,
            Button9Mesh,
            Button10Mesh,
            Button11Mesh,
            Button12Mesh,
            Button13Mesh,
            Button14Mesh,
            Button15Mesh,
            Button16Mesh,
            Button17Mesh,
            Button18Mesh,
            Button19Mesh,
            Button20Mesh,
            Button21Mesh,
            Button22Mesh,
            HatMesh
        };
        #endregion BlackMeshes

        //Static
        public static List<Snowman> Snowmen = new List<Snowman>();
        public static int StartSpeed = 5;
        public static int StartHealth = 300;
        public static int HeadshotMultiplier = 10;
        private static int Deaths = 0;
        private static readonly Random random = new Random();
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

                    if (StartSpeed < 10)
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
            SnowmenNoseMeshes.Clear();
            SnowmenArmsMeshes.Clear();
            SnowmenBlackMeshes.Clear();
            foreach (Snowman snowman in Snowmen)
            {
                SnowmenBodyMeshes.Add(snowman.UpdateSnowmanMesh(SnowmanBodyMesh));
                SnowmenNoseMeshes.Add(snowman.UpdateSnowmanMesh(SnowmanNoseMesh));
                SnowmenArmsMeshes.Add(snowman.UpdateSnowmanMesh(SnowmanArm1Mesh));
                SnowmenArmsMeshes.Add(snowman.UpdateSnowmanMesh(SnowmanArm2Mesh));
                foreach (Mesh mesh in SnowmanBlackMeshes)
                {
                    SnowmenBlackMeshes.Add(snowman.UpdateSnowmanMesh(mesh));
                }
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
