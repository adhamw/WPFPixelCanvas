using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPixelCanvas.Canvas.Models.Boids3D
{
    public static class Boid3DLimits
    {
        public static Vector3D PositionsMin { get; set; }
        public static Vector3D PositionsMax { get; set; }
        public static Vector3D VelocityMin { get; set; }
        public static Vector3D VelocityMax { get; set; }
        public static Vector3D AccelerationMin { get; set; }
        public static Vector3D AccelerationMax { get; set; }
    }
}