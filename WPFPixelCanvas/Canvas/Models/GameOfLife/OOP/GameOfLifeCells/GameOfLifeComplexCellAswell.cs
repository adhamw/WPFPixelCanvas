using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPixelCanvas.Canvas.Models.GameOfLife.OOP.GameOfLifeCells
{
    public class GameOfLifeComplexCellAsWell : IGameOfLifeCell
    {
        //Private fields
        Random _randomSource { get; set; }
        private int _totalAge { get; set; } = 0;

        //Constructor
        public GameOfLifeComplexCellAsWell(bool isalive, int tangleid)
        {
            TangleId = tangleid;
            Neighbours = new GameOfLifeCell[0];
            IsAlive = isalive;
            _randomSource = new Random();
        }

        //Interface
        public void DoProgress()
        {
            Age++;
            _totalAge++;

            int livecellcount = 0;
            int friendlytanglecount = 0;
            double averagNeighboureAge = 0;

            foreach (var neighbour in Neighbours)
            {
                int na = neighbour.Age;
                averagNeighboureAge += na;

                if (neighbour.IsAlive) { livecellcount++; }
                if (neighbour.TangleId == TangleId) { friendlytanglecount++; }
            }
            averagNeighboureAge = averagNeighboureAge / Neighbours.Length;

            //Modified rule to take tangles into the equation
            IsAlive = friendlytanglecount > 6;
            if (!IsAlive) { TangleId = Neighbours[_randomSource.Next(Neighbours.Length - 1)].TangleId; }

        }

        //Public Propeties
        public IGameOfLifeCell[] Neighbours { get; set; }
        public bool IsAlive { get; private set; }
        public int Age { get; set; } = 0;
        public int TangleId { get; set; } = 0;    // Allows grouping of cells into a unit ( i.e. a tangle )
        public double Value { get; set; } = 0.0;
    }
}
