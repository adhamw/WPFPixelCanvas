using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPixelCanvas.Canvas.Models.Boids3D
{
    // Class that sets up a boids simulation and
    // exposes positional data for plotting to screen

    // The system consists of a set of Boid objects.
    // Each object has position, velocity and acceleration attributes allowing
    // it to move around and accelerate/decelerate.
    //
    // A Boid object can be in either of two states: Leader or follower.
    // A leader-boid will pick it's target destinations by random. ( It will 
    // roam around the screen in a random fashion ). There can be multiple
    // leader-boids in a simulation.

    // A non-leader-boid, is aware of the leader-boids, and will track whichever
    // leaderboid is nearest.
    //
    // Every boid has a color. Non-leader-boids will adjust their color so it is
    // closer to the color of the nearest leader-boid.
    //
    // The BoidSimulator is not optimized for speed, but still runs ok with 4-8k boids
    // on a mid-end computer. Enough to play around with.


    public class BoidSimulator3D
    {
        //## Private fields
        private Random _randomSource { get; set; }
        private Boid3D[] _boids { get; set; }
        private int _numberOfBoids;


        //## Constructor(s)
        public BoidSimulator3D(int screenWidth, int screenHeight)
        {
            //Init private fields
            _randomSource = new Random();

            // Define limits
            Boid3DLimits.AccelerationMin = new Vector3D(-2.7, -2.7,-0.2);
            Boid3DLimits.AccelerationMax = new Vector3D(2.7, 2.7,0.2);
            Boid3DLimits.VelocityMin = new Vector3D(-20.1, -20.1,-1.7);
            Boid3DLimits.VelocityMax = new Vector3D(20.1, 20.1,1.7);
            Boid3DLimits.PositionsMin = new Vector3D(0.0, 0.0,0.1);
            Boid3DLimits.PositionsMax = new Vector3D(screenWidth - 1, screenHeight - 1,12);

            // Set default values for other parameters
            LeaderBoidProbability = 1;
            DeltaTime = 0.05;
            AccelerationFactor = 0.01;
            NumberOfBoids = 8000;       // Note: setting the number of boids, rebuilds the boid list ( so all parameters must be set first )
            BoidRandomness = 0.1;       // Adds random values to parameters at +-10% of its real value
        }


        /// <summary>
        /// Creates a list of boids and assigns leadership
        /// </summary>
        /// <param name="boidcount"></param>
        private void BuildBoids(int boidcount)
        {
            // Create the boids
            _boids = new Boid3D[NumberOfBoids];           // Creates array with space for all boids
            byte[] colorvalues = new byte[3];           // Reserves three bytes for r,g,b values

            // Create individual boids
            for (int i = 0; i < NumberOfBoids; i++)
            {
                // Determine the boid color values 
                _randomSource.NextBytes(colorvalues);

                // Create and add boid to the swarm
                double randomness_DeltaTime = 1 + BoidRandomness * (1 - 2 * _randomSource.NextDouble());
                double randomness_AccelerationFactor = 1 + BoidRandomness * (1 - 2 * _randomSource.NextDouble());

                _boids[i] = new Boid3D(_randomSource, i + 1)
                {
                    AccelerationFactor = this.AccelerationFactor * randomness_AccelerationFactor,
                    DeltaTime = this.DeltaTime * randomness_DeltaTime,
                    Color = new Tuple<byte, byte, byte>(colorvalues[0], colorvalues[1], colorvalues[2])
                };
            }

            // Set up leader boids. 
            List<Boid3D> leaderboids = new List<Boid3D>();
            foreach (var boid in _boids)
            {
                bool isLeaderBoid = _randomSource.Next(1000) < LeaderBoidProbability;
                boid.IsLeaderBoid = isLeaderBoid;  // Determine if leaderboid or not
                if (boid.IsLeaderBoid) { leaderboids.Add(boid); }                 // If leader boid, add to leader boid list
            }

            // Let all the other boids know about the leader boids
            foreach (var boid in _boids)
            {
                if (!boid.IsLeaderBoid) { boid.SetLeaderBoids(leaderboids.ToArray()); }
            }
        }


        //## Public interface

        /// <summary>
        /// Moves all boids forward one time step
        /// </summary>
        public void Update() { foreach (var boid in _boids) { boid.Update(); } }


        //## Public properties
        public int NumberOfBoids
        {
            get { return _numberOfBoids; }
            set { _numberOfBoids = value; BuildBoids(_numberOfBoids); }
        }
        public Boid3D[] Boids { get { return _boids; } }                              // All our boids
        public double DeltaTime { get; set; }                                       // The time step
        public double AccelerationFactor { get; set; }                              // Effect of acceleration
        public int LeaderBoidProbability { get; set; }                              // Per-mille (1/10000) Chance of a boid turning into a leaderboid 
        public double BoidRandomness { get; set; }                                  // Adds random offset to boid parameters. Good values: [0..0.5]

    }
}
