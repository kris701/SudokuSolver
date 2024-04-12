using SudokuSolver.Models;

namespace SudokuSolver.Solvers.Algorithms.LogicSolvers.LogicPruners
{
    public class CertainsPruner : BasePruner
    {
        public override bool Prune(SearchContext context)
        {
            var pruned = 0;
            var pre = 1;
            while (pruned - pre != 0)
            {
                pre = pruned;
                for (byte x = 0; x < SudokuBoard.BoardSize; x++)
                    for (byte y = 0; y < SudokuBoard.BoardSize; y++)
                        if (context.Candidates[x, y].Count == 1)
                            pruned += RemoveCandidate(context, context.Candidates[x, y][0]);

                for (byte blockX = 0; blockX < SudokuBoard.Blocks; blockX++)
                {
                    for (byte blockY = 0; blockY < SudokuBoard.Blocks; blockY++)
                    {
                        var cellPossibilities = GetAssignmentsFromBlock(context, blockX, blockY);
                        for (byte i = 1; i <= SudokuBoard.BoardSize; i++)
                            if (cellPossibilities.Count(x => x.Value == i) == 1)
                                pruned += RemoveCandidate(context, cellPossibilities.First(x => x.Value == i));
                    }
                }
            }
            if (pruned > 0)
                Console.WriteLine($"\t\tRemoved {pruned} certains");
            return pruned > 0;
        }

        private int RemoveCandidate(SearchContext context, CellAssignment cell)
        {
            var pruned = 0;
            cell.Apply(context.Board);
            context.Candidates[cell.X, cell.Y].Clear();
            pruned++;

            for (byte x2 = 0; x2 < SudokuBoard.BoardSize; x2++)
                pruned += context.Candidates[x2, cell.Y].RemoveAll(z => z.Value == cell.Value);
            for (byte y2 = 0; y2 < SudokuBoard.BoardSize; y2++)
                pruned += context.Candidates[cell.X, y2].RemoveAll(z => z.Value == cell.Value);

            var blockX = context.Board.BlockX(ref cell.X);
            var blockY = context.Board.BlockY(ref cell.Y);
            var fromX = blockX * SudokuBoard.Blocks;
            var toX = (blockX + 1) * SudokuBoard.Blocks;
            var fromY = blockY * SudokuBoard.Blocks;
            var toY = (blockY + 1) * SudokuBoard.Blocks;

            for (byte x = (byte)fromX; x < toX; x++)
                for (byte y = (byte)fromY; y < toY; y++)
                    pruned += context.Candidates[x, y].RemoveAll(z => z.Value == cell.Value);
            return pruned;
        }
    }
}
