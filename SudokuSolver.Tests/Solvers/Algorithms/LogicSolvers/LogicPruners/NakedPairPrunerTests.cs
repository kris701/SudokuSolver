using SudokuSolver.Models;
using SudokuSolver.Solvers.Algorithms.LogicSolvers.LogicPruners;
using SudokuSolver.Solvers.Preprocessors;

namespace SudokuSolver.Tests.Solvers.Algorithms.LogicSolvers.LogicPruners
{
    [TestClass]
    public class NakedPairPrunerTests : BaseLogicPrunerTest
    {
        [TestMethod]
        [DataRow("400000938032094100095300240370609004529001673604703090957008300003900400240030709", 18)]
        [DataRow("080090030030000069902063158020804590851907046394605870563040987200000015010050020", 9)]
        public void Can_PruneCorrectly(string board, int expectedChange)
        {
            // ARRANGE
            var values = new List<byte>();
            foreach (var c in board)
                values.Add(byte.Parse($"{c}"));
            var context = Preprocessor.Preprocess(new SudokuBoard(values.ToArray()));
            IPruner pruner1 = new NakedPairPruner();
            var preCount = GetCardinality(context.Candidates);

            // ACT
            while (pruner1.Prune(context)) { }

            // ASSERT
            Assert.AreEqual(expectedChange, preCount - GetCardinality(context.Candidates));
        }
    }
}
