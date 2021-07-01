using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFPixelCanvas.Canvas.Models.GameOfLife;
using WPFPixelCanvas.Canvas.Models.GameOfLife.OOP;
using WPFPixelCanvas.Canvas.Models.GameOfLife.OOP.GameOfLifeCells;

namespace WPFPixelCanvas.Canvas.Models.PlotModules
{
    public class Pattern_OOPGameOfLife_Color : ICanvasPlotter
    {
        //Local fields
        private byte[] _buffer { get; set; }
        private GameOfLifeEngineOOP _engine { get; set; }
        private Tuple<byte, byte, byte>[] _palette { get; set; }
        private Random _randomSource { get; set; }

        //Constructor
        public Pattern_OOPGameOfLife_Color(int width, int height)
        {
            Width = width;      // Defines width of plot area ( e.g. 800 pixels )
            Height = height;    // Defines height of plot area ( e.g. 600 pixels )

            //Prepare a cell factory that produces complex GOL cells
            int tanglecount = 15;
            GameOfLifeCellFactory cellfactory = new GameOfLifeCellFactory(GameOfLifeCellTypes.Complex, tanglecount);
            _engine = new GameOfLifeEngineOOP(width, height, 5, cellfactory);

            //Create a palette
            _randomSource = new Random();
            _palette = new Tuple<byte, byte, byte>[tanglecount];
            for(int i=0;i<tanglecount;i++)
            {
                _palette[i] = new Tuple<byte,byte,byte>((byte)_randomSource.Next(255), (byte)_randomSource.Next(255), (byte)_randomSource.Next(255));
            }


        }

        //Public properties
        public int Width { get; private set; }
        public int Height { get; private set; }

        //Public interface
        public byte[] Plot(int bytesperpixel, int bytesperline, long refreshcounter = 0)
        {
            int width = Width;                          // Storing widht in local variable ( caching for speed ) 
            int height = Height;                        // ditto

            //Initialize buffer if not set
            int bytesinbuffer = bytesperline * height;  // Number of bytes needed for the whole buffer
            if (_buffer == null) { _buffer = new byte[bytesinbuffer]; }

            //Sets all values to white, with no transparency ( I.e. [255][255][255]... )
            for (int i = 0; i < bytesinbuffer; i++) { _buffer[i] = 255; }

            int pos = 0;    // Buffer index

            //Progress simulation
            _engine.NextGeneration();

            //Run through all lines in image
            for (int y = 0; y < height; y++)
            {
                //Run through every pixel on a line
                for (int x = 0; x < width; x++)
                {
                    //Aquiring cell stats
                    bool isalive = _engine.Map[x, y].IsAlive;           // Is it currently survive
                    int Age = _engine.Map[x, y].Age;                    // How long did this one survive?
                    int tangleid = _engine.Map[x, y].TangleId;          // Identifies what tangle the cell belongs to

                    // ** Color calculation code **
                    var basecolor = _palette[tangleid];                 // Each tangle has their own assigned color
                    double rval, gval, bval;
                    if(!isalive) { (rval, gval, bval) = (0, 0, 0); }    // If cell not alive, leave black
                    else
                    {
                        rval = basecolor.Item1 *(1 - 0.5 * Math.Sin( Age * 0.01)); // The trigonemetric adjustment is just to shift colors
                        gval = basecolor.Item2 *(1 - 0.5 * Math.Cos( Age * 0.01)); // they do not affect the simulation state 
                        bval = basecolor.Item3 *(1 - 0.5 * Math.Sin( Age * 0.01)) ;
                    }

                    // Storing color values in buffer
                    _buffer[pos + 0] = (byte)bval;   //Blue component
                    _buffer[pos + 1] = (byte)gval;   //Green component 
                    _buffer[pos + 2] = (byte)rval;   //Red component 
                    _buffer[pos + 3] = 255;          //Alpha component

                    pos += bytesperpixel;  // Moving buffer pointer forward
                }
            }

            //Return buffer ( reference value )
            return _buffer;
        }
    }
}

