using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using WPFPixelCanvas.Canvas.Models.Blobs;
using WPFPixelCanvas.Canvas.Models.Boids3D;
using WPFPixelCanvas.Canvas.Models.PathGenerator;

namespace WPFPixelCanvas.Canvas.Models.ASelectionOfPlotModules
{
    public class Pattern_Blobs : ICanvasPlotter
    {
        //Local fields
        private byte[] _buffer { get; set; }
        private Random _randomSource { get; set; }

        private PathGeneratorSource _pathGeneratorFP1;
        private PathGeneratorSource _pathGeneratorFP2;
        private PathGeneratorSource _pathGeneratorFP3;
        private PathGeneratorSource _pathGeneratorFP4;

        private BlobManager _blobManager;

        //Constructor
        
        public Pattern_Blobs(int width, int height)
        {
            Width = width;                  // Defines width of plot area ( e.g. 800 pixels )
            Height = height;                // Defines height of plot area ( e.g. 600 pixels )
            _randomSource = new Random();   // Source for random data
        }

        //Public properties
        public int Width { get; private set; }
        public int Height { get; private set; }

        //Public interface
        Blob[] _blobs = null;
        Stopwatch sw;
        public byte[] Plot(int bytesPerPixel, int bytesperline, long refreshcounter = 0)
        {
            //Initialize buffer and generator if not set
            int bytesinbuffer = bytesperline * Height;  // Number of bytes needed for the whole buffer

            if (_buffer == null)
            {
                // Set up buffer
                _buffer = new byte[bytesinbuffer];
                int linePadding = bytesperline - Width * bytesPerPixel;
                for (int i = 0; i < bytesinbuffer; i++)
                {

                    _buffer[i] = (byte)((i % 4 == 0) ? 255 : 0);
                }


                //Define generators
                _pathGeneratorFP1 = new(new(0, 0, 0.1), new(500, 400, 1), 0.03);
                _pathGeneratorFP1.SetPathType(PathTypes.Lissajous3D);
                _pathGeneratorFP2 = new(new(0, 0, 0.1), new(500, 400, 1), 0.03);
                _pathGeneratorFP2.SetPathType(PathTypes.Lissajous3D);
                _pathGeneratorFP2.OffsetAngleBy(0.9);
                _pathGeneratorFP3 = new(new(0, 0, 0.1), new(500, 400, 1), 0.03);
                _pathGeneratorFP3.SetPathType(PathTypes.Lissajous3D);
                _pathGeneratorFP3.OffsetAngleBy(1.8);
                _pathGeneratorFP4 = new(new(0, 0, 0.1), new(500, 400, 1), 0.03);
                _pathGeneratorFP4.SetPathType(PathTypes.Lissajous3D);
                _pathGeneratorFP4.OffsetAngleBy(2.4);

                //Calculate blob positions
                _blobs = new Blob[4];
                _blobs[0] = new Blob()
                {
                    BaseColor = new Vector3D(205, 0, 0),
                    Position = _pathGeneratorFP1.GetNextPoint(),
                    Strength = 15.1,
                };
                _blobs[1] = new Blob()
                {
                    BaseColor = new Vector3D(0, 255, 0),
                    Position = _pathGeneratorFP2.GetNextPoint(),
                    Strength = 1.8,
                };
                _blobs[2] = new Blob()
                {
                    BaseColor = new Vector3D(10,10 ,255),
                    Position = _pathGeneratorFP3.GetNextPoint(),
                    Strength = 10.8,
                };

                _blobs[3] = new Blob()
                {
                    BaseColor = new Vector3D(255, 255, 255),
                    Position = _pathGeneratorFP4.GetNextPoint(),
                    Strength = 5.7,
                };

                _blobManager = new BlobManager(_blobs,_buffer, Width, Height, bytesPerPixel, linePadding);

                sw = new Stopwatch();
                
            }



            //// Add '1' to every pixel on screen untill max value reached.
            //// This will have the effect of fading the screen to white.
            //// As boids are redrawn to the screen at full color, this creates
            //// boid trails.
            //int pos = 0;

            //for (int i = 0; i < Height; i++)
            //{
            //    for (int j = 0; j < Width; j++)
            //    {
            //        int bufferval = _buffer[pos + 0];
            //        _buffer[pos + 0] = _buffer[pos + 0] < 254 ? (byte)(_buffer[pos + 0] + 2) : (byte)255;
            //        _buffer[pos + 1] = _buffer[pos + 1] < 254 ? (byte)(_buffer[pos + 1] + 2) : (byte)255;
            //        _buffer[pos + 2] = _buffer[pos + 2] < 254 ? (byte)(_buffer[pos + 2] + 2) : (byte)255;
            //        _buffer[pos + 3] = 255; // _buffer[pos + 2] < 255 ? (byte)(_buffer[pos + 2] + 1) : (byte)255;
            //        pos += bytesPerPixel;
            //    }
            //}

            //Clear screen
            for (int i = 0; i < bytesinbuffer; i++)
            {
                _buffer[i] = (byte)((i % 4 == 0) ? 255 : 0);
            }

            //Update our bobs-object
            //sw.Start();
            _blobManager.Update();
            //if (refreshcounter == 30)
            //{
            //    sw.Stop();
            //    var value = sw.ElapsedMilliseconds;
            //    bool stophere = true;

            //}

            //Update blop-point positions
            _blobs[0].Position = _pathGeneratorFP1.GetNextPoint();
            _blobs[1].Position = _pathGeneratorFP2.GetNextPoint();
            _blobs[2].Position = _pathGeneratorFP3.GetNextPoint();
            _blobs[3].Position = _pathGeneratorFP4.GetNextPoint();

            //Return buffer ( reference value )
            return _buffer;
        }
    }
}



