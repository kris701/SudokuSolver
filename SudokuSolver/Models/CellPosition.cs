﻿namespace SudokuSolver.Models
{
    public class CellPosition
    {
        public byte X;
        public byte Y;

        public CellPosition(byte x, byte y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"{X},{Y}";
        }
    }
}
