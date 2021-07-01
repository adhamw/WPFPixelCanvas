using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WPFPixelCanvas.Canvas.Models.GameOfLife.OOP.GameOfLifeCells
{
    public class GameOfLifeCellFactory
    {
        //Private fields
        private GameOfLifeCellTypes _cellType { get; set; }
        private int _tangleCount { get; set; }
        private Random _randomSource;


        //Constructor
        public GameOfLifeCellFactory(GameOfLifeCellTypes celltype, int tanglecount=1)
        {
            _randomSource = new Random();
            _cellType = celltype;
            _tangleCount = tanglecount;
        }

        //Interface
        public IGameOfLifeCell buildCell(bool isalive)
        {
            IGameOfLifeCell retval;
            switch(_cellType)
            {
                case GameOfLifeCellTypes.Complex:
                    int tangleid = _randomSource.Next(_tangleCount);
                    retval = new GameOfLifeComplexCell(isalive,tangleid);
                    break;
                case GameOfLifeCellTypes.Standard:
                    retval = new GameOfLifeCell(isalive);
                    break;
                default:
                    retval = new GameOfLifeCell(isalive);
                    break;
            }

            return retval;
        }
    }
}
