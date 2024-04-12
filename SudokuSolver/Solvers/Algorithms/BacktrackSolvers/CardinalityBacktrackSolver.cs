using SudokuSolver.Models;

namespace SudokuSolver.Solvers.Algorithms.BacktrackSolvers
{
    public class CardinalityBacktrackSolver : BaseAlgorithm
    {
        public List<CardinalityCellPosition> Cardinalities { get; set; }

        public CardinalityBacktrackSolver() : base("Cardinality Backtrack Solver")
        {
            Cardinalities = new List<CardinalityCellPosition>();
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

        private List<CardinalityCellPosition> GenerateCardinalities(SudokuBoard board, List<CellAssignment>[,] candidates)
        {
            var cardinalities = new List<CardinalityCellPosition>();
            var rowCardinalities = new Dictionary<int, int>();
            for (byte y = 0; y < SudokuBoard.BoardSize; y++)
            {
                rowCardinalities.Add(y,0);
                for (byte x = 0; x < SudokuBoard.BoardSize; x++)
                {
                    if (board[x, y] != SudokuBoard.BlankNumber)
                        continue;
                    cardinalities.Add(new CardinalityCellPosition(x, y, candidates[x, y].Count));
                    rowCardinalities[y] += candidates[x, y].Count;
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

        public class CardinalityCellPosition : CellPosition
        {
            public int Possibilities;

            public CardinalityCellPosition(byte x, byte y, int possibilities) : base(x,y)
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
