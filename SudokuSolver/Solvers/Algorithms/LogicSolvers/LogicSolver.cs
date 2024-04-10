using SudokuSolver.Models;
using SudokuSolver.Solvers.Algorithms.LogicSolvers.LogicPruners;

namespace SudokuSolver.Solvers.Algorithms.LogicSolvers
{
    public class LogicSolver : BaseAlgorithm
    {
        public List<IPruner> Pruners { get; }

        public LogicSolver(List<IPruner> pruners) : base("Logic Solver")
        {
            Pruners = pruners;
        }

        public override SearchContext Solve(SearchContext context)
        {
            bool any = true;
            while (any)
            {
                if (Stop)
                    break;
                Calls++;
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
            return context;
        }
    }
}
