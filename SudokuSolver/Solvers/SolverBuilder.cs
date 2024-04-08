using SudokuSolver.Preprocessors;
using SudokuSolver.Solvers.BacktrackSolvers;

namespace SudokuSolver.Solvers
{
    [Flags]
    public enum SolverOptions { None, BackTrack }

    public static class SolverBuilder
    {
        private static readonly Dictionary<SolverOptions, Func<IPreprocessor, ISolver>> _solvers = new Dictionary<SolverOptions, Func<IPreprocessor, ISolver>>()
        {
            { SolverOptions.BackTrack, (p) => new BacktrackSolver(p) },
        };

        public static ISolver GetSolver(SolverOptions solver, IPreprocessor preprocessor) => _solvers[solver](preprocessor);
    }
}
