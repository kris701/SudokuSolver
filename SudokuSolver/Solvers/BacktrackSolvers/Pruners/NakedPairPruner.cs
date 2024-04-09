using SudokuSolver.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Solvers.BacktrackSolvers.Pruners
{
    public class NakedPairPruner : BasePruner
    {
        public override bool Prune(SearchContext context)
        {
            var any = false;
            while (PruneNakedPairs(context)) { any = true; }
            return any;
        }

        private bool PruneNakedPairs(SearchContext context)
        {
            var pruned = 0;
            // Prune from columns
            for (int column = 0; column < SudokuBoard.BoardSize; column++)
            {
                var cellPossibilities = new List<List<CellAssignment>>();
                for (int row = 0; row < SudokuBoard.BoardSize; row++)
                    cellPossibilities.Add(GetBinaryAssignments(context, column, row));

                if (cellPossibilities.Count(x => x.Count == 2) > 0)
                {
                    cellPossibilities = RemoveUnpaired(cellPossibilities, SudokuBoard.BoardSize);

                    if (cellPossibilities.Any(x => x.Count > 0))
                    {
                        var all = new List<CellAssignment>();
                        foreach (var values in cellPossibilities)
                            all.AddRange(values);

                        foreach (var value in all)
                            pruned += PruneValueCandidatesFromColumn(context, all, value.Value);
                    }
                }
            }

            // Prune from rows
            for (int row = 0; row < SudokuBoard.BoardSize; row++)
            {
                var cellPossibilities = new List<List<CellAssignment>>();
                for (int column = 0; column < SudokuBoard.BoardSize; column++)
                    cellPossibilities.Add(GetBinaryAssignments(context, column, row));

                if (cellPossibilities.Count(x => x.Count == 2) > 0)
                {
                    cellPossibilities = RemoveUnpaired(cellPossibilities, SudokuBoard.BoardSize);

                    if (cellPossibilities.Any(x => x.Count > 0))
                    {
                        var all = new List<CellAssignment>();
                        foreach (var values in cellPossibilities)
                            all.AddRange(values);

                        foreach (var value in all)
                            pruned += PruneValueCandidatesFromRow(context, all, value.Value);
                    }
                }
            }

            // Prune from blocks
            for(int blockX = 0; blockX < SudokuBoard.Blocks; blockX++)
            {
                for (int blockY = 0; blockY < SudokuBoard.Blocks; blockY++)
                {
                    var fromX = blockX * SudokuBoard.Blocks;
                    var toX = (blockX + 1) * SudokuBoard.Blocks;
                    var fromY = blockY * SudokuBoard.Blocks;
                    var toY = (blockY + 1) * SudokuBoard.Blocks;
                    var cellPossibilities = new List<List<CellAssignment>>();
                    for (int i = 0; i <= SudokuBoard.BoardSize; i++)
                        cellPossibilities.Add(new List<CellAssignment>());

                    int offset = 0;
                    for (int x = fromX; x < toX; x++)
                    {
                        for (int y = fromY; y < toY; y++)
                        {
                            if (context.Candidates[x, y].Count == 2)
                                cellPossibilities[offset] = new List<CellAssignment>(context.Candidates[x, y]);
                            offset++;
                        }
                    }

                    cellPossibilities = RemoveUnpaired(cellPossibilities, SudokuBoard.BoardSize);

                    if (cellPossibilities.Any(x => x.Count > 0))
                    {
                        var all = new List<CellAssignment>();
                        foreach (var possibles in cellPossibilities)
                            all.AddRange(possibles);

                        var values = all.Select(x => x.Value).Distinct();
                        foreach (var value in values)
                            PruneValueCandidatesFromBlock(context, (byte)blockX, (byte)blockY, all, value);
                    }
                }
            }

            if (pruned > 0)
                Console.WriteLine($"Removed {pruned} candidates because of naked pairs");
            return pruned > 0;
        }

        private List<CellAssignment> GetBinaryAssignments(SearchContext context, int column, int row)
        {
            var result = new List<CellAssignment>();
            if (context.Candidates[column, row].Count == 2)
                result.AddRange(context.Candidates[column, row]);
            return result;
        }

        private List<List<CellAssignment>> RemoveUnpaired(List<List<CellAssignment>> cellPossibilities, int boardSize)
        {
            for (int i = 0; i < boardSize; i++)
            {
                bool remove = true;
                if (cellPossibilities[i].Count > 0)
                {
                    for (int j = 0; j < boardSize; j++)
                    {
                        if (i == j)
                            continue;
                        if (cellPossibilities[j].Count > 0)
                        {
                            if (cellPossibilities[j].All(x => cellPossibilities[i].Any(y => y.Value == x.Value)))
                            {
                                remove = false;
                                break;
                            }
                        }
                    }
                }
                if (remove)
                    cellPossibilities[i].Clear();
            }
            return cellPossibilities;
        }
    }
}
