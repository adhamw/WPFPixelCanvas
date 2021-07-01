using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPixelCanvas.Canvas.Models.PlotModules
{
    public class Pattern_FillBlue : ICanvasPlotter
    {
        //Local fields
        private byte[] _buffer { get; set; }

        //Constructor
        public Pattern_FillBlue(int width, int height)
        {
            Width = width;      // Defines width of plot area ( e.g. 800 pixels )
            Height = height;    // Defines height of plot area ( e.g. 600 pixels )
        }

        //Public properties
        public int Width { get; private set; }
        public int Height { get; private set; }

        //Public interface
        public byte[] Plot(int bytesperpixel, int bytesperline, long refreshcounter = 0)
        {
            int width = Width;                          // Storing widht in local variable ( caching for speed ) 
            int height = Height;                        // ditto

            //Initialize buffer if not set
            int bytesinbuffer = bytesperline * height;  // Number of bytes needed for the whole buffer
            if (_buffer == null) { _buffer = new byte[bytesinbuffer]; }

            //Sets all values to white, with no transparency ( I.e. [255][255][255]... )
            for (int i = 0; i < bytesinbuffer; i++) { _buffer[i] = 255; }

            int pos = 0;    // Buffer index

            //Run through all lines in image
            for (int y = 0; y < height; y++)
            {
                //Run through every pixel on a line
                for (int x = 0; x < width; x++)
                {
                    // Setting the color blue
                    byte colorR = 0;       //
                    byte colorG = 0;       //  ==> (R, G, B) = (0, 0, 255) = Blue
                    byte colorB = 255;     // 

                    // Setting the color to Black, for the pixel at (x,y):
                    _buffer[pos + 0] = colorB;   //Blue component
                    _buffer[pos + 1] = colorG;   //Green component 
                    _buffer[pos + 2] = colorR;   //Red component 
                    _buffer[pos + 3] = 255; //Alpha component

                    pos += bytesperpixel;  // Moving buffer pointer forward
                }
            }

            //Return buffer ( reference value )
            return _buffer;
        }
    }
}


