using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFPixelCanvas.Canvas.Models.Boids3D;

namespace WPFPixelCanvas.Canvas.Models.Woims
{
    public class WoimBodySegment : WoimSegment
    {

        //## Constructor
        public WoimBodySegment(Random randomsource, Vector3D position, Vector3D velocity, Vector3D acceleration) : base(randomsource, position, velocity, acceleration)
        {

        }        


        //## Public interface
        /// <summary>
        /// Progresses woim segment one time step forward.
        /// </summary>
        public override void Update()
        {
  
            //Update boid state ( I.e. position, velocity, acceleration etc )
            UpdateAccelerationVector(TrackingTarget);                                         // Boid accelerate towards the boid it follows

            Velocity = Velocity + Acceleration * DeltaTime;                                     // Update boid velocity
            Velocity.ClipComponentsToLimits(Boid3DLimits.VelocityMin, Boid3DLimits.VelocityMax);    // Ensure the velocity-value does not grow too large ( or else some boids may shoot off to far off the screen )

            Position = Position + Velocity * DeltaTime;                                         // Update boid position
            Position.ClipComponentsToLimits(Boid3DLimits.PositionsMin, Boid3DLimits.PositionsMax);  // Ensure position does not exceed screen values    
        }

        //## Public properties
        public WoimSegment TrackingTarget { get; set; }     // The segment it should track


        //## Private helpers
        private void UpdateAccelerationVector(WoimSegment target)
        {

            double distanceToTarget = target.GetDistanceFrom(this);                        // The distance between boid and current target

            // Note!! The arithmetic operations for Vector3D are overloaded to
            // perform a operations componentwise.
            // e.g.: a / b = [ a.x/b.x , a.Y/b.Y )

            //Calculate new values for acceleration
            Vector3D deltapos = target.Position - Position;
            Vector3D accelerationTarget = AccelerationFactor * distanceToTarget * deltapos;

            //Ensure we don't end up with values that are too large
            accelerationTarget.ClipComponentsToLimits(Boid3DLimits.AccelerationMin, Boid3DLimits.AccelerationMax);

            // Weight the change ( to smoothen it a bit )
            // Using formula: new value = 2/3*new value + 1/3*oldvalue
            Acceleration = 0.3333 * (2.0 * accelerationTarget + 1.0 * Acceleration);
        }


    }



}
