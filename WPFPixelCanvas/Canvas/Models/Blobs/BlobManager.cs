using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFPixelCanvas.Canvas.Models.Boids3D;

namespace WPFPixelCanvas.Canvas.Models.Blobs
{
    public class BlobManager
    {
        //## Private fields
        private int _width { get; set; }
        private int _height { get; set; }
        private byte[] _pixelBuffer { get; set; }
        private int _bytesPerPixel { get; set; }
        private int _linePaddingBytes { get; set; }
        private int _pixelBufferSize { get; set; }

        private Blob[] _blobs { get; set; }
        double[] _blobStrengths;
        private double[] _dataBuffer;

        //## Constructor(s)
        public BlobManager(Blob[] blobs, byte[] pixelBuffer, int width, int height, int bytesPerPixel, int linePaddingBytes)
        {
            //Storing incoming parameters
            _width = width;
            _height = height;
            _bytesPerPixel = bytesPerPixel;
            _linePaddingBytes = linePaddingBytes;
            _pixelBuffer = pixelBuffer;
            _blobs = blobs;

            //Create data buffer
            _pixelBufferSize = (_width* _bytesPerPixel +_linePaddingBytes)*_height;
            _dataBuffer = new double[_pixelBufferSize];
            _blobStrengths = new double[_pixelBufferSize];
        }


        public void Update()
        {
            //Declaring local variables
            double blobStrengthBase, blobX, blobY;
            double blobStrength, distX, distY,  distance;
            Blob blob; Vector3D colorContribution;

            // Local references to class variables            
            int bytesPerPixel = _bytesPerPixel;
            int linePaddingBytes = _linePaddingBytes;
            byte[] pixelBuffer = _pixelBuffer;
            double[] dataBuffer = _dataBuffer;
            double[] blobStrengths = _blobStrengths;
            int numberOfBlobs = _blobs.Length;
            Blob[] blobs = _blobs;

            // Index tracking position in buffers
            int bufferPos;

            // Clearing buffers. 
            // Note that buffers are arranged just like the pixelbuffer
            // These are used to hold partially computed color values
            bufferPos = 0;
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    dataBuffer[bufferPos + 0] = 0;
                    dataBuffer[bufferPos + 1] = 0;
                    dataBuffer[bufferPos + 2] = 0;
                    dataBuffer[bufferPos + 3] = 255;
                    blobStrengths[bufferPos + 0] = 0;
                    blobStrengths[bufferPos + 1] = 0;
                    blobStrengths[bufferPos + 2] = 0;
                    blobStrengths[bufferPos + 3] = 255;

                    bufferPos += bytesPerPixel;
                }
                bufferPos += linePaddingBytes;

            }

            // Main calculation loop
            // For every blob point, we calculate the contribution from the blob onto each pixel.

            // Contribution is determined from relationship :
            // = (distance between blob and pixel)/(Max distance)   ##
            // = (Max distance) / (distance)                        ## However, we want maximum contribution when distance is zero, so we invert the relationship.
            // = (Max distance ) / (1+distance)                     ## Distance will be '0' or larger, but we cannot divide by '0' so we add '1' to the denominator
            // = BlobStrength * (Max distance ) / (1+distance)      ## Each blob has a strength factor
            //
            // Finally, each blob has a defined color, so we multiply by color (vector)
            // to get a contribution per component:
            // Contribution = [Base color(r,g,b)] * BlobStrength * (Max distance ) / (1+distance)
            
            double maxDistance = _width > _height ? _width : _height; // Choose whichever is greater ( width or height ) as the max value for distance
            for (int i = 0; i < numberOfBlobs; i++)
            {
                blob = blobs[i];                    // Reference the blob object
                blobStrengthBase = blob.Strength;   // Reference the blob strength value
                blobX = blob.Position.X;            // Reference the blob position
                blobY = blob.Position.Y;            // ditto
                Vector3D baseColor = blob.BaseColor;// Reference the blob base color

                //Calculate blob contribution for every pixel/coordinate
                bufferPos = 0;
                for (int y = 0; y < _height; y++)
                {
                    for (int x = 0; x < _width; x++)
                    {
                        // Calculate the distance between pixel and blob
                        distX = blobX - x;                                      // Horizontal distance between blob and pixel
                        distY = blobY - y;                                      // Vertical distance between blob and pixel
                        distance = Math.Sqrt(distX * distX + distY * distY);    // Point distance between pixel and blob

                        // Calculate blob contribution to this pixel
                        blobStrength =  blobStrengthBase * (maxDistance / (1 + distance));
                        colorContribution = blobStrength * baseColor;           // The effect of a blob on a pixel

                        // Record the total applied blobstrengths per pixel
                        blobStrengths[bufferPos + 0] += blobStrength;           // Allows normalizing the color value. ( I.e. we are scaling by 'contribution/total contribution' )

                        //Set the intermediate color values in buffer           // Values must be normalized, but can't do that until 
                        dataBuffer[bufferPos + 0] += colorContribution.X;       // we know the total blobstrength which we will find out 
                        dataBuffer[bufferPos + 1] += colorContribution.Y;       // after all pixels/blobs has been processed
                        dataBuffer[bufferPos + 2] += colorContribution.Z;       // Awaiting that result, we store contributions in the data buffer
                        
                        // Update the buffer index
                        bufferPos += bytesPerPixel;
                    }

                    //Add padding for each line ( in case there is any ) 
                    bufferPos += linePaddingBytes;
                }
            }

            // Normalize values and store in output buffer
            int currpos;double value;
            bufferPos = 0;

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    //Setting the r,g,b valuse
                    for(int component = 0; component < 3; component++)
                    {
                        currpos = bufferPos + component;                                // bufferPos+0 -> G, bufferPos+1 -> B, bufferPos+2 -> R
                        double totalBlobStrengthOnPixel = blobStrengths[bufferPos];     // The sum of all blob contributions on pixel at pos x,y
                        value = (byte)(dataBuffer[currpos] / totalBlobStrengthOnPixel); // Normalize contribution value by dividing by total Contribution value

                        // Adjusting values so we get more "blob" like behavior.
                        // We are creating a divide at componentvalues of 55 or more by 
                        // multiplying such values by 1.28. We then subtract a value
                        // to avoid ugly black spot in the middle
                        // Note remove this, and you get pretty light-sources moving about
                        // but not so "blob"-like anynmore.
                        if (value > 55.0) { value = (1.28 * value - 70); }              

                        //Assign value to our buffer
                        pixelBuffer[currpos] = (byte)value;
                    }

                    // Fourth byte is alpha. Setting to no transparency.
                    currpos = bufferPos + 3;
                    pixelBuffer[currpos] = 255;

                    // Move index forward
                    bufferPos += bytesPerPixel;
                }
                //Add padding for each line ( in case there is any ) 
                bufferPos += linePaddingBytes;
            }
        }
    }

}
