using SudokuSolver.Models;
using SudokuSolver.Solvers.Algorithms.LogicSolvers;
using SudokuSolver.Solvers.Algorithms.LogicSolvers.LogicPruners;
using SudokuSolver.Solvers.Preprocessors;

namespace SudokuSolver.Tests.Solvers.Algorithms.LogicSolvers.LogicPruners
{
    [TestClass]
    public class CertainsPrunerTests
    {
        [TestMethod]
        [DataRow("600000803040700000000000000000504070300200000106000000020000050000080600000010000", 64)]
        [DataRow("052400000000070100000000000000802000300000600090500000106030000000000089700000000", 8)]
        public void Can_PruneCorrectly(string board, int expectedChange)
        {
            // ARRANGE
            var context = Preprocessor.Preprocess(new SudokuBoard(board));
            IPruner pruner1 = new CertainsPruner();
            var solver = new LogicSolver(new List<IPruner>()
            {
                pruner1,
                new NakedPairPruner(),
                new NakedTripplePruner(),
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
