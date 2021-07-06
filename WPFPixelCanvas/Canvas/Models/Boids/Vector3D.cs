using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace WPFPixelCanvas.Canvas.Models.Boids
{
    public struct Vector3D
    {
        //Operator overloads
        public static Vector3D operator +(Vector3D a, Vector3D b) { return new Vector3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z); }
        public static Vector3D operator -(Vector3D a, Vector3D b) { return new Vector3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z); }
        public static Vector3D operator *(double a, Vector3D b) { return new Vector3D(a * b.X, a * b.Y, a * b.Z); }
        public static Vector3D operator *(Vector3D a, double b) { return new Vector3D(b * a.X, b * a.Y, b * a.Z); }
        public static Vector3D operator *(Vector3D a, Vector3D b) { return new Vector3D(a.X * b.X, a.Y * b.Y, a.Z * b.Z); }
        public static Vector3D operator /(Vector3D a, Vector3D b) { return new Vector3D(a.X / b.X, a.Y / b.Y, a.Z / b.Z); }

        //Constructor
        public Vector3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;

        }

        //Public interface
        public void ClipComponentsToLimits(Vector3D minValue,Vector3D maxValue)
        {
            if (X > maxValue.X) { X = maxValue.X; } else if (X < minValue.X) { X = minValue.X; }
            if (Y > maxValue.Y) { Y = maxValue.Y; } else if (Y < minValue.Y) { Y = minValue.Y; }
            if (Z > maxValue.Z) { Z = maxValue.Z; } else if (Z < minValue.Z) { Z = minValue.Z; }
        }
        public double GetDistanceSquaredFrom(Vector3D other)
        {
            double dx = other.X - X;
            double dy = other.Y - Y;
            double dz = other.Z - Z;
            return dx * dx + dy * dy + dz * dz;
        }
        public double GetDistanceFrom(Vector3D other) { return Math.Sqrt(GetDistanceSquaredFrom(other));}


        //Public properties
        public double X { get; set; } 
        public double Y { get; set; } 
        public double Z { get; set; } 



    }
}
