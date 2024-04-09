using SudokuSolver.Models;
using SudokuSolver.Solvers.BacktrackSolvers.Pruners;
using SudokuSolver.Solvers.Preprocessors;

namespace SudokuSolver.Tests.Solvers.BacktrackSolvers.Pruners
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
            var values = new List<byte>();
            foreach (var c in board)
                values.Add(byte.Parse($"{c}"));
            var context = Preprocessor.Preprocess(new SudokuBoard(values.ToArray()));
            IPruner pruner1 = new HiddenPairPruner();
            var preCount = context.Cardinalities.Sum(x => x.Possibilities);

            // ACT
            while (pruner1.Prune(context)) { }
            context.Cardinalities = Preprocessor.GenerateCardinalities(context.Board, context.Candidates);

            // ASSERT
            Assert.AreEqual(expectedChange, preCount - context.Cardinalities.Sum(x => x.Possibilities));
        }
    }
}
