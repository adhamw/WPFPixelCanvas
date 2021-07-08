using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPixelCanvas.Canvas.Models.Boids
{
    public static class BoidLimits
    {
        public static Vector2D PositionsMin { get; set; }
        public static Vector2D PositionsMax { get; set; }
        public static Vector2D VelocityMin { get; set; }
        public static Vector2D VelocityMax { get; set; }
        public static Vector2D AccelerationMin { get; set; }
        public static Vector2D AccelerationMax { get; set; }
    }
}
