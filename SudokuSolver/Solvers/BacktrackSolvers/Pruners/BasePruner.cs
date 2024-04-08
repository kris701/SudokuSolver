using SudokuSolver.Models;
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
    }
}
