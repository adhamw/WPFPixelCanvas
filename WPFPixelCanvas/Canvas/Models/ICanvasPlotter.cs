using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPixelCanvas.Canvas.Models
{
    public interface ICanvasPlotter
    {
        //Properties
        int Width { get; }
        int Height { get; }

        //Interface
        //        void Plot(IntPtr buffer, int bytesperpixel, long refreshcount);
        byte[] Plot(int bytesperpixel, int bytesperline, long refreshcount);

    }
}
