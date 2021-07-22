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

        //private double[,] _multiplicationTable { get; set; }
        private double[] _dataBuffer;

        //## Constructor(s)
        public BlobManager(Blob[] blobs, byte[] pixelBuffer, int width, int height, int bytesPerPixel, int linePaddingBytes)
        {
            //Storing parameters
            _width = width;
            _height = height;
            _bytesPerPixel = bytesPerPixel;
            _linePaddingBytes = linePaddingBytes;
            _pixelBuffer = pixelBuffer;
            _blobs = blobs;

            //Create data buffer
            _pixelBufferSize = (_width* _bytesPerPixel +_linePaddingBytes)*_height;
            _dataBuffer = new double[_pixelBufferSize];
            blobstrengths = new double[_pixelBufferSize];


            // Define array that can hold temp values for each blob's strength
            //_blobStrengths = new double[blobs.Length];

            // Create multiplication table
            
            //_multiplicationTable = new double[(width+1)*2, (height+1)*2];
            //for(int i = -(height+1); i < (height+1); i++)
            //{
            //    for(int j = -(width+1); j < (width+1); j++)
            //    {
            //        _multiplicationTable[j+width+1, i+height+1] = Math.Sqrt((i * i) + (j * j));
            //    }
            //}

        }

        //## Public interface
        //public void UpdateOld()
        //{
        //    // Index tracking position in our pixel buffer
        //    int bufferPos = 0;
        //    _blobStrengths = new double[_blobs.Length];

        //    // Go through every x and y
        //    for (int y = 0; y < _height; y++)
        //    {
        //        for (int x = 0; x < _width; x++)
        //        {
        //            Vector3D color = CalculateBlobContribution(x,y);
                    

        //            //Set the color values in buffer                   
        //            _pixelBuffer[bufferPos + 0] = (byte)(color.X%255 );
        //            _pixelBuffer[bufferPos + 1] = (byte)(color.Y%255 );
        //            _pixelBuffer[bufferPos + 2] = (byte)(color.Z%255 );
        //            _pixelBuffer[bufferPos + 3] = (byte)255;

        //            // Update the buffer index
        //            bufferPos += _bytesPerPixel;
        //        }

        //        //Add padding for each line ( in case there is any ) 
        //        bufferPos += _linePaddingBytes;
        //    }
        //}

        double[] blobstrengths; // = new double[_pixelBufferSize];
        public void Update()
        {
            double blobStrengthBase, blobX, blobY;
            double blobStrength, distX, distY, distZ, distance;
            int bytesPerPixel = _bytesPerPixel;
            int linePaddingBytes = _linePaddingBytes;

            Blob blob;
            Vector3D colorContribution;
            Vector3D blobPosition;

            //double[] pb = new double[_pixelBufferSize];
            double[] pb = _dataBuffer;
            for (int i = 0; i < _pixelBufferSize; i++ )
            {
                pb[i] = i%4 == 3 ? 255: 0;
                blobstrengths[i] = 0;
            }

            int numberOfBlobs = _blobs.Length;
            Blob[] blobs = _blobs;

            // Index tracking position in our pixel buffer
            int bufferPos = 0;
            //_blobStrengths = new double[_blobs.Length];

            //For every blob
            double maxDistance = _width > _height ? _width : _height;
            for (int i = 0; i < numberOfBlobs; i++)
            {

                blob = blobs[i];
                blobStrengthBase = blob.Strength;
                blobX = blob.Position.X;
                blobY = blob.Position.Y;

                bufferPos = 0;
                Vector3D baseColor = blob.BaseColor;

                //Run through pixels and apply blob effect
                for (int y = 0; y < _height; y++)
                {
                    for (int x = 0; x < _width; x++)
                    {
                        distX = blobX - x;
                        distY = blobY - y;
                        //distZ = blobPosition.Z;
                        distance = Math.Sqrt(distX * distX + distY * distY) ;
                        blobStrength =  blobStrengthBase * (maxDistance / (1 + distance));                  // The effect of a blob on a pixel
                        colorContribution = blobStrength * baseColor;

                        blobstrengths[bufferPos + 0] += blobStrength; // Note using same buffer layout as databuffer

                        //Set the color values in buffer                   
                        pb[bufferPos + 0] += colorContribution.X;
                        pb[bufferPos + 1] += colorContribution.Y;
                        pb[bufferPos + 2] += colorContribution.Z;
                        

                        // Update the buffer index
                        bufferPos += bytesPerPixel;

                    }
                    //Add padding for each line ( in case there is any ) 
                    bufferPos += linePaddingBytes;
                }
            }

            // Convert generated data to colordata
            int bsindex = 0;
            for(int i=0;i<_pixelBufferSize; i++)
            {
                if (i % 4 == 3) 
                { 
                    bsindex += 4;
                    _pixelBuffer[i] = 255;
                }
                else

                {
                    double value = pb[i] / blobstrengths[bsindex];
                    if (value > 55.0) { value = (1.28*value-70); }
                    _pixelBuffer[i] += (byte)(value);

                }
            }

        }


    }

}
