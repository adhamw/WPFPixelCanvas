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
    public class CCanvasManager : CNotificationPropertyBase
    {
        //## Private fields/properties
        private int _BytesPerPixel { get; set; } = 4;       // RGBA = 4 pixels
        private ICanvasPlotter _Plotter { get; set; }   // Custom object that defines plot function
        private bool _Enabled { get; set; } = false;    // Flag that decides whether we should update image buffer or not ( by running plot function )
        private bool _RunOnce { get; set; } = false;    // Flag that decides whether we should paint once or untill Enabledflag is lowered 
        private long _RefreshCounter { get; set; } = 0;   // Counts up between buffer updates

        //## Constructor
        public CCanvasManager(ICanvasPlotter plotter)
        {
            //Verify input
            if (plotter == null) { throw new ArgumentException(); } //Check that we weren't handed an empty object

            //Set parameter values
            _Plotter = plotter;
            Width = plotter.Width;     // pixels per line. E.g. 800
            Height = plotter.Height;   // lines per image. E.g. 600

            //Define our bitmap
            Buffer = new WriteableBitmap(Width, Height, 96, 96, PixelFormats.Pbgra32, null);

            // Register the paint function  
            // Ensures application keeps repainting our target control (<Image>)
            CompositionTarget.Rendering += doPaint; //Hooks up event that processes everytime window updates?
        }

        //## Public Interface        

        /// <summary>
        /// Implements update of image buffer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void doPaint(object sender, EventArgs e)
        {
            if (!IsEnabled) { return; }                                         // Skip painting updates if not enabled

            Buffer.Lock();                                                      // Buffer ready for fast manipulation
            _Plotter.plot(Buffer.BackBuffer, _BytesPerPixel,_RefreshCounter);   // Calls custom paint function            
            _RefreshCounter++;
            if (_RunOnce) { IsEnabled = false; _RunOnce = false; }              // Lowers enable flag to stop painting, if in runonce mode
            Buffer.AddDirtyRect(new Int32Rect(0, 0, Width, Height));            // Ensures we update the whole <Image> area
            Buffer.Unlock();                                                    // Image manipulation done, release buffer
        }
        public void stop() { if (IsEnabled) { IsEnabled = false; } } // Lowers IsEnabled flag
        public void start() { if (!IsEnabled) { _RefreshCounter = 0; IsEnabled = true; } } // Raises IsEnabled flag
        public void runOnce() { if (!IsEnabled) { _RefreshCounter = 0; _RunOnce = true; IsEnabled = true; } } // Raises IsEnabled flag

        //Public properties
        public int Width { get; private set; }
        public int Height { get; private set; }
        public WriteableBitmap Buffer { get; private set; }
        public bool IsEnabled
        {
            get { return _Enabled; }
            set { _Enabled = value; OnPropertyChanged(); }
        }

    }
}
