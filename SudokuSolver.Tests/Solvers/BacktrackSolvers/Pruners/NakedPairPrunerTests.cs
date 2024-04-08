﻿using SudokuSolver.Models;
using SudokuSolver.Solvers.BacktrackSolvers.Pruners;
using SudokuSolver.Solvers.BacktrackSolvers.Reducers;
using SudokuSolver.Solvers.Preprocessors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Tests.Solvers.BacktrackSolvers.Pruners
{
    [TestClass]
    public class NakedPairPrunerTests
    {
        [TestMethod]
        [DataRow("400000938032094100095300240370609004529001673604703090957008300003900400240030709", 3, 18)]
        [DataRow("080090030030000069902063158020804590851907046394605870563040987200000015010050020", 3, 9)]
        public void Can_PruneCorrectly(string board, int boardSize, int expectedChange)
        {
            // ARRANGE
            var values = new List<byte>();
            foreach (var c in board)
                values.Add(byte.Parse($"{c}"));
            var context = Preprocessor.Preprocess(new SudokuBoard(values.ToArray(), (byte)boardSize));
            IPruner pruner1 = new NakedPairPruner();
            var preCount = context.Cardinalities.Sum(x => x.Possibilities);

            // ACT
            while (pruner1.Prune(context)) { }
            context.Cardinalities = Preprocessor.GenerateCardinalities(context.Board, context.Candidates);

            // ASSERT
            Assert.AreEqual(expectedChange, preCount - context.Cardinalities.Sum(x => x.Possibilities));
        }
    }
}