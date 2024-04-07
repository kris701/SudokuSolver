using SudokuSolver.Models;
using SudokuSolver.Preprocessors;
using SudokuSolver.Solvers.GuidedBacktrackSolvers;

namespace SudokuSolver.Tests.Solvers.GuidedBacktrackSolvers
{
    [TestClass]
    public class GuidedBacktrackSolverTests
    {
        public static IEnumerable<object[]> Data() => BaseTests.TestCases();

        [TestMethod]
        [DynamicData(nameof(Data), DynamicDataSourceType.Method)]
        public void Can_Solve(List<byte> boardValues, byte blockSize)
        {
            // ARRANGE
            var board = new SudokuBoard(boardValues.ToArray(), blockSize);
            var solver = new GuidedBacktrackSolver(PreprocessorBuilder.GetPreprocessor(PreprocessorOptions.Full, board.BoardSize));
            solver.Configuration = "Default";
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
