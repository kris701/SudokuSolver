using SudokuSolver.Models;
using SudokuSolver.Solvers.Algorithms.LogicSolvers;
using SudokuSolver.Solvers.Algorithms.LogicSolvers.LogicPruners;
using SudokuSolver.Solvers.Preprocessors;

namespace SudokuSolver.Tests.Solvers.Algorithms.LogicSolvers.LogicPruners
{
    [TestClass]
    public class NakedPairPrunerTests
    {
        [TestMethod]
        [DataRow("400000938032094100095300240370609004529001673604703090957008300003900400240030709", 13)]
        [DataRow("080090030030000069902063158020804590851907046394605870563040987200000015010050020", 9)]
        public void Can_PruneCorrectly(string board, int expectedChange)
        {
            // ARRANGE
            var context = Preprocessor.Preprocess(new SudokuBoard(board));
            IPruner pruner1 = new NakedPairPruner();
            var solver = new LogicSolver(new List<IPruner>()
            {
                new CertainsPruner(),
                pruner1,
                new NakedTripplePruner(),
                new HiddenPairPruner(),
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
