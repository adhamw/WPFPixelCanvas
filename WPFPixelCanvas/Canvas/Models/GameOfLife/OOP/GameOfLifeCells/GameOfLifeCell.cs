using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPixelCanvas.Canvas.Models.GameOfLife.OOP.GameOfLifeCells
{
    public class GameOfLifeCell : IGameOfLifeCell
    {
        //Private fields

        //Constructor
        public GameOfLifeCell(bool isalive)
        {
            Neighbours = new GameOfLifeCell[0];
            IsAlive = isalive;
        }

        //Interface
        public void DoProgress()
        {
            int livecellcount = 0;
            foreach (var neighbour in Neighbours) { if (neighbour.IsAlive) livecellcount++; }

            //Alternative settings -- very organic result
            bool survived = IsAlive && (livecellcount == 1 || livecellcount == 2);
            bool isBorn = !IsAlive && (livecellcount == 3);

            IsAlive = survived || isBorn;
        }

        //Public Propeties
        //public double Value { get; private set; }
        public IGameOfLifeCell[] Neighbours { get; set; }
        public bool IsAlive { get; private set; }

        //Don't need these for standard GOL-cell. Default values
        public int Age => 0;
        public double Value => 0.0;
        public int TangleId => 0;
    }
}
