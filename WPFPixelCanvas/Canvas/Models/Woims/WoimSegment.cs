using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFPixelCanvas.Canvas.Models.Boids3D;

namespace WPFPixelCanvas.Canvas.Models.Woims
{
    // Base class for woim segments
    public abstract class WoimSegment
    {
        //## Private fields
        protected Random _randomSource { get; set; }
        protected Woim3DLimits _limits { get; set; }


        //## Constructor(s)
        public WoimSegment(Random randomsource, Woim3DLimits limits, Vector3D position=null, Vector3D velocity=null, Vector3D  acceleration=null )
        {
            //Initialize private fields
            _randomSource = randomsource;                   // Feeds us random numbers
            _limits = limits;                               // Limits on velocity, acceleration and position

            //Default woim movement parameters
            AccelerationFactor = 0.3;                      // Affects how "fast" woim segment can turn
            DeltaTime = 0.3;                               // Affects .. everything

            Position = position??GetRandomPosition();               // Segment start at given position ( or random if none given )
            Acceleration = acceleration??GetRandomAcceleration();   // Segment start with given acceleration ( or random if none given )
            Velocity = velocity??GetRandomVelocity();               // Segment start with given velocity ( or pick at random if none given )
        }


        //## Public interface
        public double GetDistanceFrom(WoimSegment other) { return other.Position.GetDistanceFrom(this.Position); }
        public double GetSquaredDistanceFrom(WoimSegment other) { return other.Position.GetDistanceSquaredFrom(this.Position); }


        //## Public interface
        public abstract void Update();


        //## Public properties       
        public Vector3D Position { get; set; }              // Where is it location
        public Vector3D Velocity { get; set; }              // How fast is it going?
        public Vector3D Acceleration { get; set; }          // How fast can it adjust?
        public Tuple<byte, byte, byte> Color { get; set; }  // Woim segment color
        public double AccelerationFactor { get; set; }      // Affects impact of acceleration 
        public double DeltaTime { get; set; }               // The timestep. Affects everything.
        public Vector3D StartPos { get; set; }              // Where on screen is the woim segment added


        //## Private/protected helpers
        protected Vector3D GetRandomAcceleration()
        {
            if (_randomSource == null) { _randomSource = new Random(); }
            double x = _limits.AccelerationMin.X + _randomSource.NextDouble() * (_limits.AccelerationMax.X - _limits.AccelerationMin.X);
            double y = _limits.AccelerationMin.Y + _randomSource.NextDouble() * (_limits.AccelerationMax.Y - _limits.AccelerationMin.Y);
            double z = _limits.AccelerationMin.Z + _randomSource.NextDouble() * (_limits.AccelerationMax.Z - _limits.AccelerationMin.Z);

            return new Vector3D(x, y, z);
        }
        protected Vector3D GetRandomVelocity()
        {
            if (_randomSource == null) { _randomSource = new Random(); }
            double x = _limits.VelocityMin.X + _randomSource.NextDouble() * (_limits.VelocityMax.X - _limits.VelocityMin.X);
            double y = _limits.VelocityMin.Y + _randomSource.NextDouble() * (_limits.VelocityMax.Y - _limits.VelocityMin.Y);
            double z = _limits.VelocityMin.Z + _randomSource.NextDouble() * (_limits.VelocityMax.Z - _limits.VelocityMin.Z);

            return new Vector3D(x, y, z);
        }
        protected Vector3D GetRandomPosition()
        {
            //if (_randomSource == null) { _randomSource = new Random(); }
            double x = _limits.PositionsMin.X + _randomSource.NextDouble() * (_limits.PositionsMax.X - _limits.PositionsMin.X);
            double y = _limits.PositionsMin.Y + _randomSource.NextDouble() * (_limits.PositionsMax.Y - _limits.PositionsMin.Y);
            double z = _limits.PositionsMin.Z + _randomSource.NextDouble() * (_limits.PositionsMax.Z - _limits.PositionsMin.Z);

            return new Vector3D(x, y, z);
        }




    }
}
