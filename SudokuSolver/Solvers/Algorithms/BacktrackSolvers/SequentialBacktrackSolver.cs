using SudokuSolver.Helpers;
using SudokuSolver.Models;

namespace SudokuSolver.Solvers.Algorithms.BacktrackSolvers
{
    public class SequentialBacktrackSolver : BaseAlgorithm
    {
        public List<CellPosition> SearchOrder { get; set; }
        public SequentialBacktrackSolver() : base("Sequential Backtrack Solver")
        {
            SearchOrder = new List<CellPosition>();
        }

        public override SearchContext Solve(SearchContext context)
        {
            var cpy = context.Copy();
            SearchOrder = Order(cpy);
            var board = BacktrackSolve(cpy);
            if (board != null)
                return cpy;
            return context;
        }

        private List<CellPosition> Order(SearchContext context)
        {
            var order = new List<CellPosition>();

            for (byte y = 0; y < SudokuBoard.BoardSize; y++)
                for (byte x = 0; x < SudokuBoard.BoardSize; x++)
                    if (context.Board[x, y] == SudokuBoard.BlankNumber)
                        order.Add(new CellPosition(x, y));

            return order;
        }

        private SudokuBoard? BacktrackSolve(SearchContext context, int bestOffset = 0)
        {
            if (Stop)
                return null;

            if (bestOffset >= SearchOrder.Count)
            {
                if (context.Board.IsComplete())
                    return context.Board;
                return null;
            }

            Calls++;

            var loc = SearchOrder[bestOffset];
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
