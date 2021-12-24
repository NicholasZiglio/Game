using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace Game
{
    class Snowflake
    {
        private static readonly PolylineCurve SnowflakePolylineCurve = (PolylineCurve) Rhino.Runtime.CommonObject.FromJSON(Properties.Resources.SnowflakePolylineCurveJson);
        public static List<Snowflake> Snowflakes = new List<Snowflake>();
        public static List<Polyline> SnowflakePolylines = new List<Polyline>();
        private static readonly Random random = new Random();
        private Point3d _Location;
        private readonly double  _RotationSpeedX;
        private readonly double _RotationSpeedY;
        private double _RotationX;
        private double _RotationY;

        public static double gravityMultiplier = 1.25;
        public static double randomMovementMultiplier = 5.0;
        public static double rotationMultiplier = 5.0;


        public Snowflake(Point3d location)
        {
            _Location = new Point3d(location.X + random.Next(-200, 200), location.Y + random.Next(-200, 200), random.Next(1, 100));
            _RotationSpeedX = random.NextDouble() - 0.5;
            _RotationSpeedY = random.NextDouble()- 0.5;
            _RotationX = random.NextDouble();
            _RotationY = random.NextDouble();
        }

        public static void GenerateSnowflakes(int snowflakes, Point3d location)
        {
            for (int i = 0; i < snowflakes; i++)
            {
                Snowflakes.Add(new Snowflake(location));
            }
        }

        public static void UpdateSnowflakes(Point3d location, double deltaTime, Vector3d gravity)
        {
            SnowflakePolylines.Clear();
            foreach (Snowflake snowflake in Snowflakes)
            {
                snowflake.UpdateSnowflake(location, deltaTime, gravity);
                PolylineCurve snowflakePolylineCurve = (PolylineCurve)SnowflakePolylineCurve.Duplicate();
                snowflakePolylineCurve.Translate((Vector3d)snowflake._Location);
                snowflakePolylineCurve.Rotate(snowflake._RotationX, Vector3d.XAxis, snowflake._Location);
                snowflakePolylineCurve.Rotate(snowflake._RotationY, Vector3d.XAxis, snowflake._Location);
                SnowflakePolylines.Add(snowflakePolylineCurve.ToPolyline());
            }

        }

        private void UpdateSnowflake(Point3d location, double deltaTime, Vector3d gravity)
        {
            _Location += gravity * gravityMultiplier * deltaTime;
            _Location += new Vector3d(random.NextDouble(), random.NextDouble(), random.NextDouble()) * randomMovementMultiplier * deltaTime;
            _RotationX += _RotationSpeedX * deltaTime * rotationMultiplier;
            _RotationY += _RotationSpeedY * deltaTime * rotationMultiplier;

            if (_Location.Z < 0)
            {
                _Location = new Point3d(location.X + random.Next(-200, 200), location.Y + random.Next(-200, 200), random.Next(75, 100));
            }

            
        }

    }
}
