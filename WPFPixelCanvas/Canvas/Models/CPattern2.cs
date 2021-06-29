using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPixelCanvas.Canvas.Models
{
    class CPattern2 : ICanvasPlotter
    {
        //Constructor
        public CPattern2(int width, int height)
        {
            Width = width;      // Defines width of plot area ( e.g. 800 pixels )
            Height = height;    // Defines height of plot area ( e.g. 600 pixels )
        }

        //Public properties
        public int Width { get; private set; }
        public int Height { get; private set; }

        //Public interface
        public void plot(IntPtr buffer, int bytesperpixel, long refreshcounter = 0)
        {
            int width = Width;                          // Storing widht in local variable ( caching for speed ) 
            int height = Height;                        // ditto
            int pos = 0;

            int bytesperline = bytesperpixel * width;   // Number of bytes in a single line in buffer
            int bytesinbuffer = bytesperline * height;  // Number of bytes needed for the whole buffer

            unsafe
            {
                // Retrieve a byte pointer to our graphics buffer
                var bytes = (byte*)buffer.ToPointer();

                for (int i = 0; i < bytesinbuffer; i++) { bytes[i] = 255; }

                //Run through all lines in image
                for (int y = 0; y < height; y++)
                {
                    //Run through every pixel on a line
                    for (int x = 0; x < width; x++)
                    {
                        //Offset the x and y values
                        double ox = Math.Abs(x + 50.0 * Math.Cos((refreshcounter)*0.012 + Math.Sin((y+refreshcounter) * 0.02)));
                        double oy = Math.Abs(y + 50.0 * Math.Cos((refreshcounter)*0.017 + Math.Cos((x+refreshcounter) * 0.02))); 

                        //Convert from coordinates to color values
                        double rval = 255.0 * (oy / height);                // The percentage of the max value for y, multiplied by 255
                        double gval = 255.0 * (ox + oy) / (width + height); // The percentage of the max value of x+y, multiplied by 255
                        double bval = 255.0 * (ox / width);                 // The percentage of the max value for x, multiplied by 255

                        // Cast double values to byte values
                        byte colorR = (byte)rval;
                        byte colorG = (byte)gval;
                        byte colorB = (byte)bval;

                        // Storing color values in buffer
                        bytes[pos + 0] = colorB;   //Blue component
                        bytes[pos + 1] = colorG;   //Green component 
                        bytes[pos + 2] = colorR;   //Red component 
                        bytes[pos + 3] = 255;      //Alpha component

                        pos += bytesperpixel;  // Moving buffer pointer forward
                    }
                }

            }
        }

    }
}
