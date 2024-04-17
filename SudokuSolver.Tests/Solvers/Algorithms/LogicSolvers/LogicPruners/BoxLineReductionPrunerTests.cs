using SudokuSolver.Models;
using SudokuSolver.Solvers.Algorithms.LogicSolvers;
using SudokuSolver.Solvers.Algorithms.LogicSolvers.LogicPruners;
using SudokuSolver.Solvers.Preprocessors;

namespace SudokuSolver.Tests.Solvers.Algorithms.LogicSolvers.LogicPruners
{
    [TestClass]
    public class BoxLineReductionPrunerTests
    {
        [TestMethod]
        [DataRow("016007803090800000870001060048000300650009082039000650060900020080002936924600510", 11)]
        [DataRow("020943715904000600750000040500480000200000453400352000042000081005004260090208504", 12)]
        public void Can_PruneCorrectly(string board, int expectedChange)
        {
            // ARRANGE
            var context = Preprocessor.Preprocess(new SudokuBoard(board));
            IPruner pruner1 = new BoxLineReductionPruner();
            var solver = new LogicSolver(new List<IPruner>()
            {
                new CertainsPruner(),
                new NakedPairPruner(),
                new NakedTripplePruner(),
                new HiddenPairPruner(),
                new HiddenTripplePruner(),
                new PointingPairsPruner(),
                pruner1,

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
