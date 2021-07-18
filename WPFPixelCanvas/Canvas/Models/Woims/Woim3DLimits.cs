// Uses Vector3D from Boid3D

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFPixelCanvas.Canvas.Models.Boids3D;

namespace WPFPixelCanvas.Canvas.Models.Woims
{
    public class Woim3DLimits
    {
        public Vector3D PositionsMin { get; set; }
        public Vector3D PositionsMax { get; set; }
        public Vector3D VelocityMin { get; set; }
        public Vector3D VelocityMax { get; set; }
        public Vector3D AccelerationMin { get; set; }
        public Vector3D AccelerationMax { get; set; }
    }
}


