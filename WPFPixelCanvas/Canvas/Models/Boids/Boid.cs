using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPixelCanvas.Canvas.Models.Boids
{
    //Defines a Boid object. It can move, it can follow other boids.
    public class Boid
    {
        //Private fields
        private Boid _leaderBoid;
        private Random _randomSource { get; set; }
        private Vector3D _positionLimitsMin { get; set; }
        private Vector3D _positionLimitsMax { get; set; }
        private Vector3D _accelerationLimitsMin { get; set; }
        private Vector3D _accelerationLimitsMax { get; set; }
        private double _deltaTime;
        private double _accelerationFactor { get; set; }

        //Constructor(s)
        public Boid(Vector3D positionLimitsMin, Vector3D positionLimitsMax, Vector3D accelerationLimitsMin, Vector3D accelerationLimitsMax, double accelerationfactor, double deltaTime)
        {
            Vector3D startPos = GetRandomPosition(); // No start position given, choosing a random one
            init(positionLimitsMin, positionLimitsMax, accelerationLimitsMin, accelerationLimitsMax, accelerationfactor, deltaTime, startPos);
        }
        public Boid(Vector3D positionLimitsMin, Vector3D positionLimitsMax, Vector3D accelerationLimitsMin, Vector3D accelerationLimitsMax, double accelerationfactor, double deltaTime, Vector3D startPos)
        {
            init(positionLimitsMin, positionLimitsMax, accelerationLimitsMin, accelerationLimitsMax, accelerationfactor, deltaTime, startPos);
        }
        private void init(Vector3D positionLimitsMin, Vector3D positionLimitsMax, Vector3D accelerationLimitsMin, Vector3D accelerationLimitsMax, double accelerationfactor, double deltaTime, Vector3D startPos)
        {
            //Initialize private fields
            _randomSource = new Random();           // Feeds us random numbers
            _positionLimitsMin = positionLimitsMin; // Defines the boids world that
            _positionLimitsMax = positionLimitsMax; // boids movement
            _accelerationLimitsMax = accelerationLimitsMax; // How quickly can boid change its movement
            _accelerationLimitsMin = accelerationLimitsMin; // How quickly can boid change its movement
            _accelerationFactor = accelerationfactor; // Affects how tight relationship between distance and acceleration is
            _deltaTime = deltaTime;                 // Timestep
            _leaderBoid = this;                     // If no other boids to follow

            //Initialize public properties
            Position = startPos;                    // Boid start at this position
            TargetPosition = GetRandomPosition();   // Boid want to move to some random position ( within our world  )

        }


        //Public interface
        public void update()
        {
            // Determine where Boid should move
            AssignLeaderBoid();                             // Determine which of the leaderboids to follow
            if (_leaderBoid.Id != this.Id)                  // Determine new target coordinates 
            { TargetPosition = _leaderBoid.Position; }      //   .. Follow the closest of leaderboids
            else { TargetPosition = GetRandomPosition(); }  //   ..  No boid to follow, select target randomly

            //Update movement parameters
            UpdateAccelerationVector(); // Boid accelerate towards the boid it follows
            Velocity = Velocity + Acceleration * _deltaTime;    // New boid velocity
            Position = Position + Velocity * _deltaTime;        // New boid position
        }

        //Public interface
        public Vector3D GetRandomPosition()
        {
            double x = _positionLimitsMin.X + _randomSource.NextDouble() * (_positionLimitsMax.X - _positionLimitsMin.X);
            double y = _positionLimitsMin.Y + _randomSource.NextDouble() * (_positionLimitsMax.Y - _positionLimitsMin.Y);
            double z = _positionLimitsMin.Z + _randomSource.NextDouble() * (_positionLimitsMax.Z - _positionLimitsMin.Z);

            return new Vector3D(x, y, z);
        }
        public double GetDistanceFrom(Boid other) { return other.Position.GetDistanceFrom(this.Position);}
        public double GetSquaredDistanceFrom(Boid other) { return other.Position.GetDistanceSquaredFrom(other.Position);}

        //Public properties       
        public Vector3D Position { get; set; }
        public Vector3D TargetPosition { get; set; }
        public Vector3D Velocity{ get; set; }
        public Vector3D Acceleration{ get; set; }
        public double LeaderType { get; set; } // [0..1]. 1=Perfect leader, 0=Perfect follower
        public Boid[] Leaders { get; set; }  // This boid will follow the closes of these boids

        public int Id { get; set; } //Identifies this boid uniquely

        //Private helpers
        //Update acceleration vector
        private void AssignLeaderBoid()
        {
            _leaderBoid = this;
            double leaderBoidDistance = double.MaxValue;
            foreach (Boid leader in Leaders)
            {
                double distance = leader.GetDistanceFrom(this);  // Get the distance to other boid
                if (distance < leaderBoidDistance)
                {
                    _leaderBoid = leader;               // Updating which boid to follow
                    leaderBoidDistance = distance;      // The distance to the closes leader boid ( in the moment ) 
                }
            }
        }
        private void UpdateAccelerationVector()
        {
            // Note!! The arithmetic operations for Vector3D are overloaded to
            // performa operations componentwise.
            // e.g.: a / b = [ a.x/b.x , a.Y/b.Y ,  a.Z/b.Z)

            //Calculate new values for acceleration
            Vector3D distanceToTarget = TargetPosition - Position;
            Vector3D accelerationTarget = _accelerationFactor * distanceToTarget;

            //Ensure we don't end up with values that are too large
            accelerationTarget.ClipComponentsToLimits(_accelerationLimitsMin, _accelerationLimitsMax);

            // Weight the change ( to smoothen it a bit )
            // Using formulat: new value = 3/4*new value + 1/4*oldvalue
            Acceleration = 0.25 * (3.0 * accelerationTarget + 1 * Acceleration);
        }



    }
}
