using SudokuSolver.Models;
using SudokuSolver.Solvers.Algorithms.LogicSolvers;
using SudokuSolver.Solvers.Algorithms.LogicSolvers.LogicPruners;
using SudokuSolver.Solvers.Preprocessors;

namespace SudokuSolver.Tests.Solvers.Algorithms.LogicSolvers.LogicPruners
{
    [TestClass]
    public class SingleChainsPrunerTests
    {
        [TestMethod]
        [DataRow("009600010000000023100070400004008000030040600600900008000004009010030000800100700", 5)]
        public void Can_PruneCorrectly(string board, int expectedChange)
        {
            // ARRANGE
            var context = Preprocessor.Preprocess(new SudokuBoard(board));
            IPruner pruner1 = new SingleChainsPruner();
            var solver = new LogicSolver(new List<IPruner>()
            {
                new CertainsPruner(),
                new NakedPairPruner(),
                new NakedTripplePruner(),
                new HiddenPairPruner(),
                new HiddenTripplePruner(),
                new PointingPairsPruner(),
                new BoxLineReductionPruner(),

                new XWingPruner(),
                pruner1
            });

            // ACT
            solver.Solve(context);

            // ASSERT
            Assert.AreEqual(expectedChange, pruner1.PrunedCandidates);
        }
    }
}
