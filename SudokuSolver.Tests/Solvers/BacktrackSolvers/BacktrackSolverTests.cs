using SudokuSolver.Models;
using SudokuSolver.Solvers.BacktrackSolvers;

namespace SudokuSolver.Tests.Solvers.BacktrackSolvers
{
    [TestClass]
    public class BacktrackSolverTests
    {
        public static IEnumerable<object[]> Data() => BaseTests.TestCases();

        [TestMethod]
        [DynamicData(nameof(Data), DynamicDataSourceType.Method)]
        public void Can_Solve(string dataset, string boardStr, List<byte> boardValues)
        {
            // ARRANGE
            var board = new SudokuBoard(boardValues.ToArray());
            var solver = new BacktrackSolver();
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
