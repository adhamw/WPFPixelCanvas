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
        Blob[] _blobs = null;
        Stopwatch _StopWatch;


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
        public byte[] Plot(int bytesPerPixel, int bytesPerLine, long refreshcounter = 0)
        {
            //Initialize buffer and generator if not set
            if (_buffer == null)
            {
                // Set up necessary structures
                int bytesInBuffer = bytesPerLine * Height;  // Number of bytes needed for the whole buffer
                InitBuffers(bytesInBuffer);     // The graphics buffer
                InitPathGenerators();           // The paths the blobs will take
                DefineBlobs();                  // The blob objects

                // Create manager object for blobs
                int linePadding = bytesPerLine - Width * bytesPerPixel;
                _blobManager = new BlobManager(_blobs,_buffer, Width, Height, bytesPerPixel, linePadding);

            }

            //Update our bobs-object
            _blobManager.Update();

            //Update blob positions
            for(int i=0;i<4;i++) { _blobs[i].Position = _pathGenerators[i].GetNextPoint();}

            //Return buffer ( reference value )
            return _buffer;
        }


        //## Private helpers
        private PathGeneratorSource[] _pathGenerators;
        private void InitPathGenerators()
        {
            //Define generators
            _pathGenerators = new PathGeneratorSource[4];
            for(int i=0;i<4;i++) 
            { 
                _pathGenerators[i]  = new(new(0, 0, 0.1), new(500, 400, 1), 0.03); 
                _pathGenerators[i].SetPathType(PathTypes.Lissajous3D);
            }
            //Give each blob different starting points ( along the same path )
            _pathGenerators[1].OffsetAngleBy(0.9);
            _pathGenerators[2].OffsetAngleBy(1.8);
            _pathGenerators[3].OffsetAngleBy(2.4);
        }
        private void InitBuffers(int bytesInBuffer)
        {
            // Set up buffer
            _buffer = new byte[bytesInBuffer];
            for (int i = 0; i < bytesInBuffer; i++) { _buffer[i] = (byte)((i % 4 == 0) ? 255 : 0); }
        }
        private void DefineBlobs()
        {
            //Calculate blob positions
            _blobs = new Blob[4];
            _blobs[0] = new Blob()
            {
                BaseColor = new Vector3D(205, 0, 0),
                Position = _pathGenerators[0].GetNextPoint(),
                Strength = 15.1,
            };
            _blobs[1] = new Blob()
            {
                BaseColor = new Vector3D(0, 255, 0),
                Position = _pathGenerators[1].GetNextPoint(),
                Strength = 1.8,
            };
            _blobs[2] = new Blob()
            {
                BaseColor = new Vector3D(10, 10, 255),
                Position = _pathGenerators[2].GetNextPoint(),
                Strength = 10.8,
            };

            _blobs[3] = new Blob()
            {
                BaseColor = new Vector3D(255, 255, 255),
                Position = _pathGenerators[3].GetNextPoint(),
                Strength = 5.7,
            };

        }

    }
}



