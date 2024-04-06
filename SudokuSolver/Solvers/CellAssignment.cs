using SudokuSolver.Models;
using System.Data.Common;

namespace SudokuSolver.Solvers
{
    public class CellAssignment
    {
        public byte X { get; set; }
        public byte Y { get; set; }
        public byte Value { get; set; }

        public CellAssignment(byte x, byte y, byte value)
        {
            X = x;
            Y = y;
            Value = value;
        }

        public bool IsLegal(SudokuBoard board)
        {
            if (board.ColumnContains(X, Value) ||
                board.RowContains(Y, Value))
                return false;

            if (board.BlockContains(X,Y,Value))
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
