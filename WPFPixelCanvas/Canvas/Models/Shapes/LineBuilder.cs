using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPixelCanvas.Canvas.Models.Shapes
{

    // Class that allows drawing lines
    public class LineBuilder
    {
        private byte[] _pixelBuffer;
        private int _width;
        private int _height;
        private int _bytesPerPixel;
        private int _bytesPerLine;

        //## Constructor(s)
        public LineBuilder(byte[] buffer, int width, int height, int bytesPerPixel, int bytesPerLine)
        {
            _pixelBuffer = buffer;
        }

        //## Public interface


        private void DrawLineAsFunctionOfY(int x1, int x2, int y1, int y2)
        {

        }
        private void DrawLineAsFunctionOfX(int x1, int x2, int y1, int y2)
        {
            int kdx = 0;
            for(int m=0;m<x2;m++)
            {
                kdx++;
            }
        }

        public void drawLine(int x1,int y1, int x2, int y2)
        {
            //Determine Line Quadrant 
            int dx = x2 - x1;
            int dy = y2 - y1;

            //Are we in Q2 or Q3 ( line going left )
            if(dx<0)
            {
                // Find the absolute value of the distance between x-coordinates
                dx = -dx; 

                //We're in Q3 ( Line goes left and down in normal coordinate system )
                if (dy < 0)
                {
                    // Find the absolute value of the distance between y coordinates
                    dy = -dy;

                    // Calculate y(x) for lines where dx>dy
                    if(dx>dy)
                    {

                    }
                    else // Calculate x(y) for lines where dy>dx
                    {

                    }

                }
                else // We're in Q2 ( Line goes left and up in normal coordinate system )
                {

                }
            }
            else // Must be in Q1 or Q4
            {
                //We're in Q4 ( Line goes right and down in normal coordinate system )
                if (dy < 0)
                {
                    // Find the absolute value of the distance between y coordinates
                    dy = -dy;

                }
                else // We're in Q1 ( Line goes right and up in normal coordinate system )
                {

                }
            }

        }
    }
}
