using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WPFPixelCanvas.Canvas.Models.Sprites
{
    //Holds image data and stats 
    public class Sprite
    {
        //## Private fields
        Tuple<byte, byte, byte> _transparentColor { get; set; }

        //## Constructor(s)
        public Sprite(Tuple<byte,byte,byte> transparentColor)
        {
            _transparentColor = transparentColor;
        }

        //## Public interface
        public void loadImage(string path, int bytesPerPixel)
        {
            //Sanitize input ( ensure the file exists )
            if (path == null) { throw new ArgumentException($"The path argument has no value"); }
            if (!File.Exists(path)) { throw new ArgumentException($"The path '{path}' does not exist"); }

            //Try to load
            BitmapImage bitmap = new(new Uri(path,UriKind.Relative));
            
            Width = (int) bitmap.PixelWidth;
            Height = (int) bitmap.PixelHeight;

            //Check the format to figure out number of bytes per pixel
            int bytesPerPixelInBitmap = bitmap.Format.BitsPerPixel >> 3;
            
            //Reserve space for image data
            int stride = calculateStride(bytesPerPixelInBitmap, Width); // Stride of incoming image
            byte[] inputBuffer = new byte[Height * stride];
            bitmap.CopyPixels(inputBuffer, stride, 0);

            Buffer = new byte[Height * bytesPerPixel * Width];

            //Convert 
            convertImageDataToBufferData(bitmap.Format, inputBuffer, Width, Height);

        }
        private void convertImageDataToBufferData(PixelFormat format, byte[] img, int width, int height)
        {
            int targetIndex = 0;   
            if (format == PixelFormats.Bgra32 || format == PixelFormats.Pbgra32) { for (int i = 0; i < img.Length; i += 4) { storePixelData(targetIndex, img[i + 2], img[i + 1], img[i + 0], img[i + 3]); targetIndex += 4; } return; }
            if (format == PixelFormats.Bgr32) { for (int i = 0; i < img.Length; i += 4) { storePixelData(targetIndex, img[i + 2], img[i + 1], img[i + 0], 255); targetIndex += 4; } return; }
            if (format == PixelFormats.Bgr24) { for (int i = 0; i < img.Length; i += 3) { storePixelData(targetIndex, img[i + 2], img[i + 1], img[i + 0], 255); targetIndex += 4; } return; }
            if (format == PixelFormats.Rgb24) { for (int i = 0; i < img.Length; i += 3) { storePixelData(targetIndex, img[i + 0], img[i + 1], img[i + 2], 255); targetIndex += 4; } return; }

            throw new NotSupportedException("Data format of image not handled.");
            
        }

        private void storePixelData(int pos, byte r, byte g, byte b, byte a)
        {
            Buffer[pos + 0] = b;
            Buffer[pos + 1] = g;
            Buffer[pos + 2] = r;

            bool doNotdisplayPixel = _transparentColor.Item1 == r && _transparentColor.Item2 == g && _transparentColor.Item3 == b;
            if (doNotdisplayPixel) { Buffer[pos + 3] = 0;}
            else { Buffer[pos + 3] = a;}
        }


        // Calculates stride. Ensures every image row starts on 32-bit boundary ( 4 bytes )
        private int calculateStride(int bytesPerPixel, int imageWidth)
        {
            int bytes = imageWidth * bytesPerPixel; // E.g. 393 * 3 = 1179
            int integerPart = bytes / 4;            // E.g. 1179 / 4 = 294  ( 1179/4=294.75)
            int remainder = bytes % 4;              // E.g. 1179 % 4 = 3    ( .75 * 4 = 3 )

            // Calculate the stride
            int stride = integerPart * 4;           
            if(remainder>0) { stride += 4; }        // If there was a remainder, the current
                                                    // stride value is too small. We are missing
                                                    // 1,2 or 3 bytes. We need to be on a 4-byte boundary
                                                    // so we add 4 bytes.

            return stride;                
        }

        //## Public properties
        public byte[] Buffer { get; set; }
        public string Path { get; private set; } = "";
        public int Width { get; private set; } = 0;
        public int Height { get; private set; } = 0;
        public int Stride { get; private set; } = 0;

        //## Private helpers
    }
}
