using SudokuSolver.Models;
using System.Text;

namespace SudokuSolver.Solvers.Algorithms.LogicSolvers.LogicPruners
{
    public class BoxLineReductionPruner : BasePruner
    {
        public override bool Prune(SearchContext context)
        {
            var pruned = 0;
            pruned += PruneFromRows(context);
            if (pruned == 0)
                pruned += PruneFromColumns(context);

            if (pruned > 0)
            {
                PrunedCandidates += pruned;
                Console.WriteLine($"\t\tRemoved {pruned} candidates because of box line reductions");
            }
            return pruned > 0;
        }

        private int PruneFromRows(SearchContext context)
        {
            for (byte row = 0; row < SudokuBoard.BoardSize; row++)
            {
                var rowValues = GetAssignmentsFromRow(context, row);
                for (byte i = 1; i <= SudokuBoard.BoardSize; i++)
                {
                    var values = rowValues.Where(x => x.Value == i).ToList();
                    if (values.DistinctBy(x => context.Board.BlockX(ref x.X)).Count() == 1)
                    {
                        var blockX = context.Board.BlockX(ref values[0].X);
                        var blockY = context.Board.BlockY(ref values[0].Y);
                        var pruned = PruneValueCandidatesFromBlock(context, blockX, blockY, values, i);
                        if (pruned > 0)
                            return pruned;
                    }
                }
            }
            return 0;
        }

        private int PruneFromColumns(SearchContext context)
        {
            for (byte column = 0; column < SudokuBoard.BoardSize; column++)
            {
                var columnValues = GetAssignmentsFromColumn(context, column);
                for (byte i = 1; i <= SudokuBoard.BoardSize; i++)
                {
                    var values = columnValues.Where(x => x.Value == i).ToList();
                    if (values.DistinctBy(x => context.Board.BlockY(ref x.Y)).Count() == 1)
                    {
                        var blockX = context.Board.BlockX(ref values[0].X);
                        var blockY = context.Board.BlockY(ref values[0].Y);
                        var pruned = PruneValueCandidatesFromBlock(context, blockX, blockY, values, i);
                        if (pruned > 0)
                            return pruned;
                    }
                }
            }
            return 0;
        }
    }
}
