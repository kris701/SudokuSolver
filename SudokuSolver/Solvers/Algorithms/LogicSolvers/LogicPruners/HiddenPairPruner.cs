using SudokuSolver.Models;

namespace SudokuSolver.Solvers.Algorithms.LogicSolvers.LogicPruners
{
    public class HiddenPairPruner : BasePruner
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
                        if (cellPossibilities.Count(x => x.Value == i) == 2)
                        {
                            for (int j = 1; j <= SudokuBoard.BoardSize; j++)
                            {
                                if (i == j)
                                    continue;
                                if (cellPossibilities.Count(x => x.Value == j) == 2)
                                {
                                    if (cellPossibilities.Where(x => x.Value == i).All(
                                        x => cellPossibilities.Where(x => x.Value == j).Any(
                                            y => y.X == x.X && y.Y == x.Y)))
                                    {
                                        foreach (var possibility in cellPossibilities.Where(x => x.Value == i))
                                            pruned += context.Candidates[possibility.X, possibility.Y].RemoveAll(x => x.Value != i && x.Value != j);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (pruned > 0)
            {
                PrunedCandidates += pruned;
                Console.WriteLine($"\t\tRemoved {pruned} candidates because of hidden pairs");
            }
            return pruned > 0;
        }
    }
}
