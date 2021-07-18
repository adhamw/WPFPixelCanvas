using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFPixelCanvas.Canvas.Models.Boids3D;

namespace WPFPixelCanvas.Canvas.Models.PlotModules
{
    // THE BOIDS 3D
    // Pretty much the same as the Pattern_Boids, except this one takes place in three dimensions.
    // 
    // The only difference is that boids now have three coordinates x,y and z.
    // When plotting the points, we simply divide X and Y by the coordinate Z-value
    // ( and correct by centre point)
    // 

    // Note!: This is not an accurate physical simulation, nor is it meant to be.
    // It is all about the pretty colors/patterns.
    // Mostly a playground for experimenting with the WPFPixelCanvas.

    

    public class Pattern_Boids3D : ICanvasPlotter
    {
        //## Local fields
        private byte[] _buffer { get; set; }
        private BoidSimulator3D _boidsSimulator;


        //## Constructor(s)
        public Pattern_Boids3D(int width, int height)
        {
            Width = width;                  // Defines width of plot area ( e.g. 800 pixels )
            Height = height;                // Defines height of plot area ( e.g. 600 pixels )

            //Set up the boids simulator
            _boidsSimulator = new BoidSimulator3D(width, height)
            {
                DeltaTime = 0.05,          // How fast does the simulation progress. Good values [0.01.. 0.1]
                AccelerationFactor = 0.01, // How fast does a boid change direction. Good values [0.005..0.1]
                LeaderBoidProbability = 2, // How many leaderboids per 1000 boids. Good values: [0..20]
                BoidRandomness = 0.3,
                NumberOfBoids = 5000       // Note!! This rebuilds boid-array, making use of parameters above, so this assignment must come last
            };
        }


        //## Public properties
        public int Width { get; private set; }
        public int Height { get; private set; }


        //## Public interface
        public byte[] Plot(int bytesPerPixel, int bytesPerLine, long refreshCounter = 0)
        {
            int width = Width;                          // Storing width in local variable ( caching for speed ) 
            int height = Height;                        // ditto

            //Initialize buffer if not set
            int bytesInBuffer = bytesPerLine * height;  // Number of bytes needed for the whole buffer
            if (_buffer == null) { _buffer = new byte[bytesInBuffer]; }

            // Add '1' to every pixel on screen untill max value reached.
            // This will have the effect of fading the screen to white.
            // As boids are redrawn to the screen at full color, this creates
            // boid trails.
            int pos = 0;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int bufferval = _buffer[pos + 0];
                    _buffer[pos + 0] = _buffer[pos + 0] < 255 ? (byte)(_buffer[pos + 0] + 1) : (byte)255;
                    _buffer[pos + 1] = _buffer[pos + 1] < 255 ? (byte)(_buffer[pos + 1] + 1) : (byte)255;
                    _buffer[pos + 2] = _buffer[pos + 2] < 255 ? (byte)(_buffer[pos + 2] + 1) : (byte)255;
                    pos += bytesPerPixel;
                }
            }

            // Move simulation forwards
            _boidsSimulator.Update();

            int centerX = width / 2;
            int centerY = height / 2;

            // Plot every boid
            foreach (var boid in _boidsSimulator.Boids)
            {



                //Adjust x,y values for z
                Vector3D boidpos = boid.Position;

                int plotX = (int)(centerX + (boidpos.X - centerX) / (1+0.5*boidpos.Z));
                int plotY = (int)(centerY + (boidpos.Y - centerY) / (1+0.5*boidpos.Z));

                int offset = (int)plotX * bytesPerPixel + (int)plotY * bytesPerLine;

                //Set colorvalues
                double bval = boid.Color.Item1;
                double rval = boid.Color.Item2;
                double gval = boid.Color.Item3;

                // Storing color values in buffer
                _buffer[offset + 0] = (byte)(rval);   //Blue component
                _buffer[offset + 1] = (byte)(gval);   //Green component 
                _buffer[offset + 2] = (byte)(bval);   //Red component 
                _buffer[offset + 3] = 255;            //Alpha component

            }

            // Return buffer ( reference value )
            return _buffer;
        }


    }
}




