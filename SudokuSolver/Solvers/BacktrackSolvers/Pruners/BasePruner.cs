﻿using SudokuSolver.Models;
using SudokuSolver.Solvers.BacktrackSolvers.Reducers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Solvers.BacktrackSolvers.Pruners
{
    public abstract class BasePruner : IPruner
    {
        public abstract bool Prune(SearchContext context);

        internal List<CellAssignment> GetAssignmentsFromBlock(SearchContext context, byte blockX, byte blockY)
        {
            var fromX = blockX * context.Board.Blocks;
            var toX = (blockX + 1) * context.Board.Blocks;
            var fromY = blockY * context.Board.Blocks;
            var toY = (blockY + 1) * context.Board.Blocks;
            var cellPossibilities = new List<CellAssignment>();
            for (int x = fromX; x < toX; x++)
                for (int y = fromY; y < toY; y++)
                    cellPossibilities.AddRange(context.Candidates[x, y]);
            return cellPossibilities;
        }

        internal int PruneValueCandidatesFromRow(SearchContext context, List<CellAssignment> ignore, byte value)
        {
            var pruned = 0;
            for (int x = 0; x < context.Board.BoardSize; x++)
                if (!ignore.Any(z => z.X == x))
                    pruned += context.Candidates[x, ignore[0].Y].RemoveAll(v => v.Value == value);
            return pruned;
        }

        internal int PruneValueCandidatesFromColumn(SearchContext context, List<CellAssignment> ignore, byte value)
        {
            var pruned = 0;
            for (int y = 0; y < context.Board.BoardSize; y++)
                if (!ignore.Any(z => z.Y == y))
                    pruned += context.Candidates[ignore[0].X, y].RemoveAll(v => v.Value == value);
            return pruned;
        }

        internal int PruneValueCandidatesFromBlock(SearchContext context, byte blockX, byte blockY, List<CellAssignment> ignore, byte value)
        {
            var pruned = 0;
            var fromX = blockX * context.Board.Blocks;
            var toX = (blockX + 1) * context.Board.Blocks;
            var fromY = blockY * context.Board.Blocks;
            var toY = (blockY + 1) * context.Board.Blocks;
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
