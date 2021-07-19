using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFPixelCanvas.Canvas.Models.Boids;
using WPFPixelCanvas.Canvas.Models.Boids3D;

namespace WPFPixelCanvas.Canvas.Models.Moire
{
    public class MoirePatternGenerator
    {
        //## Private fields
        private int Width { get; set; }
        private int Height { get; set; }
        private Vector3D _colorA { get; set; }
        private Vector3D _colorB { get; set; }
        private int _ringSize { get; set; }
        byte[] _pixelBuffer { get; set; }
        int _bytesPerPixel { get; set; }
        int _linePaddingBytes { get; set; }
        
        //## Constructor(s)
        public MoirePatternGenerator(byte[] pixelBuffer, int width, int height, int ringSize, int bytesPerPixel, int linePaddingBytes)
        {
            //Storing parameters
            Width = width;
            Height = height;
            _ringSize = ringSize;
            _bytesPerPixel = bytesPerPixel;
            _linePaddingBytes = linePaddingBytes;
            _pixelBuffer = pixelBuffer;

            //Setting default values
            _colorA = new Vector3D(0, 0, 0);
            _colorB = new Vector3D(255.0, 255.0, 255.0);
        }

        //## Public interface
        public void Update(params Vector3D[] focalPoints)
        {
            //Assume at least two points in list
            if (focalPoints.Length < 2) { throw new ArgumentException("Must have at least two focal points"); }

            //Keep track of buffer pointer
            int bufferPos = 0;

            // Go through every x and y
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    //Calculate the distance to the first focal point
                    Vector3D point = focalPoints[0];
                    int fpx = (int)point.X;
                    int fpy = (int)point.Y;

                    int distX = fpx - x;
                    int distY = fpy - y;
                    int value = (int) Math.Sqrt(distX * distX + distY * distY);

                    // Xor the distances to the remaining focal points
                    for ( int i = 1; i < focalPoints.Length; i++)
                    {
                        distX = (int)focalPoints[i].X - x;
                        distY = (int)focalPoints[i].Y - y;
                        int newDistance = (int)Math.Sqrt(distX * distX + distY * distY);
                        newDistance = (int)(newDistance / focalPoints[i].Z);
                        value ^= newDistance; // Bitwise xor
                    }
                    // Adjust values to ring size
                    value = value / _ringSize;

                    // Determine the associated color
                    Vector3D color = (value % 2) == 0 ? _colorA : _colorB;

                    ////Set the color values in buffer                   
                    //_pixelBuffer[bufferPos + 0] = (byte)color.X;
                    //_pixelBuffer[bufferPos + 1] = (byte)color.Y;
                    //_pixelBuffer[bufferPos + 2] = (byte)color.Z;
                    //_pixelBuffer[bufferPos + 3] = (byte)255;

                    //Set the color values in buffer                   
                    _pixelBuffer[bufferPos + 0] = (byte)(value % 256);
                    _pixelBuffer[bufferPos + 1] = (byte)(2*value % 256);
                    _pixelBuffer[bufferPos + 2] = (byte)(4*value % 256);
                    _pixelBuffer[bufferPos + 3] = (byte)255;

                    // Update the buffer index
                    bufferPos += _bytesPerPixel;
                }

                //Add padding for each line ( in case there is any ) 
                bufferPos += _linePaddingBytes;
            }


        }

        //## Private helpers



    }
}
