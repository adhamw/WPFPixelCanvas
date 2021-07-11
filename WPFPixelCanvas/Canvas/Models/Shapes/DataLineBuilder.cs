using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPixelCanvas.Canvas.Models.Shapes
{

    // Class that allows drawing lines
    public class DataLineBuilder
    {
        private int _width;
        private int _height;
        private Random _randomSource;

        //## Constructor(s)
        public DataLineBuilder(Random randomSource, int width, int height)
        {
            _width = width;
            _height = height;

            _randomSource = randomSource;


        }

        //## Public interface
        public void PutPixel(double[] buffer, int x, int y, double value)
        {
            int index = y * _width + x ;
            buffer[index] = value;
        }
        public void DrawLine(double[] buffer, int x1, int y1, int x2, int y2, double value)
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
                    if (dx > dy) { CoreDrawRightToLeftDescending_XMajor(buffer, x1, y1, x2, y2, value, dy, dx); }
                    else { CoreDrawRightToLeftDescending_YMajor(buffer, x1, y1, x2, y2, value, dy, dx); }
                }
                else //## Q2 ( Line goes left and up  )
                {
                    // Draw line by stepping in x if dx>dy or in y if dx>=dy
                    if (dx > dy) { CoreDrawRightToLeftAscending_XMajor(buffer, x1, y1, x2, y2, value, dy, dx); }
                    else { CoreDrawRightToLeftAscending_YMajor(buffer, x1, y1, x2, y2, value, dy, dx); } // Calculate x(y) for lines where dy>dx
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
                    if (dx > dy) { CoreDrawLeftToRightDescending_XMajor(buffer, x1, y1, x2, y2, value, dy, dx); }
                    else { CoreDrawLeftToRightDescending_YMajor(buffer, x1, y1, x2, y2, value, dy, dx); }

                }
                else //## Q1 ( Line goes right and up  )
                {
                    // Draw line by stepping in x if dx>dy or in y if dx>=dy
                    if (dx > dy) { CoreDrawLeftToRightAscending_XMajor(buffer, x1, y1, x2, y2, value, dy, dx); }
                    else { CoreDrawLeftToRightAscending_YMajor(buffer, x1, y1, x2, y2, value, dy, dx); }
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
        private void CoreDrawLeftToRightAscending_XMajor(double[] buffer, int x1, int y1, int x2, int y2, double value, int dy, int dx)
        {

            int x = x1, y = y1;
            int kdx = 0;
            while (x < x2)
            {
                PutPixel(buffer, x, y, value);

                kdx += dy;                          // Update elevation factor
                if (kdx > dx)                       // Meaning dy/dx is now greater than one
                {
                    kdx -= dx;                      // Keeping only the reaminder
                    y++;                            // Stepping in y-direction
                    PutPixel(buffer, x, y, value);  // Plotting before updating x to avoid disconnected lines 
                }
                x++;                                // Step forward in x
            }

            PutPixel(buffer, x2, y2, value);       // Plot the endpixel of line
        }
        private void CoreDrawLeftToRightDescending_XMajor(double[] buffer, int x1, int y1, int x2, int y2, double value, int dy, int dx)
        {
            int x = x1, y = y1;
            int kdx = 0;
            while (x < x2)
            {
                PutPixel(buffer, x, y, value);

                kdx += dy;                          // Update elevation factor
                if (kdx > dx)                       // Meaning dy/dx is now greater than one
                {
                    kdx -= dx;                      // Keeping only the reaminder
                    y--;                            // Stepping in y-direction
                    PutPixel(buffer, x, y, value);  // Plotting before updating x to avoid disconnected lines 
                }
                x++;                                // Step forward in x
            }
            PutPixel(buffer, x2, y2, value);        // Plotting point for x=x2
        }
        private void CoreDrawRightToLeftAscending_XMajor(double[] buffer, int x1, int y1, int x2, int y2, double value, int dy, int dx)
        {
            int x = x1, y = y1;
            int kdx = 0;
            while (x > x2)
            {
                PutPixel(buffer, x, y, value);

                kdx += dy;                          // Update elevation factor
                if (kdx > dx)                       // Meaning dy/dx is now greater than one
                {
                    kdx -= dx;                      // Keeping only the reaminder
                    y++;                            // Stepping in y-direction
                    PutPixel(buffer, x, y, value);  // Plotting before updating x to avoid disconnected lines 
                }
                x--;                                // Step forward in x
            }
            PutPixel(buffer, x2, y2, value);     // Plotting point for x=x2
        }
        private void CoreDrawRightToLeftDescending_XMajor(double[] buffer, int x1, int y1, int x2, int y2, double value, int dy, int dx)
        {

            int x = x1, y = y1;
            int kdx = 0;
            while (x > x2)
            {
                PutPixel(buffer, x, y, value);

                kdx += dy;                          // Update elevation factor
                if (kdx > dx)                       // Meaning dy/dx is now greater than one
                {
                    kdx -= dx;                      // Keeping only the reaminder
                    y--;                            // Stepping in y-direction
                    PutPixel(buffer, x, y, value);     // Plotting before updating x to avoid disconnected lines 
                }
                x--;                                // Step forward in x
            }
            PutPixel(buffer, x2, y2, value);     // Plotting point for x=x2
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
        private void CoreDrawLeftToRightAscending_YMajor(double[] buffer, int x1, int y1, int x2, int y2, double value, int dy, int dx)
        {
            int x = x1, y = y1;
            int kdy = 0;
            while (y < y2)
            {
                PutPixel(buffer, x, y, value);

                kdy += dx;                          // Update elevation factor
                if (kdy > dy)                       // Meaning dy/dx is now greater than one
                {
                    kdy -= dy;                      // Keeping only the remainder
                    x++;                            // Stepping in x-direction
                    PutPixel(buffer, x, y, value);  // Plotting before updating x to avoid disconnected lines 
                }
                y++;                                // Step forward in y
            }

            PutPixel(buffer, x2, y2, value);   // Plot the end pixel of line
        }
        private void CoreDrawLeftToRightDescending_YMajor(double [] buffer, int x1, int y1, int x2, int y2, double value, int dy, int dx)
        {
            int x = x1, y = y1;
            int kdy = 0;
            while (y > y2)
            {
                PutPixel(buffer, x, y, value);

                kdy += dx;                          // Update elevation factor
                if (kdy > dy)                       // Meaning dx/dy is now greater than one
                {
                    kdy -= dy;                      // Keeping only the reaminder
                    x++;                            // Stepping in x-direction
                    PutPixel(buffer, x, y, value);     // Plotting before updating x to avoid disconnected lines 
                }
                y--;                                // Step forward in y
            }
            PutPixel(buffer, x2, y2, value);     // Plotting point for x=x2

        }
        private void CoreDrawRightToLeftAscending_YMajor(double[] buffer, int x1, int y1, int x2, int y2, double value, int dy, int dx)
        {
            int x = x1, y = y1;
            int kdy = 0;
            while (y < y2)
            {
                PutPixel(buffer, x, y, value);

                kdy += dx;                          // Update elevation factor
                if (kdy > dy)                       // Meaning dx/dy is now greater than one
                {
                    kdy -= dy;                      // Keeping only the reaminder
                    x--;                            // Stepping in x-direction
                    PutPixel(buffer, x, y, value);  // Plotting before updating x to avoid disconnected lines 
                }
                y++;                                // Step forward in y
            }
            PutPixel(buffer, x2, y2, value);        // Plotting point for x=x2
        }
        private void CoreDrawRightToLeftDescending_YMajor(double[] buffer, int x1, int y1, int x2, int y2, double value, int dy, int dx)
        {
            int x = x1, y = y1;
            int kdy = 0;
            while (y > y2)
            {
                PutPixel(buffer, x, y, value);

                kdy += dx;                          // Update elevation factor
                if (kdy > dy)                       // Meaning dx/dy is now greater than one
                {
                    kdy -= dy;                      // Keeping only the reaminder
                    x--;                            // Stepping in x-direction
                    PutPixel(buffer, x, y, value);  // Plotting before updating x to avoid disconnected lines 
                }
                y--;                                // Step forward in y
            }
            PutPixel(buffer, x2, y2, value);        // Plotting point for x=x2
        }

    }
}
