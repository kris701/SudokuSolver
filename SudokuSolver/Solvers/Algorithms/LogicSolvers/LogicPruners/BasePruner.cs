﻿using SudokuSolver.Models;

namespace SudokuSolver.Solvers.Algorithms.LogicSolvers.LogicPruners
{
    public abstract class BasePruner : IPruner
    {
        public int PrunedCandidates { get; set; } = 0;
        public abstract bool Prune(SearchContext context);

        internal List<CellAssignment> GetAssignmentsFromBlock(SearchContext context, byte blockX, byte blockY)
        {
            var fromX = blockX * SudokuBoard.Blocks;
            var toX = (blockX + 1) * SudokuBoard.Blocks;
            var fromY = blockY * SudokuBoard.Blocks;
            var toY = (blockY + 1) * SudokuBoard.Blocks;
            var cellPossibilities = new List<CellAssignment>();
            for (int x = fromX; x < toX; x++)
                for (int y = fromY; y < toY; y++)
                    cellPossibilities.AddRange(context.Candidates[x, y]);
            return cellPossibilities;
        }

        internal List<CellPosition> GetFreePositionsFromBlock(SearchContext context, byte blockX, byte blockY)
        {
            var fromX = blockX * SudokuBoard.Blocks;
            var toX = (blockX + 1) * SudokuBoard.Blocks;
            var fromY = blockY * SudokuBoard.Blocks;
            var toY = (blockY + 1) * SudokuBoard.Blocks;
            var cellPossibilities = new List<CellPosition>();
            for (byte x = (byte)fromX; x < toX; x++)
                for (byte y = (byte)fromY; y < toY; y++)
                    if (context.Candidates[x,y].Count > 0)
                        cellPossibilities.Add(new CellPosition(x,y));
            return cellPossibilities;
        }

        internal List<CellAssignment> GetAssignmentsFromRow(SearchContext context, byte row)
        {
            var cellPossibilities = new List<CellAssignment>();
            for (int x = 0; x < SudokuBoard.BoardSize; x++)
                cellPossibilities.AddRange(context.Candidates[x, row]);
            return cellPossibilities;
        }

        internal List<CellAssignment> GetAssignmentsFromColumn(SearchContext context, byte column)
        {
            var cellPossibilities = new List<CellAssignment>();
            for (int y = 0; y < SudokuBoard.BoardSize; y++)
                cellPossibilities.AddRange(context.Candidates[column, y]);
            return cellPossibilities;
        }

        internal int PruneValueCandidatesFromRows(SearchContext context, List<CellAssignment> ignore, byte value)
        {
            var pruned = 0;
            var rows = ignore.DistinctBy(x => x.Y).Select(x => x.Y).ToList();
            for (int x = 0; x < SudokuBoard.BoardSize; x++)
                foreach (var y in rows)
                    pruned += context.Candidates[x, y].RemoveAll(v => !ignore.Contains(v) && v.Value == value);
            return pruned;
        }

        internal int PruneValueCandidatesFromColumns(SearchContext context, List<CellAssignment> ignore, byte value)
        {
            var pruned = 0;
            var columns = ignore.DistinctBy(x => x.X).Select(x => x.X).ToList();
            for (int y = 0; y < SudokuBoard.BoardSize; y++)
                foreach (var x in columns)
                    pruned += context.Candidates[x, y].RemoveAll(v => !ignore.Contains(v) && v.Value == value);
            return pruned;
        }

        internal int PruneValueCandidatesFromBlock(SearchContext context, byte blockX, byte blockY, List<CellAssignment> ignore, byte value)
        {
            var pruned = 0;
            var fromX = blockX * SudokuBoard.Blocks;
            var toX = (blockX + 1) * SudokuBoard.Blocks;
            var fromY = blockY * SudokuBoard.Blocks;
            var toY = (blockY + 1) * SudokuBoard.Blocks;
            for (int x = fromX; x < toX; x++)
                for (int y = fromY; y < toY; y++)
                    if (!ignore.Any(z => z.X == x && z.Y == y))
                        pruned += context.Candidates[x, y].RemoveAll(z => z.Value == value);
            return pruned;
        }

        internal bool IsRowAlligned(List<CellAssignment> assignments)
        {
            if (assignments.Count == 0)
                return false;
            var x = assignments[0].X;
            foreach (var assignment in assignments.Skip(1))
                if (assignment.X != x)
                    return false;

            return true;
        }

        internal bool IsColumnAlligned(List<CellAssignment> assignments)
        {
            if (assignments.Count == 0)
                return false;
            var y = assignments[0].Y;
            foreach (var assignment in assignments.Skip(1))
                if (assignment.Y != y)
                    return false;

            return true;
        }
    }
}
