using global::WPFPixelCanvas.Canvas.Models.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFPixelCanvas.Canvas.Models.Bobs;
using WPFPixelCanvas.Canvas.Models.Boids;
using WPFPixelCanvas.Canvas.Models.Boids3D;
using WPFPixelCanvas.Canvas.Models.Moire;
using WPFPixelCanvas.Canvas.Models.PathGenerator;

namespace WPFPixelCanvas.Canvas.Models.ASelectionOfPlotModules
{
    public class Pattern_Bobs : ICanvasPlotter
    {
        //Local fields
        private byte[] _buffer { get; set; }
        private SpriteManager _spriteManager { get; set; }
        private Random _randomSource { get; set; }
        
        private PathGeneratorSource _pathGeneratorFP1;
        private PathGeneratorSource _pathGeneratorFP2;

        private BobsManager _bobsManager;

        //Constructor
        public Pattern_Bobs(int width, int height)
        {
            Width = width;                  // Defines width of plot area ( e.g. 800 pixels )
            Height = height;                // Defines height of plot area ( e.g. 600 pixels )
            _randomSource = new Random();   // Source for random data
        }

        //Public properties
        public int Width { get; private set; }
        public int Height { get; private set; }

        //Public interface
        public byte[] Plot(int bytesPerPixel, int bytesperline, long refreshcounter = 0)
        {
            //Initialize buffer and generator if not set
            if (_buffer == null)
            {
                // Set up buffer
                int bytesinbuffer = bytesperline * Height;  // Number of bytes needed for the whole buffer
                _buffer = new byte[bytesinbuffer];
                int linePadding = bytesperline - Width * bytesPerPixel;
                for (int i = 0; i < bytesinbuffer; i++) { _buffer[i] = 255; }


                //Define generators
                _pathGeneratorFP1 = new(new(0, 0, 0.1), new(500, 400, 1), 0.03);
                _pathGeneratorFP1.SetPathType(PathTypes.Lissajous3D);

                _spriteManager = new SpriteManager(_buffer, Width, Height, bytesPerPixel, new Tuple<byte, byte, byte>(250, 180, 250));
                _spriteManager.AddSprite().loadImage(@".\SpriteData.\glassball2_128x128.bmp", bytesPerPixel);


                _bobsManager = new BobsManager(100,_buffer,Width,Height,bytesPerPixel,linePadding,_spriteManager);
            }

            //Use path points as focal points ( so we get some movement )
            Vector3D fp1 = _pathGeneratorFP1.GetNextPoint();


            // Add '1' to every pixel on screen untill max value reached.
            // This will have the effect of fading the screen to white.
            // As boids are redrawn to the screen at full color, this creates
            // boid trails.
            int pos = 0;

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    int bufferval = _buffer[pos + 0];
                    _buffer[pos + 0] = _buffer[pos + 0] < 254 ? (byte)(_buffer[pos + 0] + 2) : (byte)255;
                    _buffer[pos + 1] = _buffer[pos + 1] < 254 ? (byte)(_buffer[pos + 1] + 2) : (byte)255;
                    _buffer[pos + 2] = _buffer[pos + 2] < 254 ? (byte)(_buffer[pos + 2] + 2) : (byte)255;
                    _buffer[pos + 3] = 255; // _buffer[pos + 2] < 255 ? (byte)(_buffer[pos + 2] + 1) : (byte)255;
                    pos += bytesPerPixel;
                }
            }

            //Update our bobs-object
            _bobsManager.Update(fp1);
            

            //Return buffer ( reference value )
            return _buffer;
        }
    }
}



