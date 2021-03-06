using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPixelCanvas.Canvas.Models.PlotModules
{
    public class Pattern_RandomDots : ICanvasPlotter
    {
        //Local fields
        private byte[] _buffer { get; set; }

        //Constructor
        public Pattern_RandomDots(int width, int height)
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

            //Draw 10.000 random dots
            int pos;
            Random rand = new Random();      //Prepares a random number source
            for (int i = 0; i < 10000; i++)
            {
                int x = rand.Next(width);   // Picks a random integer between 0 and WIDTH
                int y = rand.Next(height);  // Picks a random integer between 0 and HEIGHT

                //Next we want to convert the position (x,y) to a position in our buffer.
                pos = (y * bytesperline) + (x * bytesperpixel);

                #region ** How does the conversion formula work? Look here.**
                // Assume the canvas is 800 pixels wide. 
                // Each pixel is defined by 4 bytes: [b][g][r][a]
                // To describe all pixels on a horizontal line, we
                // would need: 800 pixels * 4 bytes/pixel = 3200bytes
                // 

                // Assume the first byte in our buffer maps to the first pixel
                // on the first line. ( The upper left corner of our image )

                // To go to the next line in our buffer, we would count up
                // as many bytes as there is in one line. In our example we add
                // 3200 to our position in the buffer.

                // In other words: y * bytes_per_line --> position in buffer
                // of the first pixel on the yth line


                //To move one pixel forward, we must move 4 positions in the buffer
                //Pointer = 0:          ↓ 
                //    ( Buffer )       [b][g][r][a][b][g][r][a][b][g][r][a]...
                //    ( pointer value) [0][1][2][3][4][5][6][7][8][9]...
                //    ( pixel )        |- Pixel 1 -|- Pixel 2 -|- Pixel 3 -| ...
                //    ( x-coordinate ) |-   x=0   -|-   x=1   -|-   x=2   -| ...
                //Pointer = 4:                      ↓ 
                //    ( Buffer )       [b][g][r][a][b][g][r][a][b][g][r][a]...
                //    ( pointer value) [0][1][2][3][4][5][6][7][8][9]...
                //    ( pixel )        |- Pixel 1 -|- Pixel 2 -|- Pixel 3 -| ...
                //    ( x-coordinate ) |-   x=0   -|-   x=1   -|-   x=2   -| ...
                // etc.
                // So the x-position is given by x*4 = x * bytesperpixel.
                // This is the x-position relative to the first pixel on a line.
                // To get the buffer position for a point (x,y), we have to add
                // these two values together:
                #endregion

                // Setting the color to Black, for the pixel at (x,y):
                _buffer[pos + 0] = 0;   //Blue component
                _buffer[pos + 1] = 0;   //Green component ==> (R,G,B) = (0,0,0) = Black
                _buffer[pos + 2] = 0;   //Red component 
                _buffer[pos + 3] = 255; //Alpha component

                #region ** How does setting the color work? Look here **
                //We have a pointer (pos) that points to pixel data for a pixel
                //at position x,y:
                //                               ↓--------------  pos+0 = pos
                //                               ↓  ↓-----------  pos+1
                //                               ↓  ↓  ↓--------  pos+2
                //    ( Buffer )       ...[r][a][b][g][r][a][b][g]...

                //Each component can take a value between 0 and 255 and defines
                //how much of it should contribute to the final color.
                // At 0 the component does not contribute, at 255 it contributers 100%.
                // With all components set to zero, we get black.
                #endregion
            }

            //Return buffer ( reference value )
            return _buffer;
        }
    }
}


