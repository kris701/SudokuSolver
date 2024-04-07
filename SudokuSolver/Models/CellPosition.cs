namespace SudokuSolver.Models
{
    public class CellPosition
    {
        public byte X;
        public byte Y;
        public int Possibilities;

        public CellPosition(byte x, byte y, int possibilities)
        {
            X = x;
            Y = y;
            Possibilities = possibilities;
        }
    }
}
