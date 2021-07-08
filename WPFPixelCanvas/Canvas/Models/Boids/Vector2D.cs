using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace WPFPixelCanvas.Canvas.Models.Boids
{
    public class Vector2D
    {
        // Operator overloads to allow componentwise operations.
        // ( Note that these do not conform with standard mathematical vector operations! )
        // ( They are simply there to simplify the code a bit )
        public static Vector2D operator +(Vector2D a, Vector2D b) { return new Vector2D(a.X + b.X, a.Y + b.Y); }
        public static Vector2D operator -(Vector2D a, Vector2D b) { return new Vector2D(a.X - b.X, a.Y - b.Y); }
        public static Vector2D operator *(double a, Vector2D b) { return new Vector2D(a * b.X, a * b.Y); }
        public static Vector2D operator *(Vector2D a, double b) { return new Vector2D(b * a.X, b * a.Y); }
        public static Vector2D operator *(Vector2D a, Vector2D b) { return new Vector2D(a.X * b.X, a.Y * b.Y); }
        public static Vector2D operator /(Vector2D a, Vector2D b) { return new Vector2D(a.X / b.X, a.Y / b.Y); }

        //Constructor
        public Vector2D(double x, double y) { X = x; Y = y; }

        //Public interface
        public void ClipComponentsToLimits(Vector2D minValue, Vector2D maxValue)
        {
            if (X >= maxValue.X) { X = maxValue.X; } else if (X <= minValue.X) { X = minValue.X; }
            if (Y >= maxValue.Y) { Y = maxValue.Y; } else if (Y <= minValue.Y) { Y = minValue.Y; }
        }
        public double GetDistanceSquaredFrom(Vector2D other)
        {
            double dx = other.X - X;
            double dy = other.Y - Y;
            return dx * dx + dy * dy;
        }
        public double GetDistanceFrom(Vector2D other) { return Math.Sqrt(GetDistanceSquaredFrom(other)); }


        //Public properties
        public double X { get; set; }
        public double Y { get; set; }
    }
}
