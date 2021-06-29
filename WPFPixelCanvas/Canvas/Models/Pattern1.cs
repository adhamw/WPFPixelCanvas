using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPixelCanvas.Canvas.Models
{
    public class Pattern1 : ICanvasPlotter
    {
        //Constructor
        public Pattern1(int width, int height)
        {
            Width = width;      // Defines width of plot area ( e.g. 800 pixels )
            Height = height;    // Defines height of plot area ( e.g. 600 pixels )
        }

        //Public properties
        public int Width { get; private set; }
        public int Height { get; private set; }

        //Public interface
        public void Plot(IntPtr buffer, int bytesperpixel, long refreshcounter=0)
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
                        // Calculating colors based on x and y values
                        byte colorR = (byte)((255*y/height));
                        byte colorG = (byte)((x * y+refreshcounter) % 256);
                        byte colorB = (byte)((x / (y + 1)) % 256);

                        // Setting the color to Black, for the pixel at (x,y):
                        bytes[pos + 0] = colorB;   //Blue component
                        bytes[pos + 1] = colorG;   //Green component ==> (R,G,B) = (0,0,0) = Black
                        bytes[pos + 2] = colorR;   //Red component 
                        bytes[pos + 3] = 255; //Alpha component


                        pos += bytesperpixel;  // Moving buffer pointer forward
                    }
                }

            }


        }
    }
}
