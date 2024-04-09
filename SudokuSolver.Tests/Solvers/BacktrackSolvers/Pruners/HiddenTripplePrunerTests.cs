using SudokuSolver.Models;
using SudokuSolver.Solvers.BacktrackSolvers.Pruners;
using SudokuSolver.Solvers.Preprocessors;

namespace SudokuSolver.Tests.Solvers.BacktrackSolvers.Pruners
{
    [TestClass]
    public class HiddenTripplePrunerTests
    {
        [TestMethod]
        [DataRow("000001030231090000065003100678924300103050006000136700009360570006019843300000000", 15)]
        public void Can_PruneCorrectly(string board, int expectedChange)
        {
            // ARRANGE
            var values = new List<byte>();
            foreach (var c in board)
                values.Add(byte.Parse($"{c}"));
            var context = Preprocessor.Preprocess(new SudokuBoard(values.ToArray()));
            IPruner pruner1 = new HiddenTripplePruner();
            var preCount = context.Cardinalities.Sum(x => x.Possibilities);

            // ACT
            while (pruner1.Prune(context)) { }
            context.Cardinalities = Preprocessor.GenerateCardinalities(context.Board, context.Candidates);

            // ASSERT
            Assert.AreEqual(expectedChange, preCount - context.Cardinalities.Sum(x => x.Possibilities));
        }
    }
}
