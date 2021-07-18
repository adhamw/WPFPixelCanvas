using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFPixelCanvas.Canvas.Models.Boids3D;

namespace WPFPixelCanvas.Canvas.Models.Woims
{
    public class WoimHeadSegment : WoimSegment
    {
        //## Private fields
        private int _refreshCounter { get; set; }

        //## Constructor(s)
        public WoimHeadSegment(Random randomsource, Woim3DLimits limits) : base(randomsource,limits)
        {
            _refreshCounter = 0;
            TargetPosition = GetNextTargetPosition(); // GetRandomPosition(); // Start with a random targetposition
           
            //Default woim movement parameters
            AccelerationFactor = 0.5;                      // Affects how "fast" woim segment can turn
            DeltaTime = 0.1;

           
        }

        //## Public interface
        /// <summary>
        /// Progresses woim segment one time step forward.
        /// </summary>
        public override void Update()
        {

            //Update boid state ( I.e. position, velocity, acceleration etc )
            UpdateAccelerationVector();                                                             // Boid accelerate towards the target

            Velocity = Velocity + Acceleration * DeltaTime;                                         // Update woim velocity
            Velocity.ClipComponentsToLimits(_limits.VelocityMin, _limits.VelocityMax);    // Ensure the velocity-value does not grow too large ( or else some boids may shoot off to far off the screen )

            Position = Position + Velocity * DeltaTime;                                             // Update woim-headposition
            if (Position.X <= _limits.PositionsMin.X) { Acceleration.X = -Acceleration.X; }
            if (Position.Y <= _limits.PositionsMin.Y) { Acceleration.Y = -Acceleration.Y; }
            if (Position.Z <= _limits.PositionsMin.Z) { Acceleration.Z = -Acceleration.Z; }

            if (Position.X >= _limits.PositionsMax.X) { Acceleration.X = -Acceleration.X; }
            if (Position.Y >= _limits.PositionsMax.Y) { Acceleration.Y = -Acceleration.Y; }
            if (Position.Z >= _limits.PositionsMax.Z) { Acceleration.Z = -Acceleration.Z; }

            //Position.ClipComponentsToLimits(_limits.PositionsMin, _limits.PositionsMax);  // Ensure position does not exceed max values
            _refreshCounter++;

        }


        //## Public properties
        public Vector3D TargetPosition { get; set; }        // Where is it heading?


        //## Private helpers

        private void UpdateAccelerationVector()
        {
            //Only update with new target, if we are in vicinity of our previous target
            double distanceToTarget = TargetPosition.GetDistanceFrom(Position);
            Vector3D maxDisplacement = _limits.VelocityMax;
            double distanceComparison = maxDisplacement.X > maxDisplacement.Y ? maxDisplacement.X : maxDisplacement.Y; ;
            if (distanceToTarget < 10*distanceComparison) 
            {
                // New target position

                TargetPosition = GetNextTargetPosition();
            } // Leaderboids pick random targets to move to next

            // Note!! The arithmetic operations for Vector3D are overloaded to
            // perform a operations componentwise.
            // e.g.: a / b = [ a.x/b.x , a.Y/b.Y )

            //Calculate new values for acceleration
            Vector3D deltapos = TargetPosition - Position;
            Vector3D accelerationTarget = AccelerationFactor * distanceToTarget * deltapos;

            //Ensure we don't end up with values that are too large
            accelerationTarget.ClipComponentsToLimits(_limits.AccelerationMin, _limits.AccelerationMax);

            // Weight the change ( to smoothen it a bit )
            // Using formula: new value = 2/3*new value + 1/3*oldvalue
            Acceleration = 0.3333 * (2.0 * accelerationTarget + 1.0 * Acceleration);
        }

        private double _angle { get; set; } = 0.0;
        private Vector3D GetNextTargetPosition()
        {
            //Calculate the centre coordinate
            var radii = 0.5 * (_limits.PositionsMax - _limits.PositionsMin);
            var origo = _limits.PositionsMin + radii;

            double x = origo.X + (0.8 * radii.X * Math.Sin(_angle));
            double y = origo.Y + (0.8 * radii.X * Math.Cos(_angle));
            _angle += (3.14 / 4.0);


            //if (_randomSource == null) { _randomSource = new Random(); }
            //double x = xmin + (xmax - xmin) * _randomSource.NextDouble();
            //double y = ymin + (ymax - ymin) * _randomSource.NextDouble();
            //double x = xmin + (xmax - xmin) * _randomSource.NextDouble();
            //double x = _limits.PositionsMin.X + _randomSource.NextDouble() * (_limits.PositionsMax.X - _limits.PositionsMin.X);
            //double y = _limits.PositionsMin.Y + _randomSource.NextDouble() * (_limits.PositionsMax.Y - _limits.PositionsMin.Y);
            double z = _limits.PositionsMin.Z + _randomSource.NextDouble() * (_limits.PositionsMax.Z - _limits.PositionsMin.Z);

            return new Vector3D(x, y, z);
        }

    }
}
