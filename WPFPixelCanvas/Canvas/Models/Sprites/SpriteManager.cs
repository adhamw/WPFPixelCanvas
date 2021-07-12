using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPixelCanvas.Canvas.Models.Sprites
{
    //Class that allows loading up images to 
    //color buffers so we can draw them on our canvas
    public class SpriteManager
    {
        //## Private fields
        List<Sprite> _sprites;
        byte[] _pixelBuffer;
        int _bufferWidth;
        int _bufferHeight;
        int _bufferBytesPerPixel;

        Tuple<byte, byte, byte> _transparentColor;

        //## Constructor(s)
        public SpriteManager(byte[] pixelBuffer, int width, int height, int bytesPerPixel, Tuple<byte,byte,byte> transparentColor)
        {
            _pixelBuffer = pixelBuffer;
            _bufferWidth = width;
            _bufferHeight = height;
            _bufferBytesPerPixel = bytesPerPixel;

            _sprites = new List<Sprite>();
            _transparentColor = transparentColor;
        }

        //## Public interface
        public Sprite AddSprite()
        {
            Sprite newSprite = new(_transparentColor);   // Create sprite container
            _sprites.Add(newSprite);    // Add container to list
            return newSprite;           // Return sprite object ( allows chaining command ) 
        }

        public void DrawSprite(Sprite sprite, int x, int y, double scaling=1.0)
        {
            //Copy every row into pixelbuffer at given
            int pixelBufferStride = _bufferWidth * _bufferBytesPerPixel;
            int pixelBufferIndex = y * pixelBufferStride + x * _bufferBytesPerPixel;
            int spriteBufferIndex = 0;
            int spriteBufferStride = sprite.Width * _bufferBytesPerPixel;


            //Draw sprite

            //Determine height of drawn sprite ( in buffer after scaling)
            int targetHeight = (int)(scaling * sprite.Height);

            int steppingIndex, steppingHeight, steppingStride;
            int scaledIndex, scaledHeight, scaledStride;
            int kdy = 0;

            if (sprite.Height > targetHeight)
            {
                steppingIndex = spriteBufferIndex;   // We step forward in the index of the tallest graphic
                steppingHeight = sprite.Height;
                steppingStride = spriteBufferStride;

                scaledIndex = pixelBufferIndex;      // ..and scale the other index accordingly
                scaledHeight = targetHeight;
                scaledStride = pixelBufferStride;
                kdy = 0;                             // Remainder counter of smallest Height/largest Height
            }
            else
            {
                scaledIndex = spriteBufferIndex;   // We step forward in the index of the tallest graphic
                scaledHeight = sprite.Height;
                scaledStride = spriteBufferStride;

                steppingIndex = pixelBufferIndex;      // ..and scale the other index accordingly
                steppingHeight = targetHeight;
                steppingStride = pixelBufferStride;
                kdy = 0;                             // Remainder counter of smallest Height/largest Height
            }


            //Clip bottom/right side of sprite if sprite spills over to edge
            int distanceToBottomEdge = _bufferHeight - y;
            //int clippedSpriteWidth = distanceToRightEdge < targetHeight ? distanceToRightEdge : sprite.Width;

            int clippedSpriteHeight = distanceToBottomEdge < targetHeight ? distanceToBottomEdge : targetHeight;

            int ypos = y; // Keeping track of the y-pos, in case we need to clip
            for (int row = 0; row < steppingHeight; row++)
            {
                if(ypos>= _bufferHeight) { break; }      // Skip drawing of lines that fall outside the screen
                //if(ypos == 220)
                //{
                //    bool stophere = true;
                //}
                // Stepping pixels in sprite-buffer, calculating associated position in pixel buffer
                if (sprite.Height > targetHeight)
                {
                    DrawScaledHorizontalLine(x,sprite.Buffer, sprite.Width, scaledIndex, steppingIndex, scaling);
                    
                }
                else  // Stepping pixels in pixelBuffer, calculating associated positionin sprite buffer
                { 
                    DrawScaledHorizontalLine(x,sprite.Buffer, sprite.Width, steppingIndex, scaledIndex, scaling);
                    ypos++; // We are stepping in output buffer, so we increas y count for each call
                }

                //Update starting points for indexes into drawbuffer and sprite buffer
                steppingIndex += steppingStride;    // One step forward in stepping index
                kdy += scaledHeight;
                if (kdy > steppingHeight)
                {
                    scaledIndex += scaledStride;
                    if (sprite.Height > targetHeight) { ypos++; } // If we are updating the buffer position here, we count y. ( Happens when we do major stepping in sprite buffer, which happens when sprite.height is > target height )

                   kdy -= steppingHeight;
                }

            }
        }

        #region deletethis
        //public void DrawSprite(Sprite sprite, int x, int y)
        //{
        //    //Copy every row into pixelbuffer at given
        //    int pixelBufferStride = _bufferWidth * _bufferBytesPerPixel;
        //    int pixelBufferIndex = y * pixelBufferStride + x * _bufferBytesPerPixel;
        //    int spriteBufferIndex = 0;
        //    int spriteBufferStride = sprite.Width * _bufferBytesPerPixel;

        //    //Clip bottom/right side of sprite if sprite spills over to edge
        //    int distanceToRightEdge = _bufferWidth - x;
        //    int distanceToBottomEdge = _bufferHeight - y;
        //    int clippedSpriteWidth = distanceToRightEdge < sprite.Width ? distanceToRightEdge : sprite.Width;
        //    int clippedSpriteHeight = distanceToBottomEdge < sprite.Height ? distanceToBottomEdge : sprite.Height;

        //    //Draw sprite
        //    for (int row = 0; row < clippedSpriteHeight; row++)
        //    {
        //        int spriteXPos = spriteBufferIndex; // Starting position in sprite buffer
        //        int bufferXPos = pixelBufferIndex;  // Starting position in pixel buffer

        //        // Draw horizontal line
        //        for (int xx = 0; xx < clippedSpriteWidth; xx++)
        //        {
        //            //Transparency
        //            byte transp = sprite.Buffer[spriteXPos + 3];
        //            if (transp != 0)
        //            {
        //                _pixelBuffer[bufferXPos + 0] = sprite.Buffer[spriteXPos + 0];
        //                _pixelBuffer[bufferXPos + 1] = sprite.Buffer[spriteXPos + 1];
        //                _pixelBuffer[bufferXPos + 2] = sprite.Buffer[spriteXPos + 2];
        //                _pixelBuffer[bufferXPos + 3] = transp;
        //            }
        //            spriteXPos += 4;
        //            bufferXPos += 4;
        //        }

        //        //Update starting points for indexes into drawbuffer and sprite buffer
        //        pixelBufferIndex += pixelBufferStride;
        //        spriteBufferIndex += spriteBufferStride;
        //    }

        //}

        //public void DrawSpriteWorking(Sprite sprite, int x, int y, double scaling)
        //{
        //    //Copy every row into pixelbuffer at given
        //    int pixelBufferStride = _bufferWidth * _bufferBytesPerPixel;
        //    int pixelBufferIndex = y * pixelBufferStride + x * _bufferBytesPerPixel;
        //    int spriteBufferIndex = 0;
        //    int spriteBufferStride = sprite.Width * _bufferBytesPerPixel;

        //    //Clip bottom/right side of sprite if sprite spills over to edge
        //    int distanceToRightEdge = _bufferWidth - x;
        //    int distanceToBottomEdge = _bufferHeight - y;
        //    //int clippedSpriteWidth = distanceToRightEdge < sprite.Width ? distanceToRightEdge : sprite.Width;
        //    //int clippedSpriteHeight = distanceToBottomEdge < sprite.Height ? distanceToBottomEdge : sprite.Height;

        //    //Draw sprite

        //    //Determine height of drawn sprite ( in buffer after scaling)
        //    int targetHeight = (int)(scaling * sprite.Height);

        //    if (sprite.Height > targetHeight)
        //    {
        //        int steppingIndex = spriteBufferIndex;   // We step forward in the index of the tallest graphic
        //        int steppingHeight = sprite.Height;
        //        int steppingStride = spriteBufferStride;

        //        int scaledIndex = pixelBufferIndex;      // ..and scale the other index accordingly
        //        int scaledHeight = targetHeight;
        //        int scaledStride = pixelBufferStride;
        //        int kdy = 0;                             // Remainder counter of smallest Height/largest Height

        //        for (int row = 0; row < steppingHeight; row++)
        //        {
        //            DrawScaledHorizontalLine(x,sprite.Buffer, sprite.Width, scaledIndex, steppingIndex, scaling);

        //            //Update starting points for indexes into drawbuffer and sprite buffer
        //            steppingIndex += steppingStride;    // One step forward in stepping index
        //            kdy += scaledHeight;
        //            if (kdy > steppingHeight)
        //            {
        //                scaledIndex += scaledStride;
        //                kdy -= steppingHeight;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        int scaledIndex = spriteBufferIndex;   // We step forward in the index of the tallest graphic
        //        int scaledHeight = sprite.Height;
        //        int scaledStride = spriteBufferStride;

        //        int steppingIndex = pixelBufferIndex;      // ..and scale the other index accordingly
        //        int steppingHeight = targetHeight;
        //        int steppingStride = pixelBufferStride;
        //        int kdy = 0;                             // Remainder counter of smallest Height/largest Height

        //        for (int row = 0; row < steppingHeight; row++)
        //        {
        //            DrawScaledHorizontalLine(x,sprite.Buffer, sprite.Width, steppingIndex, scaledIndex, scaling);

        //            //Update starting points for indexes into drawbuffer and sprite buffer
        //            steppingIndex += steppingStride;    // One step forward in stepping index
        //            kdy += scaledHeight;
        //            if (kdy > steppingHeight)
        //            {
        //                scaledIndex += scaledStride;
        //                kdy -= steppingHeight;
        //            }
        //        }
        //    }

        //}
        //private void DrawScaledHorizontalLineWorking(byte[] spriteBuffer, int spriteWidth, int pixelBufferIndex, int spriteBufferIndex, double scaling)
        //{

        //    //int distanceToRightEdge = _bufferWidth - x;

        //    //Determine width of drawn sprite ( in buffer after scaling)
        //    int targetWidth = (int) (scaling * spriteWidth);

        //    // If we want to scale down, stepping in sprite index,
        //    // while calculating associated buffer index
        //    if(spriteWidth>targetWidth)
        //    {
        //        int steppingWidth = spriteWidth;
        //        int scaledWidth = targetWidth;

        //        int steppingindex = spriteBufferIndex;   // We step forward in the index of the widest graphic
        //        int scaledindex = pixelBufferIndex;      // ..and scale the other index accordingly
        //        int kdx = 0;                             // Remainder counter of smallest Width/largest Width

        //        // Draw horizontal line
        //        for (int xx = 0; xx < steppingWidth; xx++)
        //        {
        //            //Transparency
        //            byte transp = spriteBuffer[steppingindex + 3];
        //            if (transp != 0)
        //            {
        //                _pixelBuffer[scaledindex + 0] = spriteBuffer[steppingindex + 0];
        //                _pixelBuffer[scaledindex + 1] = spriteBuffer[steppingindex + 1];
        //                _pixelBuffer[scaledindex + 2] = spriteBuffer[steppingindex + 2];
        //                _pixelBuffer[scaledindex + 3] = transp;
        //            }

        //            //Update the indexes
        //            steppingindex += 4; // On step forward in stepping index
        //            kdx += scaledWidth;
        //            if(kdx>steppingWidth)
        //            {
        //                scaledindex += 4;
        //                kdx -= steppingWidth;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        int steppingWidth = targetWidth;
        //        int scaledWidth = spriteWidth;

        //        int steppingindex = pixelBufferIndex;   // We step forward in the index of the widest graphic 
        //        int scaledindex = spriteBufferIndex;    // ..and scale the other index accordingly
        //        int kdx = 0;                            // Remainder counter of smallest Width/largest Width

        //        // Draw horizontal line
        //        for (int xx = 0; xx < steppingWidth; xx++)
        //        {
        //            //Transparency
        //            byte transp = spriteBuffer[scaledindex + 3];
        //            if (transp != 0)
        //            {
        //                _pixelBuffer[steppingindex + 0] = spriteBuffer[scaledindex + 0];
        //                _pixelBuffer[steppingindex + 1] = spriteBuffer[scaledindex + 1];
        //                _pixelBuffer[steppingindex + 2] = spriteBuffer[scaledindex + 2];
        //                _pixelBuffer[steppingindex + 3] = transp;
        //            }

        //            //Update the indexes
        //            steppingindex += 4; // On step forward in stepping index
        //            kdx += scaledWidth;
        //            if (kdx > steppingWidth)
        //            {
        //                scaledindex += 4;
        //                kdx -= steppingWidth;
        //            }
        //        }

        //    }


        //}
        #endregion

        public void DrawSprite(int spriteIndex, int x, int y, double scale=1.0)
        {
            //Determine sprite 
            Sprite sprite = getSprite(spriteIndex);

            //Draw the sprite
            DrawSprite(sprite,x,y,scale);
        }

        public Sprite getSprite(int spriteIndex)
        {
            if (spriteIndex >= _pixelBuffer.Length || spriteIndex < 0) { throw new ArgumentException("Invalid sprite index"); }
            Sprite sprite = _sprites[spriteIndex];
            return sprite;
        }


        //## Public properties
        public IReadOnlyList<Sprite> Sprites { get { return _sprites.AsReadOnly(); } }


        //## Private helpers
        private void DrawScaledHorizontalLine(int x, byte[] spriteBuffer, int spriteWidth, int pixelBufferIndex, int spriteBufferIndex, double scaling)
        {
            // Declare our stepping/scaling variables
            // With stepping, I simply mean counting up an index by one ( step ).
            int steppingWidth, steppingindex;
            int scaledWidth, scaledindex;
            int kdx = 0;

            //Determine width of drawn sprite ( in buffer after scaling)
            int targetWidth = (int)(scaling * spriteWidth);

            // Set parameter values, depending on whether sprite is smaller or larger
            // than the scaled image drawn to the buffer. We step in the index of
            // whichever of the two that is larger and calculate the associated
            // index for the other.
            //
            // E.g. if original sprite is wider than the target ( downscaling )
            // we step through indexes in the sprite buffer, and calculate WHEN TO step
            // in the out buffer.

            // If downscaling
            if (spriteWidth > targetWidth)
            {
                steppingindex = spriteBufferIndex;   // We step the index of the sprite buffer
                steppingWidth = spriteWidth;         // The number of times to step through 

                scaledindex = pixelBufferIndex;      // The scaled buffer index ( used to draw the sprite in the output buffer )
                scaledWidth = targetWidth;           // The number of times to step through
                kdx = 0;                             // Numerator counter of relationship scaleWidth/steppingWidth
            }
            else // If upscaling ( or 1-1 scaling )
            {
                steppingindex = pixelBufferIndex;   // We step the index of the output buffer
                steppingWidth = targetWidth;        // The number of times to step through

                scaledWidth = spriteWidth;          // The scaled sprite buffer index 
                scaledindex = spriteBufferIndex;    // The number of times to step through
                kdx = 0;                            // Numerator counter of relationship scaleWidth/steppingWidth
            }

            // Draw horizontal line
            int xpos = x;
            for (int xx = 0; xx < steppingWidth; xx++)
            {
                if (xpos >= _bufferWidth) { break; }      // Stop drawing of lines that fall outside the screen to the right

                // Downscaling sprite
                if (spriteWidth > targetWidth)
                {
                    byte transp = spriteBuffer[steppingindex + 3];  // Check the alpha channel
                    if (transp != 0)                                // Skip drawing if alpha = 100% transparent
                    {
                        // Copy data from spriteBuffer to out buffer
                        _pixelBuffer[scaledindex + 0] = spriteBuffer[steppingindex + 0];
                        _pixelBuffer[scaledindex + 1] = spriteBuffer[steppingindex + 1];
                        _pixelBuffer[scaledindex + 2] = spriteBuffer[steppingindex + 2];
                        _pixelBuffer[scaledindex + 3] = transp;
                    }

                    // We are stepping through idexes in the sprite buffer
                    // the x-position in the output buffer does not necessarily increase
                    // Thus we must check in the code below
                }
                else // Upscaling sprite
                {
                    //Transparency
                    byte transp = spriteBuffer[scaledindex + 3];    // Check the alpha channel
                    if (transp != 0)                                // Skip drawing if alpha = 100% transparent
                    {
                        // Copy data from spriteBuffer to out buffer
                        _pixelBuffer[steppingindex + 0] = spriteBuffer[scaledindex + 0];
                        _pixelBuffer[steppingindex + 1] = spriteBuffer[scaledindex + 1];
                        _pixelBuffer[steppingindex + 2] = spriteBuffer[scaledindex + 2];
                        _pixelBuffer[scaledindex + 3] = transp;
                    }

                    // We are stepping in output buffer
                    // ( literally: stepping through the x-values),
                    // so x should increase for every call
                    xpos++;
                }

                //Update the indexes
                steppingindex += 4;         // One step forward in stepping index

                // Figure out whether we should step in the scaling index 
                // by looking at scaledWidth/SteppingWidth ( <=1 )
                kdx += scaledWidth;                             // Counting up the numerator ( above the fraction bar )
                if (kdx > steppingWidth)                        // If sum is more than the denominator ( below the fraction bar ) 
                {
                    scaledindex += 4;                           // Our fraction is 1+something. Meaning we can do one more step in the scaled index
                    kdx -= steppingWidth;                       // Subtracting a "whole" unit from our fraction to bring the fraction below '1'

                    if (spriteWidth > targetWidth) { xpos++; }   // When stepping in sprite buffer index, this code triggers when we should step in output buffer ( and thus x should progress )
                }
            }
        }
    }
}
