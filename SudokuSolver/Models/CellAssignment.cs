namespace SudokuSolver.Models
{
    public class CellAssignment
    {
        public byte X;
        public byte Y;
        public byte Value;

        public CellAssignment(byte x, byte y, byte value)
        {
            X = x;
            Y = y;
            Value = value;
        }

        public bool IsLegal(SudokuBoard board)
        {
            if (board.ColumnContains(ref X, ref Value) ||
                board.RowContains(ref Y, ref Value))
                return false;

            if (board.BlockContains(ref X, ref Y, ref Value))
                return false;
            return true;
        }

        public void Apply(SudokuBoard on) => on[X, Y] = Value;
        public void UnApply(SudokuBoard on) => on[X, Y] = SudokuBoard.BlankNumber;

        public override string ToString() => $"[{X},{Y}:{Value}]";

        public override int GetHashCode() => HashCode.Combine(X, Y, Value);

        public override bool Equals(object? obj)
        {
            if (obj is CellAssignment other)
            {
                if (other.X != X) return false;
                if (other.Y != Y) return false;
                if (other.Value != Value) return false;
                return true;
            }
            return false;
        }
    }
}
