using SudokuSolver.Models;
using SudokuSolver.Solvers.Algorithms.LogicSolvers.LogicPruners;
using SudokuSolver.Solvers.Preprocessors;

namespace SudokuSolver.Tests.Solvers.Algorithms.LogicSolvers.LogicPruners
{
    [TestClass]
    public class HiddenPairPrunerTests : BaseLogicPrunerTest
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
            var preCount = GetCardinality(context.Candidates);

            // ACT
            while (pruner1.Prune(context)) { }

            // ASSERT
            Assert.AreEqual(expectedChange, preCount - GetCardinality(context.Candidates));
        }
    }
}
