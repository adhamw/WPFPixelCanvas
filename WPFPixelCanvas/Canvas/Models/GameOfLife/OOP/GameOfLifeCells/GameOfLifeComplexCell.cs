using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPixelCanvas.Canvas.Models.GameOfLife.OOP.GameOfLifeCells
{
    public class GameOfLifeComplexCell : IGameOfLifeCell
    {
        //Constructor
        public GameOfLifeComplexCell(bool isalive, int tangleid)
        {
            TangleId = tangleid;
            Neighbours = new GameOfLifeCell[0];
            IsAlive = isalive;
        }

        //Interface
        public void DoProgress()
        {
            Age++;

            int livecellcount = 0;
            int friendlytanglecount = 0;


            foreach (var neighbour in Neighbours)
            {
                if (neighbour.IsAlive) { livecellcount++; }
                if (neighbour.TangleId == TangleId) { friendlytanglecount++; }
            }

            //Original rule
            //bool survived = IsAlive && (livecellcount == 2 || livecellcount == 3);
            //bool isBorn = !IsAlive && (livecellcount == 3);

            //Modified rule to take tangles into the equation
            bool survived, isBorn, hasfriends;
            survived = IsAlive && (livecellcount == 3 || livecellcount == 4);
            isBorn = !IsAlive && (livecellcount == 3);
            hasfriends = friendlytanglecount > 0;

            //Determine if live or dead
            IsAlive = survived || isBorn || hasfriends;
            if (isBorn) { Age = 0; }
        }

        //Public Propeties
        public IGameOfLifeCell[] Neighbours { get; set; }
        public bool IsAlive { get; private set; }
        public int Age { get; set; } = 0;
        public int TangleId { get; set; } = 0;    // Allows grouping of cells into a unit ( i.e. a tangle )
        public double Value { get; set; } = 0.0;
    }
}
