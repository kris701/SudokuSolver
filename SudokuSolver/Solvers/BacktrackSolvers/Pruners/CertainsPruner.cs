using SudokuSolver.Models;
using SudokuSolver.Solvers.BacktrackSolvers.Reducers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Solvers.BacktrackSolvers.Pruners
{
    public class CertainsPruner : BasePruner
    {
        public override bool Prune(SearchContext context)
        {
            bool any = false;
            while (PruneCertains(context)) { any = true; }
            return any;
        }

        private bool PruneCertains(SearchContext context)
        {
            var pruned = 0;
            for (byte x = 0; x < context.Board.BoardSize; x++)
                for (byte y = 0; y < context.Board.BoardSize; y++)
                    if (context.Candidates[x, y].Count == 1)
                        pruned += RemoveCandidate(context, context.Candidates[x, y][0]);

            for (byte blockX = 0; blockX < context.Board.Blocks; blockX++)
            {
                for (byte blockY = 0; blockY < context.Board.Blocks; blockY++)
                {
                    var fromX = blockX * context.Board.Blocks;
                    var toX = (blockX + 1) * context.Board.Blocks;
                    var fromY = blockY * context.Board.Blocks;
                    var toY = (blockY + 1) * context.Board.Blocks;
                    var cellPossibilities = new List<CellAssignment>();
                    for (byte x = (byte)fromX; x < toX; x++)
                        for (byte y = (byte)fromY; y < toY; y++)
                            cellPossibilities.AddRange(context.Candidates[x, y]);

                    for (byte i = 1; i <= context.Board.BoardSize; i++)
                        if (cellPossibilities.Count(x => x.Value == i) == 1)
                            pruned += RemoveCandidate(context, cellPossibilities.First(x => x.Value == i));
                }
            }
            if (pruned > 0)
                Console.WriteLine($"Removed {pruned} certains");
            return pruned > 0;
        }

        private int RemoveCandidate(SearchContext context, CellAssignment cell)
        {
            var pruned = 0;
            cell.Apply(context.Board);
            context.Candidates[cell.X, cell.Y].Clear();
            pruned++;

            for (byte x2 = 0; x2 < context.Board.BoardSize; x2++)
                pruned += context.Candidates[x2, cell.Y].RemoveAll(z => z.Value == cell.Value);
            for (byte y2 = 0; y2 < context.Board.BoardSize; y2++)
                pruned += context.Candidates[cell.X, y2].RemoveAll(z => z.Value == cell.Value);

            var blockX = context.Board.BlockX(ref cell.X);
            var blockY = context.Board.BlockY(ref cell.Y);
            var fromX = blockX * context.Board.Blocks;
            var toX = (blockX + 1) * context.Board.Blocks;
            var fromY = blockY * context.Board.Blocks;
            var toY = (blockY + 1) * context.Board.Blocks;

            for (byte x = (byte)fromX; x < toX; x++)
                for (byte y = (byte)fromY; y < toY; y++)
                    pruned += context.Candidates[x, y].RemoveAll(z => z.Value == cell.Value);
            return pruned;
        }
    }
}
