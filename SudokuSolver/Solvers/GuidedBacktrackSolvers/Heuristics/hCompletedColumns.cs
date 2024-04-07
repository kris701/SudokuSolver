using SudokuSolver.Models;
using SudokuSolver.Preprocessors;

namespace SudokuSolver.Solvers.GuidedBacktrackSolvers.Heuristics
{
    public class hCompletedColumns : IHeuristic
    {
        public int Value(SudokuBoard board, IPreprocessor preprocessor, CellAssignment assignment)
        {
            int value = board.BoardSize;
            for (byte x = 0; x < board.BoardSize; x++)
            {
                var column = board.GetColumn(ref x);
                var full = true;
                for (byte y = 0; y < column.Length; y++)
                {
                    if (column[y] == board.BlankNumber)
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
