using global::WPFPixelCanvas.Canvas.Models.GameOfLife.OOP.GameOfLifeCells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WPFPixelCanvas.Canvas.Models.GameOfLife.OOP
{
    public class GameOfLifeEngineOOP
    {
        //Local fields
        private IGameOfLifeCell[,] _map;
        private Random _randomSource { get; set; }
        private GameOfLifeCellFactory _cellFactory { get; set; }
        private int _width { get; set; }
        private int _height { get; set; }


        //Construct GameOfLife playingfield
        public GameOfLifeEngineOOP(int width, int height, int probability, GameOfLifeCellFactory cellfactory)
        {
            //Initalize
            _randomSource = new Random();
            _cellFactory = cellfactory;
            Map = new IGameOfLifeCell[width, height];   // Set the size of the map


            //Register map dimensions
            _height = height;
            _width = width;


            // Populate map with inhabitants
            int xmax = _width - 1;
            int ymax = _height - 1;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    //Is cell alive?
                    bool isAlive = _randomSource.Next(100) < probability ? true : false;

                    //Create the cell and store in map
                    var newcell = _cellFactory.buildCell(isAlive);
                    _map[x, y] = newcell;
                }
            }

            //Register neighbours
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    List<IGameOfLifeCell> neighbours = new List<IGameOfLifeCell>();
                    //Map out all neighbours
                    if (y > 0) { neighbours.Add(_map[x, y - 1]); }            // Right above current cell
                    if (y < ymax) { neighbours.Add(_map[x, y + 1]); }         // Right below current cell

                    // Left side
                    if (x > 0)
                    {
                        neighbours.Add(_map[x - 1, y]);                         // Left of curren cell
                        if (y > 0) { neighbours.Add(_map[x - 1, y - 1]); }     // Upper left corner
                        if (y < ymax) { neighbours.Add(_map[x - 1, y + 1]); }  // Lower left corner
                    }
                    //Right side
                    if (x < xmax)
                    {
                        neighbours.Add(_map[x + 1, y]);
                        if (y > 0) { neighbours.Add(_map[x + 1, y - 1]); }     // Upper right corner
                        if (y < ymax) { neighbours.Add(_map[x + 1, y + 1]); }  // Lower right corner
                    }

                    _map[x, y].Neighbours = neighbours.ToArray();
                }
            }



        }
        //Public properties
        public IGameOfLifeCell[,] Map { get { return _map; } private set { _map = value; } }

        //Public interface
        public void NextGeneration()
        {

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++) { Map[x, y].DoProgress(); }
            }
        }
    }
}

