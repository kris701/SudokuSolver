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

        private SudokuBoard? BacktrackSolve(SearchContext context, byte xOffset = 0, byte yOffset = 0)
        {
            if (Stop)
                return null;

            if (yOffset == SudokuBoard.BoardSize)
            {
                if (context.Board.IsComplete())
                    return context.Board;
                else
                    return null;
            }

            Calls++;

            // Get next free cell
            while (context.Board[xOffset, yOffset] != SudokuBoard.BlankNumber)
            {
                xOffset++;
                if (xOffset >= SudokuBoard.BoardSize)
                {
                    xOffset = 0;
                    yOffset++;
                }

                if (yOffset == SudokuBoard.BoardSize)
                {
                    if (context.Board.IsComplete())
                        return context.Board;
                    else
                        return null;
                }
            }
            // Check candidates for cell
            foreach (var possible in context.Candidates[xOffset, yOffset])
            {
                if (possible.IsLegal(context.Board))
                {
                    possible.Apply(context.Board);
                    var newX = (byte)(xOffset + 1);
                    var newY = yOffset;
                    if (newX >= SudokuBoard.BoardSize)
                    {
                        newX = 0;
                        newY++;
                    }
                    var result = BacktrackSolve(context, newX, newY);
                    if (result != null)
                        return result;
                    possible.UnApply(context.Board);
                }
            }
            return null;
        }
    }
}
