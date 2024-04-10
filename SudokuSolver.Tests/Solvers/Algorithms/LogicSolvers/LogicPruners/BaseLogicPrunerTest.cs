using SudokuSolver.Models;

namespace SudokuSolver.Tests.Solvers.Algorithms.LogicSolvers.LogicPruners
{
    public abstract class BaseLogicPrunerTest
    {
        internal int GetCardinality(List<CellAssignment>[,] candidates)
        {
            int count = 0;
            for (byte x = 0; x < SudokuBoard.BoardSize; x++)
                for (byte y = 0; y < SudokuBoard.BoardSize; y++)
                    count += candidates[x, y].Count;
            return count;
        }
    }
}
