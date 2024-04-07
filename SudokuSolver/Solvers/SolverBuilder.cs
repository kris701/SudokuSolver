using SudokuSolver.Preprocessors;
using SudokuSolver.Solvers.BacktrackSolvers;
using SudokuSolver.Solvers.GuidedBacktrackSolvers;

namespace SudokuSolver.Solvers
{
    [Flags]
    public enum SolverOptions { None, BackTrack, GuidedBackTrack }

    public static class SolverBuilder
    {
        private static readonly Dictionary<SolverOptions, Func<IPreprocessor, ISolver>> _solvers = new Dictionary<SolverOptions, Func<IPreprocessor, ISolver>>()
        {
            { SolverOptions.BackTrack, (p) => new BacktrackSolver(p) },
            { SolverOptions.GuidedBackTrack, (p) => new GuidedBacktrackSolver(p) },
        };

        public static ISolver GetSolver(SolverOptions solver, IPreprocessor preprocessor) => _solvers[solver](preprocessor);
    }
}
