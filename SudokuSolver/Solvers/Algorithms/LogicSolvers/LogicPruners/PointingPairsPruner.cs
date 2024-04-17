using SudokuSolver.Models;

namespace SudokuSolver.Solvers.Algorithms.LogicSolvers.LogicPruners
{
    public class PointingPairsPruner : BasePruner
    {
        public override bool Prune(SearchContext context)
        {
            var pruned = 0;

            for (byte blockX = 0; blockX < SudokuBoard.Blocks; blockX++)
            {
                for (byte blockY = 0; blockY < SudokuBoard.Blocks; blockY++)
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

            if (pruned > 0)
            {
                PrunedCandidates += pruned;
                Console.WriteLine($"\t\tRemoved {pruned} candidates because of pointing pairs");
            }
            return pruned > 0;
        }
    }
}
