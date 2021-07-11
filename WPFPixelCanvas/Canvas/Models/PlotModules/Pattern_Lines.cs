/*
 *  Drawing lines, using a basic bresenham algorithm to draw lines.
 *  No major optimizations implemented.
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
    public class Pattern_Lines : ICanvasPlotter
    {
        //## Local fields
        private List<byte[]> _pixelBuffers { get; set; }    // Allows working with multiple pixel buffers
        private int _pixelBufferIndex { get; set; }         // Tracks the current pixel buffer in use
        private Random _randomSource { get; set; }          // Common source for random data
        private PixelLineBuilder _lineDrawer;                    // Component that offers line drawing functionality


        //## Constructor(s)
        public Pattern_Lines(int width, int height)
        {
            Width = width;                  // Defines width of plot area ( e.g. 800 pixels )
            Height = height;                // Defines height of plot area ( e.g. 600 pixels )

            _randomSource = new Random();   // Creates a random data source
        }

        //## Public properties
        public int Width { get; private set; }
        public int Height { get; private set; }


        //## Public interface
        public byte[] Plot(int bytesPerPixel, int bytesPerLine, long refreshCounter = 0)
        {

            //Initialize buffer if not set
            if (_pixelBuffers == null)
            {
                InitializePixelBuffers(bytesPerLine, bytesPerPixel, Height, 1); // Creates a single buffer
                _lineDrawer = new PixelLineBuilder(Width, Height, bytesPerPixel, bytesPerLine);
            }

            //Get a reference to our pixel buffer
            byte[] currentBuffer = _pixelBuffers[0];

            // Identify the center of the screen and extents of lines
            int centerX = Width / 2, centerY = Height / 2;
            int dx = (int)(0.9*centerX), dy = (int)(0.9*centerY);
            int xmax = centerX + dx, ymax = centerY + dy;
            int xmin = centerX - dx, ymin = centerY - dy;

            ////Draw lines in Q1 ( lines go up and right )
            for (int i = 0; i < dx; i += 4) { DrawLine(i, currentBuffer, centerX, centerY, centerX + i, ymax); }
            for (int i = 0; i < dy; i += 4) { DrawLine(i, currentBuffer, centerX, centerY, xmax, centerY + i); }

            ////Draw lines in Q2 ( lines go up and left ) 
            for (int i = 0; i < dx; i += 4) { DrawLine(i, currentBuffer, centerX, centerY, centerX - i, ymax); }
            for (int i = 0; i < dy; i += 4) { DrawLine(i, currentBuffer, centerX, centerY, xmin, centerY + i); }

            ////Draw lines in Q3 ( lines go down and left )
            for (int i = 0; i < dx; i += 4) { DrawLine(i, currentBuffer, centerX, centerY, centerX - i, ymin); }
            for (int i = 0; i < dy; i += 4) { DrawLine(i, currentBuffer, centerX, centerY, xmin, centerY - i); }

            //Draw lines in Q4 ( lines go down and right)
            for (int i = 0; i < dx; i += 4) { DrawLine(i, currentBuffer, centerX, centerY, centerX + i, ymin); }
            for (int i = 0; i < dy; i += 4) { DrawLine(i, currentBuffer, centerX, centerY, xmax, centerY - i); }

            // Return buffer by reference
            return currentBuffer;
        }

        //## Private helpers
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
        private void DrawLine(int i, byte[] buffer, int x1, int y1,int x2, int y2)
        {
            byte colorR = (byte)(i % 255);
            byte colorG = (byte)(2 * i % 255);
            byte colorB = (byte)(3 * i % 255);
            _lineDrawer.DrawLine(buffer, x1, y1, x2, y2, colorR, colorG, colorB, 255);
        }

    }
}



