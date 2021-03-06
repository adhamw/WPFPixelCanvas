using WPFPixelCanvas.common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace WPFPixelCanvas.Canvas.Models
{
    /// <summary>
    /// Creates and manages a canvas that allows fast pixel operations
    /// </summary>
    public class CanvasManager : NotificationPropertyBase
    {
        //## Private fields/properties
        private int _bytesPerPixel { get; set; }
        private int _bytesPerLine { get; set; }
        private ICanvasPlotter _plotter { get; set; }   // Custom object that defines plot function
        private bool _enabled { get; set; } = false;    // Flag that decides whether we should update image buffer or not ( by running plot function )
        private bool _runOnce { get; set; } = false;    // Flag that decides whether we should paint once or untill Enabledflag is lowered 
        private long _refreshCounter { get; set; } = 0;   // Counts up between buffer updates

        //## Constructor
        public CanvasManager(ICanvasPlotter plotter)
        {
            //Verify input
            if (plotter == null) { throw new ArgumentException(); } //Check that we weren't handed an empty object

            //Set parameter values
            _plotter = plotter;
            Width = plotter.Width;     // pixels per line. E.g. 800
            Height = plotter.Height;   // lines per image. E.g. 600

            //Define our bitmap
            var pixelformat = PixelFormats.Pbgra32; // I.e. [b][g][r][a]
            Buffer = new WriteableBitmap(Width, Height, 96, 96, pixelformat, null);
            _bytesPerLine = Buffer.BackBufferStride;
            _bytesPerPixel = pixelformat.BitsPerPixel >> 3;

            // Register the paint function  
            // Ensures application keeps repainting our target control (<Image>)
            CompositionTarget.Rendering += DoPaint; //Hooks up event that processes everytime window updates?
        }

        //## Public Interface        

        /// <summary>
        /// Implements update of image buffer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DoPaint(object sender, EventArgs e)
        {
            if (!IsEnabled) { return; }                                         // Skip painting updates if not enabled
            var pixeldata = _plotter.Plot(_bytesPerPixel, _bytesPerLine, _refreshCounter);   // Calls custom paint function            
            _refreshCounter++;
            if (_runOnce) { IsEnabled = false; _runOnce = false; }              // Lowers enable flag to stop painting, if in runonce mode
            Buffer.WritePixels(new Int32Rect(0, 0, Width, Height), pixeldata, _bytesPerPixel * Width, 0);
        }
        public void Stop() { if (IsEnabled) { IsEnabled = false; } } // Lowers IsEnabled flag
        public void Start() { if (!IsEnabled) { _refreshCounter = 0; IsEnabled = true; } } // Raises IsEnabled flag
        public void RunOnce() { if (!IsEnabled) { _refreshCounter = 0; _runOnce = true; IsEnabled = true; } } // Raises IsEnabled flag

        //Public properties
        public int Width { get; private set; }
        public int Height { get; private set; }
        public WriteableBitmap Buffer { get; private set; }
        public bool IsEnabled
        {
            get { return _enabled; }
            set { _enabled = value; OnPropertyChanged(); }
        }

    }
}
