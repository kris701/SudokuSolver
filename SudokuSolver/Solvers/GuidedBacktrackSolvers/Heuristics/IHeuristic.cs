using SudokuSolver.Models;
using SudokuSolver.Preprocessors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Solvers.GuidedBacktrackSolvers.Heuristics
{
    public interface IHeuristic
    {
        public int Value(SudokuBoard board, IPreprocessor preprocessor, CellAssignment assignment);
    }
}
