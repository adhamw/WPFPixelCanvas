/*
 *  The basic fire 

 *    -- generate random values for X lowest rows
 *    -- have a double-buffer that matches the screen-size
 *    -- Generate random values between 0 and 1 for X bottom rows
 *    -- STart at top line and move downwards
 *    -- Calculate the summing of values on current line, previous line, and line before
 *       as whon
 *        Current line:      ......X.......
 *        Previous line:     .....XXX......
 *        2nd Previous line: ......X.......
 *        
 *    -- Convert double to color value and put in screen buffer
 * 
 * */



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFPixelCanvas.Canvas.Models.Boids;

namespace WPFPixelCanvas.Canvas.Models
{
    public class Pattern_BasicFire : ICanvasPlotter
    {
        //## Local fields
        //private byte[] _screenBuffer { get; set; }
        private List<double[]> _dataBuffers { get; set; }
        private List<byte[]> _pixelBuffers { get; set; }
        private int _dataBufferIndex { get; set; }
        private int _pixelBufferIndex { get; set; }
        private Random _randomSource { get; set; }


        //## Constructor(s)
        public Pattern_BasicFire(int width, int height)
        {
            Width = width;                  // Defines width of plot area ( e.g. 800 pixels )
            Height = height;                // Defines height of plot area ( e.g. 600 pixels )

            _randomSource = new Random();
        }

        //## Public properties
        public int Width { get; private set; }
        public int Height { get; private set; }


        //## Public interface
        private void initializeDataBuffers(int width, int height, int numberOfBuffers, double startvalue = 0.0)       
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
        private void initializeColorBuffers(int bytesPerLine, int bytesperpixel, int height, int numberOfBuffers)
        {
            _pixelBuffers = new List<byte[]>();

            int size = bytesPerLine* height;
            int pos = 0;
            for (int bufferArrayIndex = 0; bufferArrayIndex < numberOfBuffers; bufferArrayIndex++)
            {
                byte[] buffer = new byte[size];
                for (int i = 0; i < (size-bytesperpixel); i++) 
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

        public byte[] Plot(int bytesPerPixel, int bytesPerLine, long refreshCounter = 0)
        {
            int width = Width;                          // Storing width in local variable ( caching for speed ) 
            int height = Height;                        // ditto

            //Initialize buffer if not set
            if (_dataBuffers == null) initializeDataBuffers(width, height, 2);
            if (_pixelBuffers == null)
            {
                initializeColorBuffers(bytesPerLine, bytesPerPixel, height, 3);

            }

            //Reference our data buffer
            var sourceData = _dataBuffers[_dataBufferIndex % 2]; // %2(mod operation) rolls index around once 
            var destData = _dataBuffers[_dataBufferIndex++ % 2]; // we reach the end of our buffer-array

            //Fill in bottom 3 lines with random values            
            int pos = sourceData.Length - 1; //Starting at end of buffer

            int higdensitycentre = _randomSource.Next(width);
            
            for (int i = 0; i < 3; i++)
            {
                for(int j = 0; j < width; j++) 
                {
                    double distanceformula = (higdensitycentre - j) * (0.1 + 0.1 * Math.Sin(0.01 * refreshCounter));

                    double distanceToDensityCentre = 100.0 / (1+(int)Math.Abs(distanceformula));
                    int density = (int)distanceToDensityCentre;
                    bool isfire = _randomSource.Next(100) < density;
                    double firevalue = isfire ? 1.0 : _randomSource.NextDouble();
                    sourceData[pos--] = firevalue;
                }
            }

            //Average values starting at the top
            pos = 0;
            int onelineoffset = width;
            int twolineoffset = 2 * width;
            int sourceDataLength = sourceData.Length;

            for(int i = 0; i < height - 2; i++)
            {
                double yoffset = 115 * Math.Cos(0.01*(i+refreshCounter));

                for (int j = 0; j < width; j++)
                {
                    double sllp = sourceData[(pos + onelineoffset - 1 ) ];      // Second line, left pixel
                    double slcp = sourceData[(pos + onelineoffset ) ];          //  Second line, center pixel
                    double slrp = sourceData[(pos + onelineoffset + 1 ) ];      // Second line, right pixel
                    double tlcp = sourceData[(pos + twolineoffset  ) ];          // Third line, center pixel
                                                                            //                    destData[pos++] = (sllp + slcp + slrp + tlcp) / 4.1;    // Average values
                    destData[pos] = ( sllp+slcp+slrp+tlcp )/4.02;
                    pos++;
                }
            }

            // Convert decimal values to color values and fill in pixel buffer


            // Reference our pixel buffer
            byte basecolor_R = 255;
            byte basecolor_G = 130; // (byte)(128.0 - 127.0 * Math.Cos(refreshCounter * 0.005));
            byte basecolor_B = 30; // (byte)(128.0 - 127.0 * Math.Sin(refreshCounter * 0.01));
            var currentbuffer = _pixelBuffers[_pixelBufferIndex];

            pos = 0;
            int screenpos = 0;
            for (int i = 0; i < height - 2; i++)
            {
                for (int j = 0; j < width; j++)
                {

                    byte r = (byte)(basecolor_R * destData[pos]);
                    byte g = (byte)(basecolor_G * destData[pos]);
                    byte b = (byte)(basecolor_B* destData[pos]);

                    currentbuffer[screenpos + 0] = g;
                    currentbuffer[screenpos + 1] = b;
                    currentbuffer[screenpos + 2] = r;
                    currentbuffer[screenpos + 3] = 255;

                    pos++;
                    screenpos += bytesPerPixel;
                }
            }


            
            


            //Swap data buffers
            _dataBufferIndex++; //

            // Return buffer by reference
            return currentbuffer;
        }
    }
}



