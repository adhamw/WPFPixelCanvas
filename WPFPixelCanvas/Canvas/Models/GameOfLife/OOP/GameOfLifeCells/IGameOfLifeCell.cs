namespace WPFPixelCanvas.Canvas.Models.GameOfLife.OOP.GameOfLifeCells
{
    public interface IGameOfLifeCell
    {
        //Method interface
        void DoProgress();

        //Properties
        bool IsAlive { get; }
        int Age { get; }
        double Value { get; }
        int TangleId { get; }

        IGameOfLifeCell[] Neighbours { get; set; }

    }
}