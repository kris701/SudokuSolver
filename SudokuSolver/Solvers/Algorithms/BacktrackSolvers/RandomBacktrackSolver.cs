using SudokuSolver.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Solvers.Algorithms.BacktrackSolvers
{
    public class RandomBacktrackSolver : BaseAlgorithm
    {
        private Random _rnd;
        public RandomBacktrackSolver() : base("Random Backtrack Solver")
        {
            _rnd = new Random();
        }

        public override SearchContext Solve(SearchContext context)
        {
            var board = BacktrackSolve(context.Copy());
            if (board != null)
                context.Board = board;
            return context;
        }

        private SudokuBoard? BacktrackSolve(SearchContext context)
        {
            if (Stop)
                return null;

            if (context.Board.IsComplete())
                return context.Board;

            Calls++;

            // Get next free cell
            var cell = GetNewCellIndex(context);
            if (cell == -1)
                return null;
            var xOffset = cell % SudokuBoard.BoardSize;
            var yOffset = cell / SudokuBoard.BoardSize;
            // Check candidates for cell
            foreach (var possible in context.Candidates[xOffset, yOffset])
            {
                if (possible.IsLegal(context.Board))
                {
                    possible.Apply(context.Board);
                    var result = BacktrackSolve(context);
                    if (result != null)
                        return result;
                    possible.UnApply(context.Board);
                }
            }
            return null;
        }

        private int GetNewCellIndex(SearchContext context)
        {
            var index = -1;
            while (index == -1 ||
                context.Board[(byte)(index % SudokuBoard.BoardSize), (byte)(index / SudokuBoard.BoardSize)] != SudokuBoard.BlankNumber)
            {
                if (Stop)
                    return -1;
                index = _rnd.Next(0, SudokuBoard.BoardSize * SudokuBoard.BoardSize);
            }
            return index;
        }
    }
}
