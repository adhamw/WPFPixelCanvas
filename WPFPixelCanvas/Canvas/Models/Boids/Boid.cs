using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPixelCanvas.Canvas.Models.Boids
{
    /// <summary>
    ///  Defines a 'boid'. It can move around. 
    ///  It can track/lead other boids. It has its own color
    /// </summary>
    public class Boid
    {
        //## Private fields
        private Random _randomSource { get; set; }


        //## Constructor(s)
        public Boid(Random randomsource, int boidId)
        {
            //Initialize private fields
            _randomSource = randomsource;           // Feeds us random numbers
            Id = boidId;                            // Allows telling this boid apart from other boids. 

            //Default boid parameters
            AccelerationFactor = 0.02;                      // Affects how "fast" boid can turn
            DeltaTime = 0.01;                               // Affects .. everything
            Color = new Tuple<byte, byte, byte>(0, 0, 0);   // Boid color defaults to black 
            Leaders = Array.Empty<Boid>();                  // No other boids to follow by default
            CurrentLeader = null;                           // No current leader
            IsLeaderBoid = true;                            // All boids are born leaderboids ( since they do not have any other boids to follow )
            Position = GetRandomPosition();                 // Boid start at this position
            StartPos = GetRandomPosition();                 // Where should the boid be born ( on-screen )?
            TargetPosition = GetRandomPosition();           // Where should the boid go next?
            Acceleration = GetRandomAcceleration();         // How fast should it go there?
            Velocity = GetRandomVelocity();                 // ..ditto
        }


        //## Public interface

        /// <summary>
        /// Progresses boid one time step forward.
        /// </summary>
        public void Update()
        {
            //Determine where the boid should go next
            IdentifyCurrentTarget();

            //Update boid state ( I.e. position, velocity, acceleration etc )
            double distanceToTarget = TargetPosition.GetDistanceFrom(Position);                        // The distance between boid and current target
            UpdateAccelerationVector(distanceToTarget);                                         // Boid accelerate towards the boid it follows

            Velocity = Velocity + Acceleration * DeltaTime;                                     // Update boid velocity
            Velocity.ClipComponentsToLimits(BoidLimits.VelocityMin, BoidLimits.VelocityMax);    // Ensure the velocity-value does not grow too large ( or else some boids may shoot off to far off the screen )

            Position = Position + Velocity * DeltaTime;                                         // Update boid position
            Position.ClipComponentsToLimits(BoidLimits.PositionsMin, BoidLimits.PositionsMax);  // Ensure position does not exceed screen values    
        }

        /// <summary>
        /// Changes boid to following boid
        /// </summary>
        /// <param name="leaders"></param>
        public void SetLeaderBoids(Boid[] leaders)
        {
            if (leaders.Length == 0) { return; }                        // If no leaders received, do not update
            if (leaders.Length == 1 && leaders[0].Id == Id) { return; } // Cannot follow itself

            IsLeaderBoid = false;                                       // Boid should follow its leaderboids
            Leaders = leaders;                                          // One or more other boids this boid can follow            
        }
        public Vector2D GetRandomAcceleration()
        {
            if (_randomSource == null) { _randomSource = new Random(); }
            double x = BoidLimits.AccelerationMin.X + _randomSource.NextDouble() * (BoidLimits.AccelerationMax.X - BoidLimits.AccelerationMin.X);
            double y = BoidLimits.AccelerationMin.Y + _randomSource.NextDouble() * (BoidLimits.AccelerationMax.Y - BoidLimits.AccelerationMin.Y);
            //double z = BoidLimits.AccelerationMin.Z + _randomSource.NextDouble() * (BoidLimits.AccelerationMax.Z - BoidLimits.AccelerationMin.Z);

            return new Vector2D(x, y);
        }
        public Vector2D GetRandomVelocity()
        {
            if (_randomSource == null) { _randomSource = new Random(); }
            double x = BoidLimits.VelocityMin.X + _randomSource.NextDouble() * (BoidLimits.VelocityMax.X - BoidLimits.VelocityMin.X);
            double y = BoidLimits.VelocityMin.Y + _randomSource.NextDouble() * (BoidLimits.VelocityMax.Y - BoidLimits.VelocityMin.Y);
            //double z = BoidLimits.VelocityMin.Z + _randomSource.NextDouble() * (BoidLimits.VelocityMax.Z - BoidLimits.VelocityMin.Z);

            return new Vector2D(x, y);
        }
        public Vector2D GetRandomPosition()
        {
            if (_randomSource == null) { _randomSource = new Random(); }
            double x = BoidLimits.PositionsMin.X + _randomSource.NextDouble() * (BoidLimits.PositionsMax.X - BoidLimits.PositionsMin.X);
            double y = BoidLimits.PositionsMin.Y + _randomSource.NextDouble() * (BoidLimits.PositionsMax.Y - BoidLimits.PositionsMin.Y);
            //double z = BoidLimits.PositionsMin.Z + _randomSource.NextDouble() * (BoidLimits.PositionsMax.Z - BoidLimits.PositionsMin.Z);

            return new Vector2D(x, y);
        }
        public double GetDistanceFrom(Boid other) { return other.Position.GetDistanceFrom(this.Position); }
        public double GetSquaredDistanceFrom(Boid other) { return other.Position.GetDistanceSquaredFrom(this.Position); }


        //## Public properties       
        public int Id { get; set; }                         // Identifies this boid uniquely
        public bool IsLeaderBoid { get; set; }              // Will this boid lead or follow
        public Vector2D Position { get; set; }              // Where on screen?
        public Vector2D TargetPosition { get; set; }        // Where is it going?
        public Vector2D Velocity { get; set; }              // How fast is it going?
        public Vector2D Acceleration { get; set; }          // How fast can it adjust?
        public Boid[] Leaders { get; private set; }         // Boid will follow the nearest leader
        public Boid CurrentLeader { get; set; }             // Currently tracking this boid
        public Tuple<byte, byte, byte> Color { get; set; }     // Boid color
        public double AccelerationFactor { get; set; }      // Affects impact of acceleration 
        public double DeltaTime { get; set; }               // The timestep. Affects everything.
        public Vector2D StartPos { get; set; }              // Where on screen is the boid born


        //## Private helpers

        /// <summary>
        /// Sets the TargetPosition for the boid
        /// </summary>
        private void IdentifyCurrentTarget()
        {
            double distanceToTarget;

            // Leader boids roam around, picking target locations by random
            if (IsLeaderBoid)
            {
                //Only update with new target, if we are in vicinity of our previous target
                distanceToTarget = TargetPosition.GetDistanceFrom(Position);
                if (distanceToTarget < DeltaTime * 2) { TargetPosition = GetRandomPosition(); } // Leaderboids pick random targets to move to next
            }
            else { FollowNearestLeader(); } // Non-leader-boid, find the nearest leader, and follow it
        }

        /// <summary>
        /// Determines which of the leaderboids to follow
        /// </summary>
        private void FollowNearestLeader()
        {
            //If current leader not set, select the first one
            if (CurrentLeader == null) { CurrentLeader = Leaders[0]; }

            //Use distance to current leader boid as reference

            double mindistance = CurrentLeader.GetSquaredDistanceFrom(this); // dist = sqrt(x^2+y^2) => distA < distB ==> distA^2 < distB^2 ( saves a square root operation )
            bool changedLeaderBoid = false;

            //Pick the leader closest
            foreach (Boid leader in Leaders)
            {
                double distance = leader.GetSquaredDistanceFrom(this);  // Get the distance to other boid
                if (distance < mindistance)
                {
                    CurrentLeader = leader;               // Updating which boid to follow
                    mindistance = distance;      // The distance to the closes leader boid ( in the moment ) 
                    changedLeaderBoid = true;
                }
            }

            if (changedLeaderBoid)
            {
                //Updaating the targetposition
                TargetPosition = CurrentLeader.Position;

                //Adjust the boid color so it matches the leader 
                double adjusted_r = (CurrentLeader.Color.Item1 + Color.Item1) >> 1; // Simply averaging the color of leader and current boid ( brings it closer to the leader boid )
                double adjusted_g = (CurrentLeader.Color.Item2 + Color.Item2) >> 1; // .. ditto
                double adjusted_b = (CurrentLeader.Color.Item3 + Color.Item3) >> 1; // .. ditto
                Color = new Tuple<byte, byte, byte>((byte)adjusted_r, (byte)adjusted_g, (byte)adjusted_b);
            }


        }
        private void UpdateAccelerationVector(double distancetotarget)
        {
            // Note!! The arithmetic operations for Vector2D are overloaded to
            // perform a operations componentwise.
            // e.g.: a / b = [ a.x/b.x , a.Y/b.Y )

            //Calculate new values for acceleration
            Vector2D deltapos = TargetPosition - Position;
            Vector2D accelerationTarget = AccelerationFactor * distancetotarget * deltapos;

            //Ensure we don't end up with values that are too large
            accelerationTarget.ClipComponentsToLimits(BoidLimits.AccelerationMin, BoidLimits.AccelerationMax);

            // Weight the change ( to smoothen it a bit )
            // Using formula: new value = 2/3*new value + 1/3*oldvalue
            Acceleration = 0.3333 * (2.0 * accelerationTarget + 1.0 * Acceleration);
        }



    }
}
