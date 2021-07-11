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
        public byte[] Plot(int bytesPerPixel, int bytesPerLine, long refreshCounter = 0)
        {
            //Initialize buffers if not set
            if (_dataBuffers == null) InitializeDataBuffers(Width, Height, 2);
            if (_pixelBuffers == null) { InitializePixelBuffers(bytesPerLine, bytesPerPixel, Height, 3);}

            //Reference our data buffers
            var sourceData = _dataBuffers[_dataBufferIndex % 2]; // %2(mod operation) rolls index around once 
            var destData = _dataBuffers[_dataBufferIndex++ % 2]; // we reach the end of our buffer-array

            // Do the fire effect calculations
            // This fills the destData buffer with decimal values we can
            // translate into color data 
            CalculateFireEffect(sourceData,destData);


            // Get reference to our pixel buffer
            byte[] pixelBuffer = _pixelBuffers[_pixelBufferIndex];

            // We use the data in destData buffer to calculate colors
            // and fill in the pixel buffer
            FillInColors(destData, pixelBuffer, bytesPerPixel);

            //Swap data buffers
            _dataBufferIndex++; //

            // Return pixel buffer by reference
            return pixelBuffer;
        }


        //## Private helpers
        private void FillInColors(double[] dataBuffer, byte[] pixelBuffer, int bytesPerPixel)
        {
            // Define base color
            byte basecolor_R = 255;
            byte basecolor_G = 130;
            byte basecolor_B = 20;

            int pos = 0;
            int screenpos = 0;
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    // Calculate final color
                    byte r = (byte)(basecolor_R * dataBuffer[pos]);   // The red color contribution
                    byte g = (byte)(basecolor_G * dataBuffer[pos]);   // The green color contribution
                    byte b = (byte)(basecolor_B);                  // The blue color contribution

                    // Set color data in pixel buffer
                    pixelBuffer[screenpos + 0] = g;
                    pixelBuffer[screenpos + 1] = b;
                    pixelBuffer[screenpos + 2] = r;
                    pixelBuffer[screenpos + 3] = 255;

                    pos++;
                    screenpos += bytesPerPixel;
                }
            }
        }
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
        private void CalculateFireEffect(double[] sourceData, double[] destData)
        {
            //Fill in bottom 3 lines with random values            
            int pos = sourceData.Length - 1; //Starting at end of buffer

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < Width; j++)
                { sourceData[pos--] = _randomSource.NextDouble(); }
            }

            //Average values starting at the top
            pos = 0;
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
                    destData[pos] = (sllp + slcp + slrp + tlcp) / 4.03;
                    pos++;
                }
            }
        }

    }
}



