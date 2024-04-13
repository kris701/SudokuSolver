using SudokuSolver.Models;
using System.Collections.Generic;
using System.Text;

namespace SudokuSolver.Solvers.Algorithms.LogicSolvers.LogicPruners
{
    public class NakedPairPruner : BasePruner
    {
        public override bool Prune(SearchContext context)
        {
            var pruned = 0;
            // Prune from columns
            for (int column = 0; column < SudokuBoard.BoardSize; column++)
            {
                var cellPositions = new List<CellPosition>();
                for (int row = 0; row < SudokuBoard.BoardSize; row++)
                    if (context.Candidates[column, row].Count == 2)
                        cellPositions.Add(new CellPosition((byte)column, (byte)row));
                var binaries = GetBinaryCellsFromPositions(context, cellPositions);
                foreach (var binary in binaries)
                {
                    var indexes = binary.Assignments.Select(x => x.Value).Distinct();
                    foreach (var i in indexes)
                        pruned += PruneValueCandidatesFromColumns(context, binary.Assignments, i);
                    if (context.Board.BlockX(ref binary.Location1.X) == context.Board.BlockX(ref binary.Location2.X) &&
                        context.Board.BlockY(ref binary.Location1.Y) == context.Board.BlockY(ref binary.Location2.Y))
                        foreach (var i in indexes)
                            pruned += PruneValueCandidatesFromBlock(context, context.Board.BlockX(ref binary.Location1.X), context.Board.BlockY(ref binary.Location1.Y), binary.Assignments, i);
                }
            }

            // Prune from rows
            for (int row = 0; row < SudokuBoard.BoardSize; row++)
            {
                var cellPositions = new List<CellPosition>();
                for (int column = 0; column < SudokuBoard.BoardSize; column++)
                    if (context.Candidates[column, row].Count == 2)
                        cellPositions.Add(new CellPosition((byte)column, (byte)row));
                var binaries = GetBinaryCellsFromPositions(context, cellPositions);
                foreach (var binary in binaries)
                {
                    var indexes = binary.Assignments.Select(x => x.Value).Distinct();
                    foreach (var i in indexes)
                        pruned += PruneValueCandidatesFromRows(context, binary.Assignments, i);
                    if (context.Board.BlockX(ref binary.Location1.X) == context.Board.BlockX(ref binary.Location2.X) &&
                        context.Board.BlockY(ref binary.Location1.Y) == context.Board.BlockY(ref binary.Location2.Y))
                        foreach (var i in indexes)
                            pruned += PruneValueCandidatesFromBlock(context, context.Board.BlockX(ref binary.Location1.X), context.Board.BlockY(ref binary.Location1.Y), binary.Assignments, i);
                }
            }

            // Prune from blocks
            for (byte blockX = 0; blockX < SudokuBoard.Blocks; blockX++)
            {
                for (byte blockY = 0; blockY < SudokuBoard.Blocks; blockY++)
                {
                    var cellPositions = GetFreePositionsFromBlock(context, blockX, blockY);
                    cellPositions.RemoveAll(x => context.Candidates[x.X, x.Y].Count != 2);
                    var binaries = GetBinaryCellsFromPositions(context, cellPositions);
                    foreach(var binary in binaries)
                    {
                        var indexes = binary.Assignments.Select(x => x.Value).Distinct();
                        foreach (var i in indexes)
                            pruned += PruneValueCandidatesFromBlock(context, context.Board.BlockX(ref binary.Location1.X), context.Board.BlockY(ref binary.Location1.Y), binary.Assignments, i);
                    }
                }
            }

            if (pruned > 0)
                Console.WriteLine($"\t\tRemoved {pruned} candidates because of naked pairs");
            return pruned > 0;
        }

        private List<BinaryCells> GetBinaryCellsFromPositions(SearchContext context, List<CellPosition> cellPositions)
        {
            var results = new List<BinaryCells>();
            while (cellPositions.Count > 0)
            {
                var loc = cellPositions[0];
                var loc2 = cellPositions[0];
                var assignments = context.Candidates[loc.X, loc.Y];
                var assignments2 = context.Candidates[loc2.X, loc2.Y];
                bool found = false;
                for (int i = 1; !found && i < cellPositions.Count; i++)
                {
                    loc2 = cellPositions[i];
                    assignments2 = context.Candidates[loc2.X, loc2.Y];
                    if (assignments.All(x => assignments2.Any(y => x.Value == y.Value)))
                        found = true;
                }
                if (!found)
                {
                    cellPositions.RemoveAt(0);
                    continue;
                }
                else
                {
                    var all = new List<CellAssignment>();
                    all.AddRange(assignments);
                    all.AddRange(assignments2);
                    results.Add(new BinaryCells(loc, loc2, all));
                    cellPositions.Remove(loc);
                    cellPositions.Remove(loc2);
                }
            }
            return results;
        }

        private class BinaryCells
        {
            public CellPosition Location1 { get; set; }
            public CellPosition Location2 { get; set; }
            public List<CellAssignment> Assignments { get; set; }

            public BinaryCells(CellPosition location1, CellPosition location2, List<CellAssignment> assignments)
            {
                Location1 = location1;
                Location2 = location2;
                Assignments = assignments;
            }
        }
    }
}
