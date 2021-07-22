using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFPixelCanvas.Canvas.Models.Boids3D;
using WPFPixelCanvas.Canvas.Models.Sprites;

namespace WPFPixelCanvas.Canvas.Models.Bobs
{
    public class BobsManager
    {
        //##  Private fields
        private Queue<Vector3D> _previousPositions;
        private byte[] _pixelBuffer { get; set; }
        private int _width { get; set; }
        private int _height { get; set; }
        private int _bytesPerPixel { get; set; }
        private int _linePaddingBytes { get; set; }
        private int _bytesPerLine { get; set; }
        private SpriteManager _spriteManager { get; set; }
        private int _size { get; set; }

        //##  Constructor(s)
        public BobsManager(int size, byte[] pixelBuffer, int width, int height, int bytesPerPixel, int linePaddingBytes, SpriteManager spriteManager)
        {
            //Sanitize input
            if (spriteManager.Sprites.Count < 1) { throw new ArgumentException("Sprite manager must be initialized with a sprite"); }
            if (size< 1) { throw new ArgumentException("Bobs-chain must have at least 1 segment"); }

            //Storing parameters
            _width = width;
            _height = height;
            _bytesPerPixel = bytesPerPixel;
            _linePaddingBytes = linePaddingBytes;
            _bytesPerLine = _width * _bytesPerPixel + _linePaddingBytes;
            _pixelBuffer = pixelBuffer;
            _spriteManager = spriteManager;

            _size = size;
            _previousPositions = new Queue<Vector3D>(size);  //Holds positional history of bobs

        }


        //##  Public interface
        public void Update(Vector3D currentPosition)
        {
            //Add current position to our queue
            _previousPositions.Enqueue(currentPosition);
            if (_previousPositions.Count > _size) _previousPositions.Dequeue();

            int centerX = _width / 2;
            int centerY = _height / 2;

            //Sort bobs positions
            var positions = _previousPositions.OrderByDescending(o => o.Z);
            foreach(Vector3D position in _previousPositions)
            {
                int plotX = (int)(centerX + (position.X - centerX) / (1 + 0.5 * position.Z));
                int plotY = (int)(centerY + (position.Y - centerY) / (1 + 0.5 * position.Z));
                double scale = (0.2 / position.Z);

                _spriteManager.DrawSprite(0, plotX, plotY, scale);
            }

        }

        //##  Public properties
        //##  Private helpers
    }


}
