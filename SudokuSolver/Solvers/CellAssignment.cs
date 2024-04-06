using SudokuSolver.Models;

namespace SudokuSolver.Solvers
{
    public class CellAssignment
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Value { get; set; }

        public CellAssignment(int x, int y, int value)
        {
            X = x;
            Y = y;
            Value = value;
        }

        public bool IsLegal(SudokuBoard board)
        {
            if (board[X, Y] != board.BlankNumber ||
                board.ColumnContains(X, Value) ||
                board.RowContains(Y, Value) ||
                board.BlockContains(board.BlockX(X), board.BlockY(Y), Value)
                )
                return false;
            return true;
        }

        public void Apply(SudokuBoard on) => on[X, Y] = Value;

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
