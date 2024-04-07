using SudokuSolver.Models;
using SudokuSolver.Preprocessors;
using SudokuSolver.Solvers.BacktrackSolvers;

namespace SudokuSolver.Tests.Solvers.BacktrackSolvers
{
    [TestClass]
    public class BacktrackSolverTests
    {
        public static IEnumerable<object[]> Data() => BaseTests.TestCases();

        [TestMethod]
        [DynamicData(nameof(Data), DynamicDataSourceType.Method)]
        public void Can_Solve(List<byte> boardValues, byte blockSize)
        {
            // ARRANGE
            var board = new SudokuBoard(boardValues.ToArray(), blockSize);
            var solver = new BacktrackSolver(PreprocessorBuilder.GetPreprocessor(PreprocessorOptions.Full, board.BoardSize));
            solver.Timeout = BaseTests.Timeout;

            // ACT
            var result = solver.Solve(board);

            // ASSERT
            if (solver.TimedOut)
                Assert.Inconclusive();
            else
                Assert.IsNotNull(result);
        }
    }
}
