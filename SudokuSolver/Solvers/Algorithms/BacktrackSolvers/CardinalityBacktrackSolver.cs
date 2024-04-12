using SudokuSolver.Models;

namespace SudokuSolver.Solvers.Algorithms.BacktrackSolvers
{
    public class CardinalityBacktrackSolver : BaseAlgorithm
    {
        public List<CardinalityCellPosition> SearchOrder { get; set; }

        public CardinalityBacktrackSolver() : base("Cardinality Backtrack Solver")
        {
            SearchOrder = new List<CardinalityCellPosition>();
        }

        public override SearchContext Solve(SearchContext context)
        {
            var cpy = context.Copy();
            SearchOrder = Order(cpy);
            Console.WriteLine($"Total possible cell assignments: {SearchOrder.Sum(x => x.Possibilities)}");
            var board = BacktrackSolve(cpy);
            if (board != null)
                return cpy;
            return context;
        }

        private List<CardinalityCellPosition> Order(SearchContext context)
        {
            var cardinalities = new List<CardinalityCellPosition>();
            var rowCardinalities = new Dictionary<int, int>();
            for (byte y = 0; y < SudokuBoard.BoardSize; y++)
            {
                rowCardinalities.Add(y, 0);
                for (byte x = 0; x < SudokuBoard.BoardSize; x++)
                {
                    if (context.Board[x, y] != SudokuBoard.BlankNumber)
                        continue;
                    cardinalities.Add(new CardinalityCellPosition(x, y, context.Candidates[x, y].Count));
                    rowCardinalities[y] += context.Candidates[x, y].Count;
                }
            }
            if (cardinalities.Any(x => x.Possibilities == 0))
                throw new Exception("Invalid preprocessing");
            cardinalities = cardinalities.OrderBy(x => rowCardinalities[x.Y]).ThenBy(x => x.Possibilities).ToList();

            return cardinalities;
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

        public class CardinalityCellPosition : CellPosition
        {
            public int Possibilities;

            public CardinalityCellPosition(byte x, byte y, int possibilities) : base(x, y)
            {
                Possibilities = possibilities;
            }

            public override string ToString()
            {
                return $"{X},{Y}:{Possibilities}";
            }
        }
    }
}
