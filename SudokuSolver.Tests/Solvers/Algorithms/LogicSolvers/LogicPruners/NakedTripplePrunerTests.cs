using SudokuSolver.Models;
using SudokuSolver.Solvers.Algorithms.LogicSolvers;
using SudokuSolver.Solvers.Algorithms.LogicSolvers.LogicPruners;
using SudokuSolver.Solvers.Preprocessors;

namespace SudokuSolver.Tests.Solvers.Algorithms.LogicSolvers.LogicPruners
{
    [TestClass]
    public class NakedTripplePrunerTests
    {
        [TestMethod]
        [DataRow("070408029002000004854020007008374200020000000003261700000093612200000403130642070", 10)]
        [DataRow("294513006600842319300697254000056000040080060000470000730164005900735001400928637", 18)]
        public void Can_PruneCorrectly(string board, int expectedChange)
        {
            // ARRANGE
            var context = Preprocessor.Preprocess(new SudokuBoard(board));
            IPruner pruner1 = new NakedTripplePruner();
            var solver = new LogicSolver(new List<IPruner>()
            {
                new CertainsPruner(),
                new NakedPairPruner(),
                pruner1,
                new HiddenPairPruner(),
                new HiddenTripplePruner(),
                new PointingPairsPruner(),
                new BoxLineReductionPruner(),

                new XWingPruner(),
                new SingleChainsPruner()
            });

            // ACT
            solver.Solve(context);

            // ASSERT
            Assert.AreEqual(expectedChange, pruner1.PrunedCandidates);
        }
    }
}
