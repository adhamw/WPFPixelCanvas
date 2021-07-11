using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPixelCanvas.Canvas.Models.Shapes
{

    // Class that allows drawing lines
    public class PixelLineBuilder
    {
        private int _width;
        private int _height;
        private int _bytesPerPixel;
        private int _bytesPerLine;

        //## Constructor(s)
        public PixelLineBuilder(int width, int height, int bytesPerPixel, int bytesPerLine)
        {
            _width = width;
            _height = height;
            _bytesPerLine = bytesPerLine;
            _bytesPerPixel = bytesPerPixel;
        }

        //## Public interface
        public void putPixel(byte[] buffer, int x, int y, byte r, byte g, byte b, byte alpha)
        {
            int index = y * _bytesPerLine + x * _bytesPerPixel;
            buffer[index + 0] = b;
            buffer[index + 1] = g;
            buffer[index + 2] = r;
            buffer[index + 3] = alpha;
        }
        public void DrawLine(byte[] buffer, int x1, int y1, int x2, int y2, byte r, byte g, byte b, byte a)
        {
            //Determine Line Quadrant 
            int dx = x2 - x1;
            int dy = y2 - y1;


            //Are we in Q2 or Q3 ( line going left )
            if (dx <= 0)
            {
                // Find the absolute value of the distance between x-coordinates
                dx = -dx;

                //## Q3 ( Line goes left and down )
                if (dy < 0)
                {
                    // Find the absolute value of the distance between y coordinates
                    dy = -dy;

                    // Draw line by stepping in x if dx>dy or in y if dx>=dy
                    if (dx > dy) { CoreDrawRightToLeftDescending_XMajor(buffer,x1, y1, x2, y2, r, g, b, a, dy, dx); }
                    else { CoreDrawRightToLeftDescending_YMajor(buffer, x1, y1, x2, y2, r, g, b, a, dy, dx); }
                }
                else //## Q2 ( Line goes left and up  )
                {
                    // Draw line by stepping in x if dx>dy or in y if dx>=dy
                    if (dx > dy) { CoreDrawRightToLeftAscending_XMajor(buffer, x1, y1, x2, y2, r, g, b, a, dy, dx); }
                    else { CoreDrawRightToLeftAscending_YMajor(buffer, x1, y1, x2, y2, r, g, b, a, dy, dx); } // Calculate x(y) for lines where dy>dx
                }
            }
            else // Must be in Q1 or Q4
            {
                //## Q4 ( Line goes right and down )
                if (dy < 0)
                {
                    // Find the absolute value of the distance between y coordinates
                    dy = -dy;

                    // Draw line by stepping in x if dx>dy or in y if dx>=dy
                    if (dx > dy) { CoreDrawLeftToRightDescending_XMajor(buffer, x1, y1, x2, y2, r, g, b, a, dy, dx); }
                    else { CoreDrawLeftToRightDescending_YMajor(buffer, x1, y1, x2, y2, r, g, b, a, dy, dx); }

                }
                else //## Q1 ( Line goes right and up  )
                {
                    // Draw line by stepping in x if dx>dy or in y if dx>=dy
                    if (dx > dy) { CoreDrawLeftToRightAscending_XMajor(buffer, x1, y1, x2, y2, r, g, b, a, dy, dx); }
                    else { CoreDrawLeftToRightAscending_YMajor(buffer, x1, y1, x2, y2, r, g, b, a, dy, dx); }
                }
            }

        }


        //## Private interface       
        /// <summary>
        /// Variations on the core line drawing functions for dx>dy
        /// There is one for each Quarter of the coordinate system.
        /// The difference between each is whether line is being drawn
        /// left to right/right to left and wether y is descending or ascending
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        /// <param name="y1"></param>
        /// <param name="y2"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        /// <param name="dy"></param>
        /// <param name="dx"></param>      
        private void CoreDrawLeftToRightAscending_XMajor(byte[] buffer, int x1, int y1, int x2, int y2, byte r, byte g, byte b, byte a, int dy, int dx)
        {

            int x = x1, y = y1;
            int kdx = 0;
            while (x < x2)
            {
                putPixel(buffer, x, y, r, g, b, a);

                kdx += dy;                          // Update elevation factor
                if (kdx > dx)                       // Meaning dy/dx is now greater than one
                {
                    kdx -= dx;                      // Keeping only the reaminder
                    y++;                            // Stepping in y-direction
                    putPixel(buffer, x, y, r, g, b, a);     // Plotting before updating x to avoid disconnected lines 
                }
                x++;                                // Step forward in x
            }

            putPixel(buffer, x2, y2, r, g, b, a);   // Plot the endpixel of line
        }
        private void CoreDrawLeftToRightDescending_XMajor(byte[] buffer, int x1, int y1, int x2, int y2, byte r, byte g, byte b, byte a, int dy, int dx)
        {
            int x = x1, y = y1;
            int kdx = 0;
            while (x < x2)
            {
                putPixel(buffer, x, y, r, g, b, a);

                kdx += dy;                          // Update elevation factor
                if (kdx > dx)                       // Meaning dy/dx is now greater than one
                {
                    kdx -= dx;                      // Keeping only the reaminder
                    y--;                            // Stepping in y-direction
                    putPixel(buffer, x, y, r, g, b, a);     // Plotting before updating x to avoid disconnected lines 
                }
                x++;                                // Step forward in x
            }
            putPixel(buffer, x2, y2, r, g, b, a);     // Plotting point for x=x2
        }
        private void CoreDrawRightToLeftAscending_XMajor(byte[] buffer, int x1, int y1, int x2, int y2, byte r, byte g, byte b, byte a, int dy, int dx)
        {
            int x = x1, y = y1;
            int kdx = 0;
            while (x > x2)
            {
                putPixel(buffer, x, y, r, g, b, a);

                kdx += dy;                          // Update elevation factor
                if (kdx > dx)                       // Meaning dy/dx is now greater than one
                {
                    kdx -= dx;                      // Keeping only the reaminder
                    y++;                            // Stepping in y-direction
                    putPixel(buffer, x, y, r, g, b, a);     // Plotting before updating x to avoid disconnected lines 
                }
                x--;                                // Step forward in x
            }
            putPixel(buffer, x2, y2, r, g, b, a);     // Plotting point for x=x2
        }
        private void CoreDrawRightToLeftDescending_XMajor(byte[] buffer, int x1, int y1, int x2, int y2, byte r, byte g, byte b, byte a, int dy, int dx)
        {
         
            int x = x1, y = y1;
            int kdx = 0;
            while (x > x2)
            {
                putPixel(buffer, x, y, r, g, b, a);

                kdx += dy;                          // Update elevation factor
                if (kdx > dx)                       // Meaning dy/dx is now greater than one
                {
                    kdx -= dx;                      // Keeping only the reaminder
                    y--;                            // Stepping in y-direction
                    putPixel(buffer, x, y, r, g, b, a);     // Plotting before updating x to avoid disconnected lines 
                }
                x--;                                // Step forward in x
            }
            putPixel(buffer, x2, y2, r, g, b, a);     // Plotting point for x=x2
        }


        /// <summary>
        /// Variations on the core line drawing functions for: dy > dx
        /// We step forward in y, and calculate the associated x value.
        /// 
        /// This simplifies drawing operation to values of line inclination 
        /// that are less than one (dy/dx <=1) 
        ///
        /// There is one funciton for each Quarter of the coordinate system.
        /// The difference between each is whether line is being drawn
        /// left to right/right to left and wether y is descending or ascending
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        /// <param name="y1"></param>
        /// <param name="y2"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        /// <param name="dy"></param>
        /// <param name="dx"></param>      
        private void CoreDrawLeftToRightAscending_YMajor(byte[] buffer, int x1, int y1, int x2, int y2, byte r, byte g, byte b, byte a, int dy, int dx)
        {
            int x = x1, y = y1;
            int kdy = 0;
            while (y < y2)
            {
                putPixel(buffer, x, y, r, g, b, a);

                kdy += dx;                          // Update elevation factor
                if (kdy > dy)                       // Meaning dy/dx is now greater than one
                {
                    kdy -= dy;                      // Keeping only the remainder
                    x++;                            // Stepping in x-direction
                    putPixel(buffer, x, y, r, g, b, a);     // Plotting before updating x to avoid disconnected lines 
                }
                y++;                                // Step forward in y
            }

            putPixel(buffer, x2, y2, r, g, b, a);   // Plot the end pixel of line
        }
        private void CoreDrawLeftToRightDescending_YMajor(byte[] buffer, int x1, int y1, int x2, int y2, byte r, byte g, byte b, byte a, int dy, int dx)
        {
            int x = x1, y = y1;
            int kdy = 0;
            while (y > y2)
            {
                putPixel(buffer, x, y, r, g, b, a);

                kdy += dx;                          // Update elevation factor
                if (kdy > dy)                       // Meaning dx/dy is now greater than one
                {
                    kdy -= dy;                      // Keeping only the reaminder
                    x++;                            // Stepping in x-direction
                    putPixel(buffer, x, y, r, g, b, a);     // Plotting before updating x to avoid disconnected lines 
                }
                y--;                                // Step forward in y
            }
            putPixel(buffer, x2, y2, r, g, b, a);     // Plotting point for x=x2

        }
        private void CoreDrawRightToLeftAscending_YMajor(byte[] buffer, int x1, int y1, int x2, int y2, byte r, byte g, byte b, byte a, int dy, int dx)
        {
            int x = x1, y = y1;
            int kdy = 0;
            while (y < y2)
            {
                putPixel(buffer, x, y, r, g, b, a);

                kdy += dx;                          // Update elevation factor
                if (kdy > dy)                       // Meaning dx/dy is now greater than one
                {
                    kdy -= dy;                      // Keeping only the reaminder
                    x--;                            // Stepping in x-direction
                    putPixel(buffer, x, y, r, g, b, a);     // Plotting before updating x to avoid disconnected lines 
                }
                y++;                                // Step forward in y
            }
            putPixel(buffer, x2, y2, r, g, b, a);     // Plotting point for x=x2
        }
        private void CoreDrawRightToLeftDescending_YMajor(byte[] buffer, int x1, int y1, int x2, int y2, byte r, byte g, byte b, byte a, int dy, int dx)
        {
            int x = x1, y = y1;
            int kdy = 0;
            while (y > y2)
            {
                putPixel(buffer, x, y, r, g, b, a);

                kdy += dx;                          // Update elevation factor
                if (kdy > dy)                       // Meaning dx/dy is now greater than one
                {
                    kdy -= dy;                      // Keeping only the reaminder
                    x--;                            // Stepping in x-direction
                    putPixel(buffer, x, y, r, g, b, a);     // Plotting before updating x to avoid disconnected lines 
                }
                y--;                                // Step forward in y
            }
            putPixel(buffer, x2, y2, r, g, b, a);     // Plotting point for x=x2
        }

    }
}
