using SudokuSolver.Solvers.Algorithms;
using SudokuSolver.Solvers.Algorithms.BacktrackSolvers;
using SudokuSolver.Solvers.Algorithms.LogicSolvers;
using SudokuSolver.Solvers.Algorithms.LogicSolvers.LogicPruners;

namespace SudokuSolver.Solvers
{
    [Flags]
    public enum SolverOptions { BruteForceBacktrack, CardinalityBacktrack, Logical, LogicalWithBruteForceBacktrack, LogicalWithCardinalityBacktrack }

    public static class SolverBuilder
    {
        private static readonly Dictionary<SolverOptions, Func<SolverContainer>> _solvers = new Dictionary<SolverOptions, Func<SolverContainer>>()
        {
            { SolverOptions.BruteForceBacktrack, () => new SolverContainer(new List<IAlgorithm>()
            {
                new BruteForceBacktrackSolver()
            }) },
            { SolverOptions.CardinalityBacktrack, () => new SolverContainer(new List<IAlgorithm>()
            {
                new CardinalityBacktrackSolver()
            }) },
            { SolverOptions.Logical, () => new SolverContainer(new List<IAlgorithm>()
            {
                new LogicSolver(new List<IPruner>()
                {
                    new CertainsPruner(),
                    new NakedPairPruner(),
                    new NakedTripplePruner(),
                    new HiddenPairPruner(),
                    new HiddenTripplePruner(),
                    new PointingPairsPruner()
                })
            }) },
            { SolverOptions.LogicalWithBruteForceBacktrack, () => new SolverContainer(new List<IAlgorithm>()
            {
                new LogicSolver(new List<IPruner>()
                {
                    new CertainsPruner(),
                    new NakedPairPruner(),
                    new NakedTripplePruner(),
                    new HiddenPairPruner(),
                    new HiddenTripplePruner(),
                    new PointingPairsPruner()
                }),
                new BruteForceBacktrackSolver()
            }) },
            { SolverOptions.LogicalWithCardinalityBacktrack, () => new SolverContainer(new List<IAlgorithm>()
            {
                new LogicSolver(new List<IPruner>()
                {
                    new CertainsPruner(),
                    new NakedPairPruner(),
                    new NakedTripplePruner(),
                    new HiddenPairPruner(),
                    new HiddenTripplePruner(),
                    new PointingPairsPruner()
                }),
                new CardinalityBacktrackSolver()
            }) }
        };

        public static SolverContainer GetSolver(SolverOptions solver) => _solvers[solver]();
    }
}
