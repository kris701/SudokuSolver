using SudokuSolver.Solvers.BacktrackSolvers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Solvers
{
    [Flags]
    public enum Solvers { None, BackTrack }

    public static class SolverBuilder
    {
        private static Dictionary<Solvers, Func<ISolver>> _solvers = new Dictionary<Solvers, Func<ISolver>>()
        {
            { Solvers.BackTrack, () => new BacktrackSolver() }
        };

        public static ISolver GetSolver(Solvers solver) => _solvers[solver]();
    }
}
