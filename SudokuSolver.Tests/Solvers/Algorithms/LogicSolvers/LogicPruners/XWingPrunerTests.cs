using SudokuSolver.Models;
using SudokuSolver.Solvers.Algorithms.LogicSolvers;
using SudokuSolver.Solvers.Algorithms.LogicSolvers.LogicPruners;
using SudokuSolver.Solvers.Preprocessors;

namespace SudokuSolver.Tests.Solvers.Algorithms.LogicSolvers.LogicPruners
{
    [TestClass]
    public class XWingPrunerTests
    {
        [TestMethod]
        [DataRow("100000569492056108056109240009640801064010000218035604040500016905061402621000005", 6)]
        public void Can_PruneCorrectly(string board, int expectedChange)
        {
            // ARRANGE
            var context = Preprocessor.Preprocess(new SudokuBoard(board));
            IPruner pruner1 = new XWingPruner();
            var solver = new LogicSolver(new List<IPruner>()
            {
                new CertainsPruner(),
                new NakedPairPruner(),
                new NakedTripplePruner(),
                new HiddenPairPruner(),
                new HiddenTripplePruner(),
                new PointingPairsPruner(),
                new BoxLineReductionPruner(),

                pruner1,
                new SingleChainsPruner()
            });

            // ACT
            solver.Solve(context);

            // ASSERT
            Assert.AreEqual(expectedChange, pruner1.PrunedCandidates);
        }
    }
}
