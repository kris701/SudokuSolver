namespace SudokuSolver.Solvers.BacktrackSolvers
{
    public class CellPosition
    {
        public byte X { get; set; }
        public byte Y { get; set; }
        public int Possibilities { get; set; }

        public CellPosition(byte x, byte y, int possibilities)
        {
            X = x;
            Y = y;
            Possibilities = possibilities;
        }
    }
}
