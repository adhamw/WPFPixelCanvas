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
        public GameOfLifeCellFactory(GameOfLifeCellTypes celltype, int tanglecount = 1)
        {
            _randomSource = new Random();
            _cellType = celltype;
            _tangleCount = tanglecount;
        }

        //Interface
        public IGameOfLifeCell buildCell(bool isalive)
        {
            int tangleid;
            IGameOfLifeCell retval;
            switch (_cellType)
            {
                case GameOfLifeCellTypes.Complex:
                    tangleid = _randomSource.Next(_tangleCount);
                    retval = new GameOfLifeComplexCell(isalive, tangleid);
                    break;
                case GameOfLifeCellTypes.ComplexToo:
                    tangleid = _randomSource.Next(_tangleCount);
                    retval = new GameOfLifeComplexCellToo(isalive, tangleid);
                    break;
                case GameOfLifeCellTypes.ComplexAsWell:
                    tangleid = _randomSource.Next(_tangleCount);
                    retval = new GameOfLifeComplexCellAsWell(isalive, tangleid);
                    break;
                case GameOfLifeCellTypes.ComplexIndeed:
                    tangleid = _randomSource.Next(_tangleCount);
                    retval = new GameOfLifeComplexCellIndeed(isalive, tangleid);
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
