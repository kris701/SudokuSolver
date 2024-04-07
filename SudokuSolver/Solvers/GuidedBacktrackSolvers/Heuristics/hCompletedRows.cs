using SudokuSolver.Helpers;
using SudokuSolver.Models;
using SudokuSolver.Preprocessors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Solvers.GuidedBacktrackSolvers.Heuristics
{
    public class hCompletedRows : IHeuristic
    {
        public int Value(SudokuBoard board, IPreprocessor preprocessor, CellAssignment assignment)
        {
            int value = board.BoardSize;
            for (byte y = 0; y < board.BoardSize; y++)
            {
                var row = board.GetRow(ref y);
                var full = true;
                for (byte x = 0; x < row.Length; x++) 
                {
                    if (row[x] == board.BlankNumber)
                    {
                        full = false;
                        break;
                    }
                }
                if (full)
                    value--;
            }
            return value;
        }
    }
}
