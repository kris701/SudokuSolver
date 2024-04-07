using SudokuSolver.Models;
using SudokuSolver.Preprocessors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Solvers.GuidedBacktrackSolvers.Heuristics
{
    public class hMinimumBlockAssignments : IHeuristic
    {
        public int Value(SudokuBoard board, IPreprocessor preprocessor, CellAssignment assignment)
        {
            var blockX = board.BlockX(ref assignment.X);
            var blockY = board.BlockY(ref assignment.Y);
            var fromX = blockX * board.Blocks;
            var toX = (blockX + 1) * board.Blocks;
            var fromY = blockY * board.Blocks;
            var toY = (blockY + 1) * board.Blocks;

            var totals = 0;

            for (byte x = (byte)fromX; x < toX; x++)
            {
                for (byte y = (byte)fromY; y < toY; y++)
                {
                    if (assignment.X == x && assignment.Y == y)
                        continue;
                    if (board[x, y] == board.BlankNumber)
                        continue;

                    foreach (var possible in preprocessor.Candidates[x, y])
                        if (possible.Value == assignment.Value)
                            totals++;

                }
            }

            return totals;
        }
    }
}
