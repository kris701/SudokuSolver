using SudokuSolver.Models;

namespace SudokuSolver.Solvers.Algorithms.LogicSolvers.LogicPruners
{
    public class XWingPruner : BasePruner
    {
        public override bool Prune(SearchContext context)
        {
            var pruned = 0;
            pruned += PruneFromRows(context);
            pruned += PruneFromColumn(context);

            Console.WriteLine($"\t\tRemoved {pruned} candidates because of X Wings");
            return pruned > 0;
        }

        private int PruneFromRows(SearchContext context)
        {
            var pruned = 0;
            var candidates = new Dictionary<int, List<CellAssignment>>();
            for (byte row = 0; row < SudokuBoard.BoardSize; row++)
            {
                var values = GetAssignmentsFromRow(context, row);
                for (int i = 1; i < SudokuBoard.BoardSize; i++)
                {
                    if (values.Count(x => x.Value == i) == 2)
                    {
                        if (!candidates.ContainsKey(i))
                            candidates.Add(i, new List<CellAssignment>());
                        candidates[i].AddRange(values.Where(x => x.Value == i));
                    }
                }
            }
            var xWings = new Dictionary<int, List<CellAssignment>>();
            foreach (var key in candidates.Keys)
                if (candidates[key].All(x => candidates[key].Any(y => x != y && x.X == y.X)))
                    xWings.Add(key, candidates[key]);
            foreach (var key in xWings.Keys)
                pruned += PruneValueCandidatesFromColumns(context, xWings[key], (byte)key);
            return pruned;
        }

        private int PruneFromColumn(SearchContext context)
        {
            var pruned = 0;
            var candidates = new Dictionary<int, List<CellAssignment>>();
            for (byte column = 0; column < SudokuBoard.BoardSize; column++)
            {
                var values = GetAssignmentsFromColumn(context, column);
                for (int i = 1; i < SudokuBoard.BoardSize; i++)
                {
                    if (values.Count(x => x.Value == i) == 2)
                    {
                        if (!candidates.ContainsKey(i))
                            candidates.Add(i, new List<CellAssignment>());
                        candidates[i].AddRange(values.Where(x => x.Value == i));
                    }
                }
            }
            var xWings = new Dictionary<int, List<CellAssignment>>();
            foreach (var key in candidates.Keys)
                if (candidates[key].All(x => candidates[key].Any(y => x != y && x.Y == y.Y)))
                    xWings.Add(key, candidates[key]);
            foreach (var key in xWings.Keys)
                pruned += PruneValueCandidatesFromRows(context, xWings[key], (byte)key);
            return pruned;
        }
    }
}
