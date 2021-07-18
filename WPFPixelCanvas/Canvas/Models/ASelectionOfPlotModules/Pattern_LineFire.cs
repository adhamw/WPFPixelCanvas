/*
 *  This uses the fire effect ( from Pattern_BasicFire ) 
 *  and draws a rotating line on top of it.
 *  
 *  As the fire effect "averages" pixels upwards 
 *  The line pattern will fade away upwards with the "fire"
 *  
 * */



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFPixelCanvas.Canvas.Models.Boids;
using WPFPixelCanvas.Canvas.Models.Shapes;

namespace WPFPixelCanvas.Canvas.Models
{
    public class Pattern_LineFire : ICanvasPlotter
    {
        //## Local fields
        private List<double[]> _dataBuffers { get; set; }
        private int _dataBufferIndex { get; set; }
        private List<byte[]> _pixelBuffers { get; set; }
        private int _pixelBufferIndex { get; set; }
        private Random _randomSource { get; set; }
        private DataLineBuilder _lineDrawer;           // Fills in a value along a line in a data-buffer 


        //## Constructor(s)
        public Pattern_LineFire(int width, int height)
        {
            Width = width;                  // Defines width of plot area ( e.g. 800 pixels )
            Height = height;                // Defines height of plot area ( e.g. 600 pixels )

            _randomSource = new Random();   // Creates  source for random number data
        }


        //## Public properties
        public int Width { get; private set; }
        public int Height { get; private set; }


        //## Public interface
        public byte[] Plot(int bytesPerPixel, int bytesPerLine, long refreshCounter = 0)
        {

            //Initialize buffer if not set
            if (_dataBuffers == null) InitializeDataBuffers(Width, Height, 2);
            if (_pixelBuffers == null)
            {
                InitializePixelBuffers(bytesPerLine, bytesPerPixel, Height, 3);
                _lineDrawer = new DataLineBuilder(_randomSource, Width, Height);
            }

            //  Reference our data buffer
            //  Note: for the basic fire effect, we would strictly 
            //  use only one databuffer. Swapping between two buffers
            //  allows us greater flexibility in meddling with the data
            var sourceData = _dataBuffers[_dataBufferIndex % 2]; // %2(mod operation) rolls index around once 
            var destData = _dataBuffers[_dataBufferIndex++ % 2]; // we reach the end of our buffer-array

            //## Rotate line 
            DrawRotatingLine(sourceData, refreshCounter);
            CalculateFireEffect(sourceData,destData);

            // The detData now contains decimal values that we can translate
            // into color data and fill in pixel buffer

            // We do this by defining a base color, that will be modified by
            // the value in the data buffer.
            
            // The base color
            byte basecolor_R = 255;
            byte basecolor_G = 130; 
            byte basecolor_B = 20; 

            // We Reference our pixel buffer
            var currentbuffer = _pixelBuffers[_pixelBufferIndex];

            // We run through every point in databuffer/screen
            int pos = 0;
            int screenpos = 0;
            for (int i = 0; i < Height; i++) 
            {
                for (int j = 0; j < Width; j++)
                {
                    // Calculate final color
                    byte r = (byte)(basecolor_R * destData[pos]); // The red color contribution
                    byte g = (byte)(basecolor_G * destData[pos]); // The green color contribution
                    byte b = (byte)(basecolor_B);                 // The blue color contribution

                    // Set color data in pixel buffer
                    currentbuffer[screenpos + 0] = g;
                    currentbuffer[screenpos + 1] = b;
                    currentbuffer[screenpos + 2] = r;
                    currentbuffer[screenpos + 3] = 255;

                    // Move pointers forward
                    pos++;                      // Every data point in the data buffer is one 'double'
                    screenpos += bytesPerPixel; // Every pixel/color value in the pixelbuffer is X bytes long. ( E.g. rgba = 4 bytes )
                }
            }

            //Swap data buffers
            _dataBufferIndex++;                 

            // Return buffer by reference
            return currentbuffer;
        }

        //## Private helpers
        private void CalculateFireEffect(double[] sourceData, double[] destData)
        {
            //Average values starting at the top
            int pos = 0;
            int onelineoffset = Width;
            int twolineoffset = 2 * Width;

            for (int i = 0; i < Height - 2; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    double sllp = sourceData[(pos + onelineoffset - 1)];      // Second line, left pixel
                    double slcp = sourceData[(pos + onelineoffset)];          //  Second line, center pixel
                    double slrp = sourceData[(pos + onelineoffset + 1)];      // Second line, right pixel
                    double tlcp = sourceData[(pos + twolineoffset)];          // Third line, center pixel
                                                                              //                    destData[pos++] = (sllp + slcp + slrp + tlcp) / 4.1;    // Average values
                    destData[pos] = (sllp + slcp + slrp + tlcp) / 4.13;
                    pos++;
                }
            }
        }

        /// <summary>
        /// Draws a single line that rotates with a Sin/Cos pattern
        /// </summary>
        /// <param name="sourceData"></param>
        /// <param name="refreshCounter"></param>
        private void DrawRotatingLine(double[] sourceData, long refreshCounter)
        {
            // Determine centre coordinates and radius
            int centreX = Width / 2;
            int centreY = Height / 2;
            int halflinewidth = (int)(0.3 * Width);
            int halflineheight = (int)(0.3 * Height);
            int radius = halflineheight > halflinewidth ? halflinewidth : halflineheight;

            double angle = (refreshCounter) * 3.14 / 180.0;
            double radiusx = radius * (1.55 - 0.3 * Math.Sin(4.1 * angle));
            double radiusy = radius * (0.35 - 0.6 * Math.Cos(2.1 * angle));

            //Rotate line around the centre
            double offsetX = (radiusx * Math.Sin(2.3 * angle));
            double offsetY = (radiusy * Math.Cos(0.6 * angle));

            int x1 = centreX + (int)offsetX;
            int y1 = centreY + (int)offsetY;
            int x2 = centreX - (int)offsetX;
            int y2 = centreX - (int)offsetY;
            _lineDrawer.DrawLine(sourceData, x1, y1, x2, y2, 3.5);

        }
        /// <summary>
        /// Declares a set of data buffer. Each data buffer is the same
        /// size as the pixelbuffer, but holds double values instead of
        /// bytes.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="numberOfBuffers"></param>
        /// <param name="startvalue"></param>
        private void InitializeDataBuffers(int width, int height, int numberOfBuffers, double startvalue = 0.0)
        {
            _dataBuffers = new List<double[]>();

            int size = width * height;
            for (int bufferArrayIndex = 0; bufferArrayIndex < numberOfBuffers; bufferArrayIndex++)
            {
                double[] databuffer = new double[size];
                for (int i = 0; i < size; i++) { databuffer[i] = startvalue; }
                _dataBuffers.Add(databuffer);
            }
            _dataBufferIndex = 0;
        }
        /// <summary>
        /// Declares a set of pixel buffers. Having more than one
        /// allows a sort of double/triple buffering mechanism
        /// </summary>
        /// <param name="bytesPerLine"></param>
        /// <param name="bytesperpixel"></param>
        /// <param name="height"></param>
        /// <param name="numberOfBuffers"></param>
        private void InitializePixelBuffers(int bytesPerLine, int bytesperpixel, int height, int numberOfBuffers)
        {
            _pixelBuffers = new List<byte[]>();

            int size = bytesPerLine * height;
            int pos = 0;
            for (int bufferArrayIndex = 0; bufferArrayIndex < numberOfBuffers; bufferArrayIndex++)
            {
                byte[] buffer = new byte[size];
                for (int i = 0; i < (size - bytesperpixel); i++)
                {
                    buffer[i + 0] = 0;
                    buffer[i + 1] = 0;
                    buffer[i + 2] = 0;
                    buffer[i + 3] = 255;
                    pos += bytesperpixel;
                }

                _pixelBuffers.Add(buffer);
            }

            _pixelBufferIndex = 0; // Point to the first buffer

        }

    }
}



