using SudokuSolver.Solvers.BacktrackSolvers;

namespace SudokuSolver.Solvers
{
    [Flags]
    public enum SolverOptions { None, BackTrack }

    public static class SolverBuilder
    {
        private static readonly Dictionary<SolverOptions, Func<ISolver>> _solvers = new Dictionary<SolverOptions, Func<ISolver>>()
        {
            { SolverOptions.BackTrack, () => new BacktrackSolver() },
        };

        public static ISolver GetSolver(SolverOptions solver) => _solvers[solver]();
    }
}
