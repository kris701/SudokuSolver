namespace SudokuSolver.Solvers.BacktrackSolvers
{
    public class Position
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Possibilities { get; set; }

        public Position(int x, int y, int possibilities)
        {
            X = x;
            Y = y;
            Possibilities = possibilities;
        }
    }
}
