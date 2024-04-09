using SudokuSolver.Models;
using SudokuSolver.Solvers.BacktrackSolvers.Pruners;
using SudokuSolver.Solvers.Preprocessors;

namespace SudokuSolver.Tests.Solvers.BacktrackSolvers.Pruners
{
    [TestClass]
    public class CertainsPrunerTests
    {
        [TestMethod]
        [DataRow("600000803040700000000000000000504070300200000106000000020000050000080600000010000", 30)]
        [DataRow("052400000000070100000000000000802000300000600090500000106030000000000089700000000", 21)]
        public void Can_PruneCorrectly(string board, int expectedChange)
        {
            // ARRANGE
            var values = new List<byte>();
            foreach (var c in board)
                values.Add(byte.Parse($"{c}"));
            var context = Preprocessor.Preprocess(new SudokuBoard(values.ToArray()));
            IPruner pruner1 = new CertainsPruner();
            var preCount = context.Cardinalities.Sum(x => x.Possibilities);

            // ACT
            while (pruner1.Prune(context)) { }
            context.Cardinalities = Preprocessor.GenerateCardinalities(context.Board, context.Candidates);

            // ASSERT
            Assert.AreEqual(expectedChange, preCount - context.Cardinalities.Sum(x => x.Possibilities));
        }
    }
}
