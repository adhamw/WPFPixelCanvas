using WPFPixelCanvas.Canvas.Models;
using WPFPixelCanvas.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPixelCanvas.Canvas.ViewModels
{
    public class ViewModel : NotificationPropertyBase
    {
        //Local fields/properties

        //Constructor
        public ViewModel(int width, int height)
        {

            //Various plot component implementations
            //Pattern1 plotter = new(width, height);            // Makes color patterns based on x,y position
            //Pattern2 plotter = new(width, height);            // Makes wavy patterns 
            //Pattern3 plotter = new(width, height);            // Makes random dots
            //Pattern_FillBlue plotter = new(width, height);    // Makes random dots
            PatternTemplate plotter = new(width, height);    // Makes random dots

            Canvas = new CanvasManager(plotter);
            StartCommand = new RelayCommand(o => StartPlotting(), o => !Canvas.IsEnabled);
            StopCommand = new RelayCommand(o => StopPlotting(), o => Canvas.IsEnabled);
            RunOnceCommand = new RelayCommand(o => PlotOnce(), o => !Canvas.IsEnabled);
        }

        //Public properties
        public CanvasManager Canvas { get; set; }

        //Commands
        public RelayCommand StartCommand { get; set; }
        public RelayCommand StopCommand { get; set; }
        public RelayCommand RunOnceCommand { get; set; }

        //Command behavior
        private void StopPlotting() { Canvas.Stop(); }
        private void StartPlotting() { Canvas.Start(); }
        private void PlotOnce() { Canvas.RunOnce(); }

    }
}
