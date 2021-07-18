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
        private Random _randomSource { get; set; }


        //## Constructor(s)
        public WoimSegment(Random randomsource, Vector3D position=null, Vector3D velocity=null, Vector3D  acceleration=null )
        {
            //Initialize private fields
            _randomSource = randomsource;           // Feeds us random numbers

            //Default woim movement parameters
            AccelerationFactor = 0.02;                      // Affects how "fast" woim segment can turn
            DeltaTime = 0.01;                               // Affects .. everything

            Position = position??GetRandomPosition();               // Segment start at given position ( or random if none given )
            Acceleration = acceleration??GetRandomAcceleration();   // Segment start with given acceleration ( or random if none given )
            Velocity = velocity??GetRandomVelocity();               // Segment start with given velocity ( or pick at random if none given )
        }
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
            double x = Boid3DLimits.AccelerationMin.X + _randomSource.NextDouble() * (Boid3DLimits.AccelerationMax.X - Boid3DLimits.AccelerationMin.X);
            double y = Boid3DLimits.AccelerationMin.Y + _randomSource.NextDouble() * (Boid3DLimits.AccelerationMax.Y - Boid3DLimits.AccelerationMin.Y);
            double z = Boid3DLimits.AccelerationMin.Z + _randomSource.NextDouble() * (Boid3DLimits.AccelerationMax.Z - Boid3DLimits.AccelerationMin.Z);

            return new Vector3D(x, y, z);
        }
        protected Vector3D GetRandomVelocity()
        {
            if (_randomSource == null) { _randomSource = new Random(); }
            double x = Boid3DLimits.VelocityMin.X + _randomSource.NextDouble() * (Boid3DLimits.VelocityMax.X - Boid3DLimits.VelocityMin.X);
            double y = Boid3DLimits.VelocityMin.Y + _randomSource.NextDouble() * (Boid3DLimits.VelocityMax.Y - Boid3DLimits.VelocityMin.Y);
            double z = Boid3DLimits.VelocityMin.Z + _randomSource.NextDouble() * (Boid3DLimits.VelocityMax.Z - Boid3DLimits.VelocityMin.Z);

            return new Vector3D(x, y, z);
        }
        protected Vector3D GetRandomPosition()
        {
            if (_randomSource == null) { _randomSource = new Random(); }
            double x = Boid3DLimits.PositionsMin.X + _randomSource.NextDouble() * (Boid3DLimits.PositionsMax.X - Boid3DLimits.PositionsMin.X);
            double y = Boid3DLimits.PositionsMin.Y + _randomSource.NextDouble() * (Boid3DLimits.PositionsMax.Y - Boid3DLimits.PositionsMin.Y);
            double z = Boid3DLimits.PositionsMin.Z + _randomSource.NextDouble() * (Boid3DLimits.PositionsMax.Z - Boid3DLimits.PositionsMin.Z);

            return new Vector3D(x, y, z);
        }

    }
}
