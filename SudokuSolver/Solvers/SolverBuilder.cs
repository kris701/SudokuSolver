using SudokuSolver.Solvers.Algorithms;
using SudokuSolver.Solvers.Algorithms.BacktrackSolvers;
using SudokuSolver.Solvers.Algorithms.LogicSolvers;
using SudokuSolver.Solvers.Algorithms.LogicSolvers.LogicPruners;

namespace SudokuSolver.Solvers
{
    [Flags]
    public enum SolverOptions
    {
        SequentialBacktrack,
        CardinalityBacktrack,
        RandomBacktrack,
        Logical,
        LogicalWithSequentialBacktrack,
        LogicalWithCardinalityBacktrack,
        LogicalWithRandomBacktrack
    }

    public static class SolverBuilder
    {
        private static readonly Dictionary<SolverOptions, Func<SolverContainer>> _solvers = new Dictionary<SolverOptions, Func<SolverContainer>>()
        {
            { SolverOptions.SequentialBacktrack, () => new SolverContainer(new List<IAlgorithm>()
            {
                new SequentialBacktrackSolver()
            }) },
            { SolverOptions.CardinalityBacktrack, () => new SolverContainer(new List<IAlgorithm>()
            {
                new CardinalityBacktrackSolver()
            }) },
            { SolverOptions.RandomBacktrack, () => new SolverContainer(new List<IAlgorithm>()
            {
                new RandomBacktrackSolver()
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
                    new PointingPairsPruner(),
                    new BoxLineReductionPruner()
                })
            }) },
            { SolverOptions.LogicalWithSequentialBacktrack, () => new SolverContainer(new List<IAlgorithm>()
            {
                new LogicSolver(new List<IPruner>()
                {
                    new CertainsPruner(),
                    new NakedPairPruner(),
                    new NakedTripplePruner(),
                    new HiddenPairPruner(),
                    new HiddenTripplePruner(),
                    new PointingPairsPruner(),
                    new BoxLineReductionPruner()
                }),
                new SequentialBacktrackSolver()
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
                    new PointingPairsPruner(),
                    new BoxLineReductionPruner()
                }),
                new CardinalityBacktrackSolver()
            }) },
            { SolverOptions.LogicalWithRandomBacktrack, () => new SolverContainer(new List<IAlgorithm>()
            {
                new LogicSolver(new List<IPruner>()
                {
                    new CertainsPruner(),
                    new NakedPairPruner(),
                    new NakedTripplePruner(),
                    new HiddenPairPruner(),
                    new HiddenTripplePruner(),
                    new PointingPairsPruner(),
                    new BoxLineReductionPruner()
                }),
                new RandomBacktrackSolver()
            }) }
        };

        public static SolverContainer GetSolver(SolverOptions solver) => _solvers[solver]();
    }
}
