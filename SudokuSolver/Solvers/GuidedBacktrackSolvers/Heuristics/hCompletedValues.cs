using SudokuSolver.Models;
using SudokuSolver.Preprocessors;

namespace SudokuSolver.Solvers.GuidedBacktrackSolvers.Heuristics
{
    public class hCompletedValues : IHeuristic
    {
        public int Value(SudokuBoard board, IPreprocessor preprocessor, CellAssignment assignment)
        {
            var values = board.BlockSize * board.BlockSize;

            for (byte i = 1; i <= board.BlockSize * board.BlockSize; i++)
            {
                bool exists = true;
                for (byte y = 0; y < board.BlockSize * board.BlockSize; y++)
                {
                    if (!board.RowContains(ref y, ref i))
                    {
                        exists = false;
                        break;
                    }
                }
                if (exists)
                    values--;
            }

            return values;
        }
    }
}
