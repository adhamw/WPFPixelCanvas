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


        //## Constructor(s)
        public WoimHeadSegment(Random randomsource) : base(randomsource)
        {
            TargetPosition = GetRandomPosition(); // Start with a random targetposition
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
            Velocity.ClipComponentsToLimits(Boid3DLimits.VelocityMin, Boid3DLimits.VelocityMax);    // Ensure the velocity-value does not grow too large ( or else some boids may shoot off to far off the screen )

            Position = Position + Velocity * DeltaTime;                                             // Update woim-headposition
            Position.ClipComponentsToLimits(Boid3DLimits.PositionsMin, Boid3DLimits.PositionsMax);  // Ensure position does not exceed max values
        }


        //## Public properties
        public Vector3D TargetPosition { get; set; }        // Where is it heading?


        //## Private helpers

        private void UpdateAccelerationVector()
        {
            //Only update with new target, if we are in vicinity of our previous target
            double distanceToTarget = TargetPosition.GetDistanceFrom(Position);
            if (distanceToTarget < DeltaTime * 2) { TargetPosition = GetRandomPosition(); } // Leaderboids pick random targets to move to next

            // Note!! The arithmetic operations for Vector3D are overloaded to
            // perform a operations componentwise.
            // e.g.: a / b = [ a.x/b.x , a.Y/b.Y )

            //Calculate new values for acceleration
            Vector3D deltapos = TargetPosition - Position;
            Vector3D accelerationTarget = AccelerationFactor * distanceToTarget * deltapos;

            //Ensure we don't end up with values that are too large
            accelerationTarget.ClipComponentsToLimits(Boid3DLimits.AccelerationMin, Boid3DLimits.AccelerationMax);

            // Weight the change ( to smoothen it a bit )
            // Using formula: new value = 2/3*new value + 1/3*oldvalue
            Acceleration = 0.3333 * (2.0 * accelerationTarget + 1.0 * Acceleration);
        }
    }
}
