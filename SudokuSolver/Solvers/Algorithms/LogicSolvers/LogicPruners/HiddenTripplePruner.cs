using SudokuSolver.Models;

namespace SudokuSolver.Solvers.Algorithms.LogicSolvers.LogicPruners
{
    public class HiddenTripplePruner : BasePruner
    {
        public override bool Prune(SearchContext context)
        {
            var pruned = 0;

            for (byte row = 0; row < SudokuBoard.BoardSize; row++)
            {
                var candidates = GetAssignmentsFromRow(context, row);
                var removed = 1;
                while (removed > 0)
                {
                    removed = 0;
                    for (int i = 1; i <= SudokuBoard.BoardSize; i++)
                        if (candidates.Where(x => x.Value == i).Count() == 1 || candidates.Where(x => x.Value == i).Count() > 3)
                            removed += candidates.RemoveAll(x => x.Value == i);
                    for (int i = 1; i <= SudokuBoard.BoardSize; i++)
                        if (candidates.Where(x => x.Value == i).Any(y => !candidates.Any(z => z != y && z.X == y.X)))
                            removed += candidates.RemoveAll(x => x.Value == i);
                }

                if (candidates.DistinctBy(x => x.Value).Count() == 3 && candidates.DistinctBy(x => x.X).Count() == 3)
                    foreach (var candidate in candidates)
                        pruned += context.Candidates[candidate.X, candidate.Y].RemoveAll(x => !candidates.Contains(x));
            }

            for (byte column = 0; column < SudokuBoard.BoardSize; column++)
            {
                var candidates = GetAssignmentsFromColumn(context, column);
                var removed = 1;
                while (removed > 0)
                {
                    removed = 0;
                    for (int i = 1; i <= SudokuBoard.BoardSize; i++)
                        if (candidates.Where(x => x.Value == i).Count() == 1 || candidates.Where(x => x.Value == i).Count() > 3)
                            removed += candidates.RemoveAll(x => x.Value == i);
                    for (int i = 1; i <= SudokuBoard.BoardSize; i++)
                        if (candidates.Where(x => x.Value == i).Any(y => !candidates.Any(z => z != y && z.Y == y.Y)))
                            removed += candidates.RemoveAll(x => x.Value == i);
                }

                if (candidates.DistinctBy(x => x.Value).Count() == 3 && candidates.DistinctBy(x => x.Y).Count() == 3)
                    foreach (var candidate in candidates)
                        pruned += context.Candidates[candidate.X, candidate.Y].RemoveAll(x => !candidates.Contains(x));
            }

            if (pruned > 0)
                Console.WriteLine($"\t\tRemoved {pruned} candidates because of hidden tripples");
            return pruned > 0;
        }
    }
}
