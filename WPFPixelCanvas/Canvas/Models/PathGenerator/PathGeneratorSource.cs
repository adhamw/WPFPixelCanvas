using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFPixelCanvas.Canvas.Models.Boids3D;

namespace WPFPixelCanvas.Canvas.Models.PathGenerator
{
    /// <summary>
    /// Provides various sin/cos paths
    /// </summary>
    public class PathGeneratorSource
    {
        //## Private fields
        private delegate Vector3D CurrentPathGeneratorDelegate();
        CurrentPathGeneratorDelegate _CurrentPathGenerator;
        Vector3D _minValue { get; set; }
        Vector3D _maxValue { get; set; }
        Vector3D _origo { get; set; }
        Vector3D _radius { get; set; }

        private int _callCounter { get; set; }
        private double _stepSize{ get; set; }


        //## Constructor(s)
        public PathGeneratorSource(Vector3D minValue, Vector3D maxValue, double stepsize)
        {
            //Store parameters
            _minValue = minValue;
            _maxValue = maxValue;
            _origo = 0.5 * (maxValue + minValue);
            _radius = 0.5*(maxValue - minValue);
            _stepSize = stepsize;

            //Default uses a circle path
            SetPathType(PathTypes.Circle);
        }

        //## Public interface
        public Vector3D GetNextPoint() { return _CurrentPathGenerator();}
        public void SetPathType(PathTypes pathtype)
        {
            _callCounter = 0;

            switch (pathtype)
            {
                case PathTypes.Circle:
                    _CurrentPathGenerator = calculateCirclePath;
                    break;
                case PathTypes.Lissajous1:
                    _CurrentPathGenerator = calculateLissaJousPath1;
                    break;
                default:
                    _CurrentPathGenerator = calculateCirclePath;
                    break;
            }
        }

        //## Public properties
        //## Private helpers
        private double angle;
        private Vector3D calculateCirclePath()
        {
            double x = _origo.X + 0.9*_radius.X * Math.Sin(angle);
            double y = _origo.Y + 0.9 * _radius.Y * Math.Cos(angle);
            double z = 1.0;
            angle += _stepSize;

            return new Vector3D(x, y, z);
        }

        private Vector3D calculateLissaJousPath1()
        {
            double x = _origo.X + 0.9 * _radius.X * (Math.Sin(angle)+Math.Cos(2*angle));
            double y = _origo.Y + 0.9 * _radius.Y * Math.Cos(angle) + Math.Cos(-angle);
            double z = 1.0;
            angle += _stepSize;

            return new Vector3D(x, y, z);
        }

    }
}
