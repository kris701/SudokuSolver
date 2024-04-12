using SudokuSolver.Models;
using SudokuSolver.Solvers.Algorithms.LogicSolvers.LogicPruners;
using SudokuSolver.Solvers.Preprocessors;

namespace SudokuSolver.Tests.Solvers.Algorithms.LogicSolvers.LogicPruners
{
    [TestClass]
    public class HiddenTripplePrunerTests : BaseLogicPrunerTest
    {
        [TestMethod]
        [DataRow("000001030231090000065003100678924300103050006000136700009360570006019843300000000", 15)]
        public void Can_PruneCorrectly(string board, int expectedChange)
        {
            // ARRANGE
            var context = Preprocessor.Preprocess(new SudokuBoard(board));
            IPruner pruner1 = new HiddenTripplePruner();
            var preCount = GetCardinality(context.Candidates);

            // ACT
            while (pruner1.Prune(context)) { }

            // ASSERT
            Assert.AreEqual(expectedChange, preCount - GetCardinality(context.Candidates));
        }
    }
}
