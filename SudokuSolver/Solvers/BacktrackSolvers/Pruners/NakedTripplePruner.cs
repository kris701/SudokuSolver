using SudokuSolver.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Solvers.BacktrackSolvers.Pruners
{
    public class NakedTripplePruner : BasePruner
    {
        public override bool Prune(SearchContext context)
        {
            var any = false;
            while (PruneNakedTripples(context)) { any = true; }
            return any;
        }

        private bool PruneNakedTripples(SearchContext context)
        {
            var pruned = 0;

            // Prune from columns
            for (int column = 0; column < context.Board.BoardSize; column++)
            {
                var cellPossibilities = new List<CellAssignment>();
                for (int row = 0; row < context.Board.BoardSize; row++)
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
            for (int row = 0; row < context.Board.BoardSize; row++)
            {
                var cellPossibilities = new List<CellAssignment>();
                for (int column = 0; column < context.Board.BoardSize; column++)
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
            for (byte blockX = 0; blockX < context.Board.Blocks; blockX++)
            {
                for (byte blockY = 0; blockY < context.Board.Blocks; blockY++)
                {
                    if (context.Board.GetBlockValues(ref blockX, ref blockY).Count == context.Board.BlockSize * context.Board.BlockSize - 3)
                    {
                        var cellPossibilities = GetAssignmentsFromBlock(context, blockX, blockY);
                        for (byte i = 1; i <= context.Board.BoardSize; i++)
                        {
                            var valueAssignments = cellPossibilities.Where(x => x.Value == i).ToList();
                            if (IsRowAlligned(valueAssignments))
                                pruned += PruneValueCandidatesFromColumn(context, valueAssignments, i);
                            else if (IsColumnAlligned(valueAssignments))
                                pruned += PruneValueCandidatesFromRow(context, valueAssignments, i);
                        }
                    }
                }
            }

            if (pruned > 0)
                Console.WriteLine($"Removed {pruned} candidates because of naked tripples");
            return pruned > 0;
        }
    }
}
