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
        private int _width { get; set; }
        private int _height { get; set; }
        private Vector3D _colorA { get; set; }
        private Vector3D _colorB { get; set; }
        private int _ringSize { get; set; }
        private byte[] _pixelBuffer { get; set; }
        private int _bytesPerPixel { get; set; }
        private int _linePaddingBytes { get; set; }
       
        
        //## Constructor(s)
        public MoirePatternGenerator(byte[] pixelBuffer, int width, int height, int ringSize, int bytesPerPixel, int linePaddingBytes)
        {
            //Storing parameters
            _width = width;
            _height = height;
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

            // Index tracking position in our pixel buffer
            int bufferPos = 0;

            // Go through every x and y
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    int value = CalculateMoireValue(focalPoints, x, y);

                    // Determine the associated color
                    Vector3D color1 = (value % 2) == 0 ? _colorB : _colorA;
                    Vector3D color2 = (value % 4) == 0 ? _colorB : _colorA;
                    Vector3D color = 0.5 * (color1 + color2 );

                    //Set the color values in buffer                   
                    _pixelBuffer[bufferPos + 0] = (byte)(color.X);
                    _pixelBuffer[bufferPos + 1] = (byte)(color.Y);
                    _pixelBuffer[bufferPos + 2] = (byte)(color.Z);
                    _pixelBuffer[bufferPos + 3] = (byte)255;

                    // Update the buffer index
                    bufferPos += _bytesPerPixel;
                }

                //Add padding for each line ( in case there is any ) 
                bufferPos += _linePaddingBytes;
            }
        }
        public void UpdateColor(params Vector3D[] focalPoints)
        {
            //Assume at least two points in list
            if (focalPoints.Length < 2) { throw new ArgumentException("Must have at least two focal points"); }

            // Index tracking position in our pixel buffer
            int bufferPos = 0;

            // Go through every value for x and y
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    // Generate moire value for given pixel
                    int value = CalculateMoireValue(focalPoints, x, y);

                    //Set the color values in buffer                   
                    // Note how we multiply up component values ( by 4 and 8).
                    // This makes them go "out of sync" as the moire value changes
                    // The effect is that it gives more variation in the selected colors
                    // ( I.e. without the multiplications we get greyscale )

                    _pixelBuffer[bufferPos + 0] = (byte)(value % 256);
                    _pixelBuffer[bufferPos + 1] = (byte)(4 * value % 256); 
                    _pixelBuffer[bufferPos + 2] = (byte)(8 * value % 256);
                    _pixelBuffer[bufferPos + 3] = (byte)255;

                    // Update the buffer index
                    bufferPos += _bytesPerPixel;
                }

                //Add padding for each line ( in case there is any ) 
                bufferPos += _linePaddingBytes;
            }
        }

        //## Private helpers
        private int CalculateMoireValue(Vector3D[] focalPoints, int x, int y)
        {
            //Calculate the distance to the first focal point
            int value = CalculateDistance(focalPoints[0], x, y);

            // Xor the distances to the remaining focal points
            for (int i = 1; i < focalPoints.Length; i++)
            {
                int newDistance = CalculateDistance(focalPoints[i], x, y);
                value ^= newDistance; // Bitwise xor
            }

            // Determines how much detail ( _ringSize too small -> too much detail )
            value /= _ringSize;

            return value;

        }
        private int CalculateDistance(Vector3D focalPoint, int x, int y)
        {
            int distX = (int)focalPoint.X - x;
            int distY = (int)focalPoint.Y - y;
            double distance = Math.Sqrt(distX * distX + distY * distY) / focalPoint.Z;
            return (int)distance;
        }

    }
}
