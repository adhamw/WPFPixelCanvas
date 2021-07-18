﻿using WPFPixelCanvas.Canvas.Models;
using WPFPixelCanvas.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFPixelCanvas.Canvas.Models.PlotModules;
using WPFPixelCanvas.Canvas.Models.ASelectionOfPlotModules;

namespace WPFPixelCanvas.Canvas.ViewModels
{
    public class ViewModel : NotificationPropertyBase
    {
        //Local fields/properties

        //Constructor
        public ViewModel(int width, int height)
        {
            //## Various plot component implementations
            //## 

            //Pattern_FillBlue plotter = new(width, height);            // Fills screen with blue

            //Pattern_SimpleXYPatterns plotter = new(width, height);    // Makes color patterns based on x,y position
            //Pattern_PlasmaPattern plotter = new(width, height);       // Makes wavy patterns 
            //Pattern_RandomDots plotter = new(width, height);          // Makes random dots

            //Pattern_GameOfLife plotter = new(width, height);          // Single class implementation of GOL
            //Pattern_OOPGameOfLife plotter = new(width, height);       // OOP implementation of GOL
            //Pattern_OOPGameOfLife_Color plotter = new(width, height); // Same as above, but takes advantage of OOP to introduce color

            //Pattern_Boids plotter = new(width, height);               // Implements a simple boids simulator
            //Pattern_BasicFire plotter = new Pattern_BasicFire(width, height);
            //Pattern_Lines  plotter = new Pattern_Lines(width, height);
            //Pattern_LineFire plotter = new Pattern_LineFire(width, height);

            //Pattern_Sprites  plotter = new Pattern_Sprites(width, height);

            //Pattern_Boids3D plotter = new Pattern_Boids3D(width, height);
            //Pattern_Woims plotter = new Pattern_Woims(width, height);
            Pattern_Boids3DSprites plotter = new Pattern_Boids3DSprites(width, height);


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
