using SudokuSolver.Models;
using SudokuSolver.Solvers.Algorithms.LogicSolvers;
using SudokuSolver.Solvers.Algorithms.LogicSolvers.LogicPruners;
using SudokuSolver.Solvers.Preprocessors;

namespace SudokuSolver.Tests.Solvers.Algorithms.LogicSolvers.LogicPruners
{
    [TestClass]
    public class HiddenPairPrunerTests
    {
        [TestMethod]
        [DataRow("000000000904607000076804100309701080708000301051308702007502610005403208000000000", 9)]
        [DataRow("720408030080000047401076802810739000000851000000264080209680413340000008168943275", 10)]
        public void Can_PruneCorrectly(string board, int expectedChange)
        {
            // ARRANGE
            var context = Preprocessor.Preprocess(new SudokuBoard(board));
            IPruner pruner1 = new HiddenPairPruner();
            var solver = new LogicSolver(new List<IPruner>()
            {
                new CertainsPruner(),
                new NakedPairPruner(),
                new NakedTripplePruner(),
                pruner1,
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
