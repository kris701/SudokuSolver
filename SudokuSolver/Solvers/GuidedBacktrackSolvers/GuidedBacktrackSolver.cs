using SudokuSolver.Models;
using SudokuSolver.Preprocessors;

namespace SudokuSolver.Solvers.GuidedBacktrackSolvers
{
    public class GuidedBacktrackSolver : BaseSolver
    {
        public GuidedBacktrackSolver(IPreprocessor preprocessor) : base(preprocessor)
        {
        }

        public override SudokuBoard? Run(SudokuBoard board, IPreprocessor preprocessor) => SolveInner(board, preprocessor);

        internal SudokuBoard? SolveInner(SudokuBoard board, IPreprocessor preprocessor, int bestOffset = 0)
        {
            if (_stop)
                return null;

            Calls++;

            if (bestOffset >= preprocessor.Cardinalities.Count)
            {
                if (board.IsComplete())
                    return board;
                return null;
            }

            var loc = preprocessor.Cardinalities[bestOffset];
            var possibilities = preprocessor.Candidates[loc.X, loc.Y];
            var count = possibilities.Count;
            for (int i = 0; i < count; i++)
            {
                if (possibilities[i].IsLegal(board))
                {
                    possibilities[i].Apply(board);
                    var result = SolveInner(board, preprocessor, bestOffset + 1);
                    if (result != null)
                        return result;
                    possibilities[i].UnApply(board);
                }
            }
            return null;
        }
    }
}
