using global::WPFPixelCanvas.Canvas.Models.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPixelCanvas.Canvas.Models.PlotModules
{
    public class Pattern_Sprites : ICanvasPlotter
    {
        //Local fields
        private byte[] _buffer { get; set; }
        private SpriteManager _spriteManager { get; set; }

        //Constructor
        public Pattern_Sprites(int width, int height)
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
            //Initialize buffer if not set
            int bytesinbuffer = bytesperline * Height;  // Number of bytes needed for the whole buffer
            if (_buffer == null)
            {
                _buffer = new byte[bytesinbuffer];
                
                _spriteManager = new SpriteManager(_buffer, Width, Height, bytesperpixel, new Tuple<byte, byte, byte>(250, 180, 250));
                _spriteManager.AddSprite().loadImage(@".\SpriteData.\glassball2_128x128.bmp", bytesperpixel);
            }

            //Sets all values to white, with no transparency ( I.e. [255][255][255]... )
            for (int i = 0; i < bytesinbuffer; i++) { _buffer[i] = 255; }

            //Draw the sprite
            Sprite sprite = _spriteManager.getSprite(0);
            int centerx = Width / 2 - sprite.Width / 2;
            int centery = (Height / 2 - sprite.Height / 2);
            _spriteManager.DrawSprite(sprite, centerx, centery, 0.6);
            _spriteManager.DrawSprite(sprite, centerx + 10, centery + 10, 1.4);
            _spriteManager.DrawSprite(sprite, centerx + 10, centery + 10, 6.0);

            //Return buffer ( reference value )
            return _buffer;
        }
    }
}


