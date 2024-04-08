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
            for (int column = 0; column < context.Board.BoardSize; column++)
            {
                var cellPossibilities = new List<List<CellAssignment>>();
                for (int row = 0; row < context.Board.BoardSize; row++)
                    cellPossibilities.Add(GetBinaryAssignments(context, column, row));

                if (cellPossibilities.Count(x => x.Count == 2) > 0)
                {
                    cellPossibilities = RemoveUnpaired(cellPossibilities, context.Board.BoardSize);

                    if (cellPossibilities.Any(x => x.Count > 0))
                    {
                        var removeValues = new HashSet<int>();
                        var protectedRows = new List<int>();
                        for (int row = 0; row < context.Board.BoardSize; row++)
                        {
                            if (cellPossibilities[row].Count > 0)
                            {
                                protectedRows.Add(row);
                                foreach (var value in cellPossibilities[row])
                                    removeValues.Add(value.Value);
                            }
                        }
                        if (removeValues.Count > 0)
                        {
                            foreach (var value in removeValues)
                                for (int row = 0; row < context.Board.BoardSize; row++)
                                    if (!protectedRows.Contains(row))
                                        pruned += context.Candidates[column, row].RemoveAll(x => x.Value == value);
                        }
                    }
                }
            }

            for (int row = 0; row < context.Board.BoardSize; row++)
            {
                var cellPossibilities = new List<List<CellAssignment>>();
                for (int column = 0; column < context.Board.BoardSize; column++)
                    cellPossibilities.Add(GetBinaryAssignments(context, column, row));

                if (cellPossibilities.Count(x => x.Count == 2) > 0)
                {
                    cellPossibilities = RemoveUnpaired(cellPossibilities, context.Board.BoardSize);

                    if (cellPossibilities.Any(x => x.Count > 0))
                    {
                        var removeValues = new HashSet<int>();
                        var protectedColumn = new List<int>();
                        for (int column = 0; column < context.Board.BoardSize; column++)
                        {
                            if (cellPossibilities[column].Count > 0)
                            {
                                protectedColumn.Add(column);
                                foreach (var value in cellPossibilities[column])
                                    removeValues.Add(value.Value);
                            }
                        }
                        if (removeValues.Count > 0)
                        {
                            foreach (var value in removeValues)
                                for (int column = 0; column < context.Board.BoardSize; column++)
                                    if (!protectedColumn.Contains(column))
                                        pruned += context.Candidates[column, row].RemoveAll(x => x.Value == value);
                        }
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
