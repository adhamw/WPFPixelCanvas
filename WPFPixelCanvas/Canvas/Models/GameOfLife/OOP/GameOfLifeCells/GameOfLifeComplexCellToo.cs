using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPixelCanvas.Canvas.Models.GameOfLife.OOP.GameOfLifeCells
{
    public class GameOfLifeComplexCellToo : IGameOfLifeCell
    {
        //Private fields
        Random _randomSource { get; set; }
        private int _totalAge { get; set; } = 0;

        //Constructor
        public GameOfLifeComplexCellToo(bool isalive, int tangleid)
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
            bool isBorn = !IsAlive && (friendlytanglecount > 2 || livecellcount == 3);

            //If no cell born, select new tangle for this pixel
            if (!isBorn && Age > 0.3 * averagNeighboureAge) { TangleId = Neighbours[_randomSource.Next(Neighbours.Length - 1)].TangleId; }
            else { IsAlive = true; }

        }

        //Public Propeties
        public IGameOfLifeCell[] Neighbours { get; set; }
        public bool IsAlive { get; private set; }
        public int Age { get; set; } = 0;
        public int TangleId { get; set; } = 0;    // Allows grouping of cells into a unit ( i.e. a tangle )
        public double Value { get; set; } = 0.0;
    }
}
