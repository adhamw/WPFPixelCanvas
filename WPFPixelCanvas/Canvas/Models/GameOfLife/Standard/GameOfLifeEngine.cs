using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPixelCanvas.Canvas.Models.GameOfLife.Standard
{
    /// <summary>
    ///  Implements Game Of Life simulation
    ///  Rules:
    ///  -- A cell survives if: two or three live neighbours
    ///  -- A cell is born if: three live neighbours
    ///  -- Any other cell dies
    /// </summary>

    public class GameOfLifeEngine
    {
        //Local fields
        private bool[,] _map;
        private Random _randomSource { get; set; }
        private int Width { get; set; }
        private int Height { get; set; }
        private int CellCount { get; set; }

        //Construct GameOfLife playingfield
        public GameOfLifeEngine(int width, int height, int  probability)
        {
            //Initalize
            _randomSource = new Random();
            Map = new bool[width, height];   // Set the size of the map

            //Register map dimensions
            Height = height;
            Width = width;
            
            // Populate map with inhabitants
            int all = 0;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++) { _map[x, y] = _randomSource.Next(100) < probability; }
            }

        }
        //Public properties
        public bool[,] Map { get { return _map; } private set { _map = value; } }

        //Public interface
        public void NextGeneration()
        {
            int xmax = Width - 1;
            int ymax = Height - 1;
            
            for(int x=0;x<Width;x++)
            {
                for(int y=0;y<Height;y++)
                {
                    int livecellcount = 0;
                    //Count number of live cells in the surroundings
                    //Above and below
                    if (y > 0) { if (_map[x , y - 1]) { livecellcount++; } }            // Right above current cell
                    if (y < ymax) { if (_map[x , y + 1]) { livecellcount++; } }         // Right below current cell

                    // Left side
                    if (x > 0) 
                    {
                        if (_map[x - 1, y]) { livecellcount++; }                        // Left of curren cell
                        if (y > 0) { if (_map[x - 1, y - 1]) { livecellcount++; } }     // Upper left corner
                        if (y < ymax) { if (_map[x - 1, y + 1]) { livecellcount++; } }  // Lower left corner
                    }
                    //Right side
                    if (x < xmax)
                    {
                        if (_map[x + 1, y]) { livecellcount++; }
                        if (y > 0) { if (_map[x + 1, y - 1]) { livecellcount++; } }
                        if (y < ymax) {if(_map[x + 1, y + 1]) { livecellcount++; } }
                    }

                    //Check if cell survives/gets born
                    //bool survived = Map[x, y] && (livecellcount == 2 || livecellcount == 3);
                    //bool isBorn = !Map[x, y] && (livecellcount == 3);

                    //Alternative settings -- very organic result
                    bool survived = Map[x, y] && (livecellcount == 1 || livecellcount == 2);
                    bool isBorn = !Map[x, y] && (livecellcount == 3);

                    Map[x, y] = survived || isBorn;
                }
            }
        }
    }
}
