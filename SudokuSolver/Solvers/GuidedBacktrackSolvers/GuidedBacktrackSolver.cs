using SudokuSolver.Models;
using SudokuSolver.Preprocessors;
using SudokuSolver.Solvers.GuidedBacktrackSolvers.Heuristics;

namespace SudokuSolver.Solvers.GuidedBacktrackSolvers
{
    public class GuidedBacktrackSolver : BaseSolver
    {
        public override List<string> Configurations() => _configurations.Keys.ToList();
        private Dictionary<string, IHeuristic> _configurations = new Dictionary<string, IHeuristic>()
        {
            { "Default", new hSum(new List<IHeuristic>()
            {
                //new hCompletedColumns(),
                //new hCompletedRows(),
                new hMinimumBlockAssignments(),
                //new hCompletedValues()
            }) }
        };

        public GuidedBacktrackSolver(IPreprocessor preprocessor) : base(preprocessor)
        {
        }

        public override SudokuBoard? Run(SudokuBoard board, IPreprocessor preprocessor)
        {
            var heuristic = _configurations[Configuration];
            return SolveInner(board, preprocessor, heuristic);
        }

        internal SudokuBoard? SolveInner(SudokuBoard board, IPreprocessor preprocessor, IHeuristic heuristic, int bestOffset = 0)
        {
            if (_stop)
                return null;

            if (bestOffset >= preprocessor.Cardinalities.Count)
            {
                if (board.IsComplete())
                    return board;
                return null;
            }

            Calls++;

            var loc = preprocessor.Cardinalities[bestOffset];
            var possibilities = preprocessor.Candidates[loc.X, loc.Y];
            var count = possibilities.Count;

            var openList = new PriorityQueue<CellAssignment, int>();

            for (int i = 0; i < count; i++)
                if (possibilities[i].IsLegal(board))
                    openList.Enqueue(possibilities[i], heuristic.Value(board, preprocessor, possibilities[i]));

            while(openList.Count > 0)
            {
                var possible = openList.Dequeue();
                possible.Apply(board);
                var result = SolveInner(board, preprocessor, heuristic, bestOffset + 1);
                if (result != null)
                    return result;
                possible.UnApply(board);
            }

            return null;
        }
    }
}
