using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFPixelCanvas.Canvas.Models.Boids3D;
using WPFPixelCanvas.Canvas.Models.Sprites;

namespace WPFPixelCanvas.Canvas.Models.ASelectionOfPlotModules
{
    // THE BOIDS 3D
    // Exactly same as Boids 3D, but using sprites
    // 

    public class Pattern_Boids3DSprites : ICanvasPlotter
    {
        //## Local fields
        private byte[] _buffer { get; set; }
        private BoidSimulator3D _boidsSimulator;


        //## Constructor(s)
        public Pattern_Boids3DSprites(int width, int height)
        {
            Width = width;                  // Defines width of plot area ( e.g. 800 pixels )
            Height = height;                // Defines height of plot area ( e.g. 600 pixels )

            //Set up the boids simulator
            _boidsSimulator = new BoidSimulator3D(width, height)
            {
                DeltaTime = 0.1,          // How fast does the simulation progress. Good values [0.01.. 0.1]
                AccelerationFactor = 0.1, // How fast does a boid change direction. Good values [0.005..0.1]
                LeaderBoidProbability = 10, // How many leaderboids per 1000 boids. Good values: [0..20]
                BoidRandomness = 0.8,
                NumberOfBoids = 500       // Note!! This rebuilds boid-array, making use of parameters above, so this assignment must come last
            };
        }


        //## Public properties
        public int Width { get; private set; }
        public int Height { get; private set; }


        //## Public interface
        private SpriteManager _spriteManager { get; set; }
        public byte[] Plot(int bytesPerPixel, int bytesPerLine, long refreshCounter = 0)
        {
            int width = Width;                          // Storing width in local variable ( caching for speed ) 
            int height = Height;                        // ditto

            //Initialize buffer if not set
            int bytesInBuffer = bytesPerLine * height;  // Number of bytes needed for the whole buffer
            if (_buffer == null) 
            { 
                _buffer = new byte[bytesInBuffer];
                _spriteManager = new SpriteManager(_buffer, Width, Height, bytesPerPixel, new Tuple<byte, byte, byte>(250, 180, 250));
                _spriteManager.AddSprite().loadImage(@".\SpriteData.\glassball2_128x128.bmp", bytesPerPixel);


            }

            // Add '1' to every pixel on screen untill max value reached.
            // This will have the effect of fading the screen to white.
            // As boids are redrawn to the screen at full color, this creates
            // boid trails.
            int pos = 0;

            //for (int i = 0; i < height; i++)
            //{
            //    for (int j = 0; j < width; j++)
            //    {
            //        int bufferval = _buffer[pos + 0];
            //        _buffer[pos + 0] = _buffer[pos + 0] < 255 ? (byte)(_buffer[pos + 0] + 1) : (byte)255;
            //        _buffer[pos + 1] = _buffer[pos + 1] < 255 ? (byte)(_buffer[pos + 1] + 1) : (byte)255;
            //        _buffer[pos + 2] = _buffer[pos + 2] < 255 ? (byte)(_buffer[pos + 2] + 1) : (byte)255;
            //        pos += bytesPerPixel;
            //    }
            //}

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int bufferval = _buffer[pos + 0];
                    _buffer[pos + 0] = (byte)255;
                    _buffer[pos + 1] = (byte)255;
                    _buffer[pos + 2] = (byte)255;
                    _buffer[pos + 3] = (byte)255;
                    pos += bytesPerPixel;
                }
            }


            // Move simulation forwards
            _boidsSimulator.Update();

            int centerX = width / 2;
            int centerY = height / 2;

            // Plot every boid
            Sprite sprite = _spriteManager.getSprite(0);
            foreach (var boid in _boidsSimulator.Boids)
            {
                //Adjust x,y values for z
                Vector3D boidpos = boid.Position;

                int plotX = (int)(centerX + (boidpos.X - centerX) / (1 + 0.1 * boidpos.Z));
                int plotY = (int)(centerY + (boidpos.Y - centerY) / (1 + 0.1 * boidpos.Z));

                int offset = (int)plotX * bytesPerPixel + (int)plotY * bytesPerLine;


                double scale = (0.2 / boidpos.Z);
                _spriteManager.DrawSprite(sprite, plotX, plotY, scale);

            }

            // Return buffer ( reference value )
            return _buffer;
        }


    }
}





