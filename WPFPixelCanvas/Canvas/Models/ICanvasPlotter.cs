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
        void plot(IntPtr buffer, int bytesperpixel, long refreshcount);

    }
}
