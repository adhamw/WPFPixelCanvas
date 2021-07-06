using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPixelCanvas.Canvas.Models.Boids
{

    // Class that sets up a boids simulation and
    // exposes data for plotting to screen
    public class BoidSimulator
    {
        //Private fields
        private int _numberOfBoids { get; set; }
        private Boid[] _boids { get; set; }

        //Constructor
        public BoidSimulator(int numberOfBoids)
        {

            //Define limits
            Vector3D accelerationLimits_Max = new Vector3D(1.0, 1.0, 1.0);
            Vector3D accelerationLimits_Min = new Vector3D(-1.0, -1.0, -1.0);
            Vector3D positionLimits_Min = new Vector3D(-100.0, -100.0, -100.0);
            Vector3D positionLimits_Max = new Vector3D(100.0, 100.0, 100.0);

            //Set parameters
            double deltaTime = 0.5;
            double accelerationfactor = 0.4; 

            //Create the boids
            _numberOfBoids = numberOfBoids;
            _boids = new Boid[numberOfBoids];
            for(int i=0;i<numberOfBoids;i++) { _boids[i] = new Boid(positionLimits_Min, positionLimits_Max, accelerationLimits_Min, accelerationLimits_Max, accelerationfactor, deltaTime);}

            //Run through all boids and dedicate leader boids. Without it it will just be a mess of
            //boids flying around randomly
            // ...
        }

        //Public interface

        //Moves all boids forward one time step
        public void update()
        {
            foreach(var boid in _boids) { boid.update();}
        }

        //Public properties
        public Boid[] Boids { get { return _boids; }

    }
}
