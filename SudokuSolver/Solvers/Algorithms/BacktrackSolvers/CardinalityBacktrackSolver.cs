using SudokuSolver.Models;

namespace SudokuSolver.Solvers.Algorithms.BacktrackSolvers
{
    public class CardinalityBacktrackSolver : BaseAlgorithm
    {
        public List<CellPosition> Cardinalities { get; set; }

        public CardinalityBacktrackSolver() : base("Cardinality Backtrack Solver")
        {
            Cardinalities = new List<CellPosition>();
        }

        public override SearchContext Solve(SearchContext context)
        {
            var cpy = context.Copy();
            Cardinalities = GenerateCardinalities(cpy.Board, cpy.Candidates);
            Console.WriteLine($"Total possible cell assignments: {Cardinalities.Sum(x => x.Possibilities)}");
            var board = BacktrackSolve(cpy);
            if (board != null)
                context.Board = board;
            return context;
        }

        private List<CellPosition> GenerateCardinalities(SudokuBoard board, List<CellAssignment>[,] candidates)
        {
            var cardinalities = new List<CellPosition>();
            for (byte x = 0; x < SudokuBoard.BoardSize; x++)
            {
                for (byte y = 0; y < SudokuBoard.BoardSize; y++)
                {
                    if (board[x, y] != SudokuBoard.BlankNumber)
                        continue;
                    cardinalities.Add(new CellPosition(x, y, candidates[x, y].Count));
                }
            }
            if (cardinalities.Any(x => x.Possibilities == 0))
                throw new Exception("Invalid preprocessing");
            cardinalities = cardinalities.OrderBy(x => x.Possibilities).ToList();

            return cardinalities;
        }

        private SudokuBoard? BacktrackSolve(SearchContext context, int bestOffset = 0)
        {
            if (Stop)
                return null;

            if (bestOffset >= Cardinalities.Count)
            {
                if (context.Board.IsComplete())
                    return context.Board;
                return null;
            }

            Calls++;

            var loc = Cardinalities[bestOffset];
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
