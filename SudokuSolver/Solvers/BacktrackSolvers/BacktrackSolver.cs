using SudokuSolver.Models;
using SudokuSolver.Solvers.BacktrackSolvers.Pruners;
using SudokuSolver.Solvers.Preprocessors;

namespace SudokuSolver.Solvers.BacktrackSolvers
{
    public class BacktrackSolver : BaseSolver
    {
        public List<IPruner> Pruners { get; } = new List<IPruner>()
        {
            new CertainsPruner(),
            new NakedPairPruner(),
            new NakedTripplePruner(),
            new HiddenPairPruner(),
            new HiddenTripplePruner(),
            new PointingPairsPruner()
        };

        public override SudokuBoard? Run(SearchContext context)
        {
            Prune(context);
            context.Cardinalities = Preprocessor.GenerateCardinalities(context.Board, context.Candidates);
            if (context.Cardinalities.Count == 0)
            {
                if (context.Board.IsComplete())
                    return context.Board;
                return null;
            }
            Console.WriteLine($"Total possible cell assignments: {context.Cardinalities.Sum(x => x.Possibilities)}");
            Console.WriteLine("No more pruning possible, starting backtrack search...");
            return BacktrackSolve(context);
        }

        private void Prune(SearchContext context)
        {
            bool any = true;
            while (any)
            {
                any = false;
                foreach (var pruner in Pruners)
                {
                    if (pruner.Prune(context))
                    {
                        any = true;
                        break;
                    }
                }
            }
        }

        private SudokuBoard? BacktrackSolve(SearchContext context, int bestOffset = 0)
        {
            if (_stop)
                return null;

            if (bestOffset >= context.Cardinalities.Count)
            {
                if (context.Board.IsComplete())
                    return context.Board;
                return null;
            }

            Calls++;

            var loc = context.Cardinalities[bestOffset];
            var possibilities = context.Candidates[loc.X, loc.Y];
            var count = possibilities.Count;
            for (int i = 0; i < count; i++)
            {
                if (possibilities[i].IsLegal(context.Board))
                {
                    possibilities[i].Apply(context.Board);
                    var result = BacktrackSolve(context, bestOffset + 1);
                    if (result != null)
                        return result;
                    possibilities[i].UnApply(context.Board);
                }
            }
            return null;
        }
    }
}
