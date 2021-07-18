// Uses Vector3D from Boids3D project
// Uses SpriteManager from Sprites project


using global::WPFPixelCanvas.Canvas.Models.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFPixelCanvas.Canvas.Models.Boids3D;
using WPFPixelCanvas.Canvas.Models.Woims;

namespace WPFPixelCanvas.Canvas.Models.ASelectionOfPlotModules
{
    public class Pattern_Woims : ICanvasPlotter
    {
        //Local fields
        private byte[] _buffer { get; set; }
        private SpriteManager _spriteManager { get; set; }

        //Constructor
        public Pattern_Woims(int width, int height)
        {
            Width = width;      // Defines width of plot area ( e.g. 800 pixels )
            Height = height;    // Defines height of plot area ( e.g. 600 pixels )
        }

        //Public properties
        public int Width { get; private set; }
        public int Height { get; private set; }

        //Public interface
        private Woim3D _woim = null;
        public byte[] Plot(int bytesperpixel, int bytesperline, long refreshcounter = 0)
        {
            //Initialize buffer if not set
            int bytesinbuffer = bytesperline * Height;  // Number of bytes needed for the whole buffer
            if (_buffer == null)
            {
                _buffer = new byte[bytesinbuffer];

                _spriteManager = new SpriteManager(_buffer, Width, Height, bytesperpixel, new Tuple<byte, byte, byte>(250, 180, 250));
                _spriteManager.AddSprite().loadImage(@".\SpriteData.\glassball2_128x128.bmp", bytesperpixel);

                // Define limits
                Woim3DLimits headLimits = new Woim3DLimits()
                {
                    AccelerationMin = new Vector3D(-0.25, -0.25, -0.05),
                    AccelerationMax = new Vector3D(0.25, 0.25, 0.05),
                    
                    VelocityMin = new Vector3D(-2.9, -2.9, -0.25),
                    VelocityMax = new Vector3D(2.9, 2.9, 0.25),
                    
                    PositionsMin = new Vector3D(0.0, 0.0, 0.5),
                    PositionsMax = new Vector3D(Width - 1, Height - 1, 4)
                };

                double ymaxval = 20;
                double ysteps = ymaxval/920.0;
                Woim3DLimits bodyLimits = new Woim3DLimits()
                {

                    AccelerationMin = new Vector3D(-0.25, -0.25, -ysteps),
                    AccelerationMax = new Vector3D(0.25, 0.25, ysteps),

                    VelocityMin = new Vector3D(-1.1, -1.1, -3*ysteps),
                    VelocityMax = new Vector3D(1.1, 1.1, 3*ysteps),

                    PositionsMin = new Vector3D(0.0, 0.0, 0.9),
                    PositionsMax = new Vector3D(Width - 1, Height - 1, ymaxval)
                };

                //Create the woim
                _woim = new(new Random(), 100, 0, bodyLimits, headLimits);


            }

            //Sets all values to white, with no transparency ( I.e. [255][255][255]... )
            for (int i = 0; i < bytesinbuffer; i++) { _buffer[i] = 255; }

            // Update the woim ( grow, move )
            _woim.update();

            //Draw a sprite at each poisition of the 
            Sprite sprite = _spriteManager.getSprite(0);

            //Extract positions and sort them
            //var coordinates = _woim.Segments.Select(o => o.Position);
            var coordinates = _woim.Segments.Skip(1).Select(o => o.Position).OrderByDescending(o => o.Z);

            double centerX = Width / 2.0;
            double centerY = Height / 2.0;

            foreach (Vector3D coordinate in coordinates)
            {
                int x = (int)(centerX + (coordinate.X - centerX) / (1 + 0.2 * coordinate.Z));
                int y = (int)(centerY + (coordinate.Y - centerY) / (1 + 0.2 * coordinate.Z));


                double scale = (0.3 / coordinate.Z);

                _spriteManager.DrawSprite(sprite, x, y, scale);

                
            }


            //Return buffer ( reference value )
            return _buffer;
        }
    }
}


