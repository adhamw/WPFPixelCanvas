using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPixelCanvas.Canvas.Models.GameOfLife.OOP.GameOfLifeCells
{
    public class GameOfLifeComplexCell : IGameOfLifeCell
    {
        //Private fields
        Random _randomSource { get; set; }
        private int _totalAge { get; set; } = 0;

        //Constructor
        public GameOfLifeComplexCell(bool isalive, int tangleid)
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
            double averageage = 0;
            foreach (var neighbour in Neighbours)
            {
                averageage += neighbour.Age;
                if (neighbour.IsAlive) { livecellcount++; }
                if (neighbour.TangleId == TangleId) { friendlytanglecount++; }
            }
            averageage = averageage / Neighbours.Length;

            //Modified rule to take tangles into the equation
            bool survived, isBorn, isBornByFriends, isSuperior;
            survived = IsAlive && (livecellcount == 3 || livecellcount == 4);   // Original GOL rule
            isSuperior = IsAlive && (Age / averageage) > 0.3;
            isBorn = !IsAlive && (livecellcount == 3);                          // Original GOL rule
            isBornByFriends = !IsAlive && friendlytanglecount > 7;              // Taking tangles into account 

            //Determine if live or dead
            IsAlive = (isSuperior || isBorn || isBornByFriends); // || hasfriends;
            if (isBorn) { Age = 0; }
            //If it didn't survive, move to a different tangle
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
