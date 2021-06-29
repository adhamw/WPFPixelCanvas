using System;

namespace WPFPixelCanvas.Canvas.Models
{
    public class MyPattern : ICanvasPlotter
    {
        //Constructor
        public MyPattern(int width, int height)
        {
            Width = width;      // Defines width of plot area ( e.g. 800 pixels )
            Height = height;    // Defines height of plot area ( e.g. 600 pixels )
        }

        //Public properties
        public int Width { get; private set; }
        public int Height { get; private set; }

        //Public interface
        public void Plot(IntPtr buffer, int bytesperpixel, long refreshcounter = 0)
        {
            int pos = 0;
            unsafe
            {
                // Retrieve a byte pointer to our graphics buffer
                var bytes = (byte*)buffer.ToPointer();

                //Run through all lines in image
                for (int y = 0; y < Height; y++)
                {
                    //Run through every pixel on a line
                    for (int x = 0; x < Width; x++)
                    {
                        // Setting the color blue
                        byte colorR = 0;       //
                        byte colorG = 0;       //  ==> (R, G, B) = (0, 0, 255) = Blue
                        byte colorB = 255;     // 

                        // Setting the color to Black, for the pixel at (x,y):
                        bytes[pos + 0] = colorB;   //Blue component
                        bytes[pos + 1] = colorG;   //Green component 
                        bytes[pos + 2] = colorR;   //Red component 
                        bytes[pos + 3] = 255; //Alpha component

                        pos += bytesperpixel;  // Moving buffer pointer forward
                    }
                }

            }


        }
    }
}
