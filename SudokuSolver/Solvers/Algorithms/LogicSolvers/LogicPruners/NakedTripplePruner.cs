using SudokuSolver.Models;

namespace SudokuSolver.Solvers.Algorithms.LogicSolvers.LogicPruners
{
    public class NakedTripplePruner : BasePruner
    {
        public override bool Prune(SearchContext context)
        {
            var pruned = 0;

            // Prune from columns
            for (int column = 0; column < SudokuBoard.BoardSize; column++)
            {
                var cellPossibilities = new List<CellAssignment>();
                for (int row = 0; row < SudokuBoard.BoardSize; row++)
                    cellPossibilities.AddRange(context.Candidates[column, row]);

                if (cellPossibilities.DistinctBy(x => x.Value).Count() == 3)
                {
                    var isInBlock = cellPossibilities.All(x => context.Board.BlockY(ref x.Y) == context.Board.BlockY(ref cellPossibilities[0].Y));
                    if (isInBlock)
                    {
                        foreach (var possible in cellPossibilities)
                        {
                            pruned += PruneValueCandidatesFromBlock(
                                context,
                                context.Board.BlockX(ref possible.X),
                                context.Board.BlockY(ref possible.Y),
                                cellPossibilities,
                                possible.Value);
                        }
                    }
                }
            }

            // Prune from rows
            for (int row = 0; row < SudokuBoard.BoardSize; row++)
            {
                var cellPossibilities = new List<CellAssignment>();
                for (int column = 0; column < SudokuBoard.BoardSize; column++)
                    cellPossibilities.AddRange(context.Candidates[column, row]);

                if (cellPossibilities.DistinctBy(x => x.Value).Count() == 3)
                {
                    var isInBlock = cellPossibilities.All(x => context.Board.BlockX(ref x.X) == context.Board.BlockX(ref cellPossibilities[0].X));
                    if (isInBlock)
                    {
                        foreach (var possible in cellPossibilities)
                        {
                            pruned += PruneValueCandidatesFromBlock(
                                context,
                                context.Board.BlockX(ref possible.X),
                                context.Board.BlockY(ref possible.Y),
                                cellPossibilities,
                                possible.Value);
                        }
                    }
                }
            }

            // Prune from blocks
            for (byte blockX = 0; blockX < SudokuBoard.Blocks; blockX++)
            {
                for (byte blockY = 0; blockY < SudokuBoard.Blocks; blockY++)
                {
                    if (context.Board.GetBlockValues(ref blockX, ref blockY).Count == SudokuBoard.BlockSize * SudokuBoard.BlockSize - 3)
                    {
                        var cellPossibilities = GetAssignmentsFromBlock(context, blockX, blockY);
                        for (byte i = 1; i <= SudokuBoard.BoardSize; i++)
                        {
                            var valueAssignments = cellPossibilities.Where(x => x.Value == i).ToList();
                            if (IsRowAlligned(valueAssignments))
                                pruned += PruneValueCandidatesFromColumns(context, valueAssignments, i);
                            else if (IsColumnAlligned(valueAssignments))
                                pruned += PruneValueCandidatesFromRows(context, valueAssignments, i);
                        }
                    }
                }
            }

            if (pruned > 0)
                Console.WriteLine($"\t\tRemoved {pruned} candidates because of naked tripples");
            return pruned > 0;
        }
    }
}
