namespace SudokuSolver.Solvers.BacktrackSolvers
{
    public class CellPosition
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Possibilities { get; set; }

        public CellPosition(int x, int y, int possibilities)
        {
            X = x;
            Y = y;
            Possibilities = possibilities;
        }
    }
}
