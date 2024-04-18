using SudokuSolver.Helpers;
using SudokuSolver.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Solvers.Algorithms.LogicSolvers.LogicPruners
{
    public class SingleChainsPruner : BasePruner
    {
        public override bool Prune(SearchContext context)
        {
            var pruned = 0;

            for (int i = 1; i <= SudokuBoard.BoardSize; i++)
            {
                var possibles = GetChainAssignments(context, i);
                if (possibles.Count == 0)
                    continue;
                pruned += PruneFor2NdRule(context, possibles);
                pruned += PruneFor4thRule(context, possibles);
            }

            if (pruned > 0)
            {
                PrunedCandidates += pruned;
                Console.WriteLine($"\t\tRemoved {pruned} candidates because of Single Chains elimination");
            }
            return pruned > 0;
        }

        private int PruneFor2NdRule(SearchContext context, HashSet<ChainCell> possiblities)
        {
            var pruned = 0;
            for(int row = 0; row < SudokuBoard.BoardSize; row++)
            {
                var forRow = possiblities.Where(x => x.Cell.Y == row).ToList();
                if (forRow.Count != 2)
                    continue;
                if (forRow.All(x => x.Color == forRow[0].Color) && forRow[0].IsInChain(forRow[1].Cell) && forRow[1].IsInChain(forRow[0].Cell))
                {
                    var targetCells = forRow[0].GetCellsOfColor(forRow[0].Color);
                    foreach (var target in targetCells)
                        if (context.Candidates[target.X, target.Y].Remove(target))
                            pruned++;
                }
            }

            for (int column = 0; column < SudokuBoard.BoardSize; column++)
            {
                var forRow = possiblities.Where(x => x.Cell.X == column).ToList();
                if (forRow.Count != 2)
                    continue;
                if (forRow.All(x => x.Color == forRow[0].Color) && forRow[0].IsInChain(forRow[1].Cell) && forRow[1].IsInChain(forRow[0].Cell))
                {
                    var targetCells = forRow[0].GetCellsOfColor(forRow[0].Color);
                    foreach (var target in targetCells)
                        if (context.Candidates[target.X, target.Y].Remove(target))
                            pruned++;
                }
            }

            for (int blockY = 0; blockY < SudokuBoard.Blocks; blockY++)
            {
                for (int blockX = 0; blockX < SudokuBoard.Blocks; blockX++)
                {
                    var forBlock = possiblities.Where(x => context.Board.BlockX(ref x.Cell.X) == blockX && context.Board.BlockY(ref x.Cell.Y) == blockY).ToList();
                    if (forBlock.Count != 2)
                        continue;
                    if (forBlock.All(x => x.Color == forBlock[0].Color) && forBlock[0].IsInChain(forBlock[1].Cell) && forBlock[1].IsInChain(forBlock[0].Cell))
                    {
                        var targetCells = forBlock[0].GetCellsOfColor(forBlock[0].Color);
                        foreach (var target in targetCells)
                            if (context.Candidates[target.X, target.Y].Remove(target))
                                pruned++;
                    }
                }
            }

            return pruned;
        }

        private int PruneFor4thRule(SearchContext context, HashSet<ChainCell> possiblities)
        {
            var pruned = 0;

            var checkedCells = new HashSet<ChainCell>();
            foreach(var possible in possiblities)
            {
                if (checkedCells.Contains(possible))
                    continue;
                var fullChain = possible.GetFullChain().ToList();
                checkedCells.AddRange(fullChain);

                for(int i = 0; i < fullChain.Count; i++)
                {
                    for (int j = 0; j < fullChain.Count; j++)
                    {
                        if (fullChain[i].Color == fullChain[j].Color)
                            continue;
                        var intersected = Intersection(context, fullChain[i].Cell, fullChain[j].Cell);
                        foreach (var inter in intersected)
                            if (context.Candidates[inter.X, inter.Y].Remove(inter))
                                pruned++;
                    }
                }
            }

            return pruned;
        }

        private List<CellAssignment> Intersection(SearchContext context, CellAssignment a, CellAssignment b)
        {
            if (a.X == b.X)
                return GetAssignmentsFromColumn(context, a.X).Where(x => x.Value == a.Value).Except(new List<CellAssignment>() { a,b }).ToList();
            else if (a.Y == b.Y)
                return GetAssignmentsFromRow(context, a.Y).Where(x => x.Value == a.Value).Except(new List<CellAssignment>() { a, b }).ToList();
            else
            {
                var result = new List<CellAssignment>();
                result.AddRange(context.Candidates[a.X, b.Y].Where(x => x.Value == a.Value));
                result.AddRange(context.Candidates[b.X, a.Y].Where(x => x.Value == a.Value));
                return result;
            }
        }

        private HashSet<ChainCell> GetChainAssignments(SearchContext context, int value)
        {
            var possibles = new HashSet<ChainCell>();

            // Row connections
            for(byte row = 0; row < SudokuBoard.BoardSize; row++)
            {
                var assignments = GetAssignmentsFromRow(context, row);
                var ofValue = assignments.Where(x => x.Value == value).ToList();
                if (ofValue.Count == 2)
                    InsertNewChainCells(ofValue[0], ofValue[1], possibles);
            }

            // Column connections
            for (byte column = 0; column < SudokuBoard.BoardSize; column++)
            {
                var assignments = GetAssignmentsFromColumn(context, column);
                var ofValue = assignments.Where(x => x.Value == value).ToList();
                if (ofValue.Count == 2)
                    InsertNewChainCells(ofValue[0], ofValue[1], possibles);
            }

            // Intra-block connections
            for (byte blockX = 0; blockX < SudokuBoard.Blocks; blockX++)
            {
                for (byte blockY = 0; blockY < SudokuBoard.Blocks; blockY++)
                {
                    var assignments = GetAssignmentsFromBlock(context, blockX, blockY);
                    var ofValue = assignments.Where(x => x.Value == value).ToList();
                    if (ofValue.Count == 2)
                        InsertNewChainCells(ofValue[0], ofValue[1], possibles);
                }
            }

            // Assign switching colors to all cells
            foreach(var possible in possibles)
            {
                if (possible.Color != Color.NotSet)
                    continue;
                possible.SetColor(Color.On);
            }

            // Check to make sure there is no invalid connections
            foreach(var possible in possibles)
            {
                var invColor = Color.NotSet;
                if (possible.Color == Color.On)
                    invColor = Color.Off;
                else
                    invColor = Color.On;
                if (possible.Connected.Any(x => x.Color != invColor))
                    throw new Exception("Connected list had invalid colors!");
            }

            return possibles;
        }

        private void InsertNewChainCells(CellAssignment fromCell, CellAssignment toCell, HashSet<ChainCell> possibles)
        {
            var from = new ChainCell(fromCell);
            var to = new ChainCell(toCell);
            if (possibles.Contains(to))
                to = possibles.First(x => x.Equals(to));
            else
                possibles.Add(to);
            if (possibles.Contains(from))
                from = possibles.First(x => x.Equals(from));
            else
                possibles.Add(from);

            from.Connected.Add(to);
            to.Connected.Add(from);
        }

 
        private enum Color { NotSet, On, Off }

        private class ChainCell
        {
            public CellAssignment Cell { get; set; }
            public Color Color { get; set; }
            public HashSet<ChainCell> Connected { get; set; }

            public ChainCell(CellAssignment cell)
            {
                Cell = cell;
                Color = Color.NotSet;
                Connected = new HashSet<ChainCell>();
            }

            public bool IsInChain(CellAssignment cell) => IsInChain(cell, new HashSet<CellAssignment>());
            private bool IsInChain(CellAssignment cell, HashSet<CellAssignment> checkedCells)
            {
                if (checkedCells.Contains(Cell))
                    return false;
                checkedCells.Add(Cell);

                if (Cell.Equals(cell))
                    return true;
                foreach (var to in Connected)
                    if (to.IsInChain(cell, checkedCells))
                        return true;
                return false;
            }

            public void SetColor(Color color)
            {
                if (Color == color)
                    return;

                Color = color;
                var invColor = Color.NotSet;
                if (Color == Color.On)
                    invColor = Color.Off;
                else
                    invColor = Color.On;
                foreach (var to in Connected)
                    to.SetColor(invColor);
            }

            public HashSet<CellAssignment> GetCellsOfColor(Color color) => GetCellsOfColor(color, new HashSet<CellAssignment>());
            private HashSet<CellAssignment> GetCellsOfColor(Color color, HashSet<CellAssignment> checkedCells)
            {
                if (checkedCells.Contains(Cell))
                    return new HashSet<CellAssignment>();
                checkedCells.Add(Cell);

                var cells = new HashSet<CellAssignment>();
                if (Color == color)
                    cells.Add(Cell);
                foreach (var to in Connected)
                    cells.AddRange(to.GetCellsOfColor(color, checkedCells));

                return cells;
            }

            public HashSet<ChainCell> GetFullChain() => GetFullChain(new HashSet<CellAssignment>());
            private HashSet<ChainCell> GetFullChain(HashSet<CellAssignment> checkedCells)
            {
                if (checkedCells.Contains(Cell))
                    return new HashSet<ChainCell>();
                checkedCells.Add(Cell);

                var chainCells = new HashSet<ChainCell>() { this };
                foreach (var con in Connected)
                    chainCells.AddRange(con.GetFullChain(checkedCells));
                return chainCells;
            }


            public override int GetHashCode()
            {
                return HashCode.Combine(Cell);
            }

            public override bool Equals(object? obj)
            {
                if (obj is ChainCell other)
                {
                    if (!other.Cell.Equals(Cell)) return false;
                    return true;
                }
                return false;
            }
        }
    }
}
