﻿using SudokuSolver.Models;

namespace SudokuSolver.Solvers
{
    public class PossibleAssignment
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Value { get; set; }

        public PossibleAssignment(int x, int y, int value)
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
                board.CellContains(board.CellX(X), board.CellY(Y), Value)
                )
                return false;
            return true;
        }

        public void Apply(SudokuBoard on) => on[X, Y] = Value;

        public override string ToString() => $"[{X},{Y}:{Value}]";

        public override int GetHashCode() => HashCode.Combine(X, Y, Value);

        public override bool Equals(object? obj)
        {
            if (obj is PossibleAssignment other)
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
