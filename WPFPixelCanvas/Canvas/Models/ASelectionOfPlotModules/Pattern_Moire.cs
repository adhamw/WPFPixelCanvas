using global::WPFPixelCanvas.Canvas.Models.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFPixelCanvas.Canvas.Models.Boids;
using WPFPixelCanvas.Canvas.Models.Boids3D;
using WPFPixelCanvas.Canvas.Models.Moire;
using WPFPixelCanvas.Canvas.Models.PathGenerator;

namespace WPFPixelCanvas.Canvas.Models.ASelectionOfPlotModules
{
    public class Pattern_Moire : ICanvasPlotter
    {
        //Local fields
        private byte[] _buffer { get; set; }
        private SpriteManager _spriteManager { get; set; }
        private int _ringSize { get; set; }
        private Random _randomSource { get; set; }
        private MoirePatternGenerator _moireGenerator;
        private PathGeneratorSource _pathGeneratorFP1;
        private PathGeneratorSource _pathGeneratorFP2;


        //Constructor
        public Pattern_Moire(int width, int height)
        {
            Width = width;      // Defines width of plot area ( e.g. 800 pixels )
            Height = height;    // Defines height of plot area ( e.g. 600 pixels )
            _ringSize = 12;     // Higher values gives less color impact
            _randomSource = new Random(); // Source for random data
        }

        //Public properties
        public int Width { get; private set; }
        public int Height { get; private set; }

        //Public interface
        public byte[] Plot(int bytesperpixel, int bytesperline, long refreshcounter = 0)
        {
            //Initialize buffer and generator if not set
            if (_buffer == null)
            {
                // Set up buffer
                int bytesinbuffer = bytesperline * Height;  // Number of bytes needed for the whole buffer
                _buffer = new byte[bytesinbuffer];
                int linePadding = bytesperline - Width * bytesperpixel;

                //Define generators
                _moireGenerator = new(_buffer, Width, Height, _ringSize, bytesperpixel, linePadding);
                _pathGeneratorFP1 = new(new(-600, -500, 1), new(800, 700, 1), 0.01);
                _pathGeneratorFP2 = new(new(-100, -100, 1), new(600, 500, 1), 0.02);
                _pathGeneratorFP2.SetPathType(PathTypes.Lissajous1);
            }

            //Use path points as focal points ( so we get some movement )
            Vector3D fp1 = _pathGeneratorFP1.GetNextPoint();
            Vector3D fp2 = _pathGeneratorFP2.GetNextPoint();

            // Create moire pattern
            _moireGenerator.Update(fp1, fp2);

            //Return buffer ( reference value )
            return _buffer;
        }
    }
}


