using SudokuSolver.Models;
using SudokuSolver.Solvers.Algorithms.LogicSolvers.LogicPruners;
using SudokuSolver.Solvers.Preprocessors;

namespace SudokuSolver.Tests.Solvers.Algorithms.LogicSolvers.LogicPruners
{
    [TestClass]
    public class BoxLineReductionPrunerTests : BaseLogicPrunerTest
    {
        [TestMethod]
        [DataRow("016007803090800000870001060048000300650009082039000650060900020080002936924600510", 23)]
        [DataRow("020943715904000600750000040500480000200000453400352000042000081005004260090208504", 19)]
        public void Can_PruneCorrectly(string board, int expectedChange)
        {
            // ARRANGE
            var context = Preprocessor.Preprocess(new SudokuBoard(board));
            IPruner pruner1 = new BoxLineReductionPruner();
            var preCount = GetCardinality(context.Candidates);

            // ACT
            while (pruner1.Prune(context)) { }

            // ASSERT
            Assert.AreEqual(expectedChange, preCount - GetCardinality(context.Candidates));
        }
    }
}
