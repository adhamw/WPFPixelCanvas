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
                case PathTypes.Lissajous2D:
                    _CurrentPathGenerator = CalculateLissaJousPath2D;
                    break;
                case PathTypes.Lissajous3D:
                    _CurrentPathGenerator = CalculateLissaJousPath3D;
                    break;
                default:
                    _CurrentPathGenerator = calculateCirclePath;
                    break;
            }
        }

        public void OffsetAngleBy(double value)
        {
            _angle += value;
        }

        //## Public properties
        //## Private helpers
        private double _angle;
        private Vector3D calculateCirclePath()
        {
            double x = _origo.X + 0.9*_radius.X * Math.Sin(_angle);
            double y = _origo.Y + 0.9 * _radius.Y * Math.Cos(_angle);
            double z = 1.0;
            _angle += _stepSize;

            return new Vector3D(x, y, z);
        }

        private Vector3D CalculateLissaJousPath2D()
        {
            double x = _origo.X + 0.9 * _radius.X * (Math.Sin(_angle)+Math.Cos(2*_angle));
            double y = _origo.Y + 0.9 * _radius.Y * Math.Cos(_angle) + Math.Cos(-_angle);
            double z = 1.0;
            _angle += _stepSize;

            return new Vector3D(x, y, z);
        }

        double _angle2 = 0;
        double _angleChange = 0;
        private Vector3D CalculateLissaJousPath3D()
        {
            double x = _origo.X +  _radius.X * ( Math.Sin(0.8 * Math.Sin(2 * (_angle + _angle2))));
            double y = _origo.Y +  _radius.Y * ( Math.Cos(_angle));
            double z = _maxValue.Z * (1 + 0.8 * Math.Cos(2 * Math.Sin(2 * (_angle + _angle2))));
            _angle += _stepSize;


            _angleChange += _stepSize;
            if(_angleChange>3.14)
            {
                _angleChange -= 3.14;
                _angle2 += 0.01;
            }
            

            return new Vector3D(x, y, z);
        }


    }
}
