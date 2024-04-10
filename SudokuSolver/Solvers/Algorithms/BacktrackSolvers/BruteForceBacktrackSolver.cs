using SudokuSolver.Models;

namespace SudokuSolver.Solvers.Algorithms.BacktrackSolvers
{
    public class BruteForceBacktrackSolver : BaseAlgorithm
    {
        public BruteForceBacktrackSolver() : base("Brute Force Backtrack Solver")
        {
        }

        public override SearchContext Solve(SearchContext context)
        {
            var board = BacktrackSolve(context.Copy());
            if (board != null)
                context.Board = board;
            return context;
        }

        private SudokuBoard? BacktrackSolve(SearchContext context, int bestOffset = 0)
        {
            if (Stop)
                return null;
            if (context.Board.IsComplete())
                return context.Board;

            Calls++;

            for (byte x = 0; x < SudokuBoard.BoardSize; x++)
            {
                for (byte y = 0; y < SudokuBoard.BoardSize; y++)
                {
                    if (context.Board[x, y] != SudokuBoard.BlankNumber)
                        continue;
                    foreach (var possible in context.Candidates[x, y])
                    {
                        if (possible.IsLegal(context.Board))
                        {
                            possible.Apply(context.Board);
                            var result = BacktrackSolve(context, bestOffset + 1);
                            if (result != null)
                                return result;
                            possible.UnApply(context.Board);
                        }
                    }
                }
            }

            return null;
        }
    }
}
