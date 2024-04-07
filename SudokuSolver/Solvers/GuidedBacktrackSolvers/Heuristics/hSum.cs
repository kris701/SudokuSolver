using SudokuSolver.Models;
using SudokuSolver.Preprocessors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Solvers.GuidedBacktrackSolvers.Heuristics
{
    public class hSum : IHeuristic
    {
        public List<IHeuristic> Heuristics { get; set; }

        public hSum(List<IHeuristic> heuristics)
        {
            Heuristics = heuristics;
        }

        public int Value(SudokuBoard board, IPreprocessor preprocessor, CellAssignment assignment)
        {
            var value = 0;
            foreach (var heuristic in Heuristics)
                value += heuristic.Value(board, preprocessor, assignment);
            return value;
        }
    }
}
