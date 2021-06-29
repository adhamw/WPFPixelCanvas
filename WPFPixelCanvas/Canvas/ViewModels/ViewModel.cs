using WPFPixelCanvas.Canvas.Models;
using WPFPixelCanvas.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPixelCanvas.Canvas.ViewModels
{
    public class ViewModel : CNotificationPropertyBase
    {
        //Local fields/properties

        //Constructor
        public ViewModel(int width, int height)
        {
            //Various plot component implementations
            //CPattern1 plotter = new CPattern1(width, height);   // Makes color patterns based on x,y position
            //CPattern2 plotter = new CPattern2(width, height);   // Makes wavy patterns 
            CPattern3 plotter = new CPattern3(width, height);     // Makes random dots

            Canvas = new CCanvasManager(plotter);
            //CompositionTarget.Rendering += Canvas.doPaint; //Hooks up event that processes everytime window updates?

            StartCommand = new CRelayCommand(o => startPlotting(), o => !Canvas.IsEnabled);
            StopCommand = new CRelayCommand(o => stopPlotting(), o => Canvas.IsEnabled);
            RunOnceCommand = new CRelayCommand(o => plotOnce(), o => !Canvas.IsEnabled);
        }

        //Public properties
        public CCanvasManager Canvas { get; set; }

        //Commands
        public CRelayCommand StartCommand { get; set; }
        public CRelayCommand StopCommand { get; set; }
        public CRelayCommand RunOnceCommand { get; set; }

        //Command behavior
        private void stopPlotting() { Canvas.stop(); }
        private void startPlotting() { Canvas.start(); }
        private void plotOnce() { Canvas.runOnce(); }

    }
}
