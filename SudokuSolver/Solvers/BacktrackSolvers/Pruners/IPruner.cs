using SudokuSolver.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Solvers.BacktrackSolvers.Reducers
{
    public interface IPruner
    {
        public bool Prune(SearchContext context);
    }
}
