using SudokuSolver.Models;
using System.Diagnostics;
using System.Timers;

namespace SudokuSolver.Solvers.BacktrackSolvers
{
    public class BacktrackSolver : BaseSolver
    {
        private static readonly Dictionary<string, SearchOptions> _configurations = new Dictionary<string, SearchOptions>()
        {
            { "Default", new SearchOptions() },
            { "NonOptimised", new SearchOptions(){
                GroundLegalCandidatesOnly = false,
                PruneCertains = false,
                PruneHiddenPairs = false,
                PruneNakedPairs = false
            } }
        };
        public override List<string> Configurations() => _configurations.Keys.ToList();

        public SearchOptions Options { get; set; } = new SearchOptions();
        private string _configuration = "Default";

        public override string Configuration {
            get { return _configuration; } 
            set {
                if (!_configurations.ContainsKey(value))
                    throw new BacktrackSolverException($"'{value}' is an invalid configuration for this solver! Options are: {string.Join(",", Configurations())}");
                Options = _configurations[value];
                _configuration = value;
            } 
        }

        public override SudokuBoard? Run(SudokuBoard board)
        {
            var preprocessor = new Preprocessor(Options, _size);
            Console.WriteLine($"Preprocessing...");
            var processed = preprocessor.Preprocess(board);
            Console.WriteLine($"Preprocess complete!");
            if (preprocessor.Failed)
                return null;
            Console.WriteLine($"Solving...");
            return SolveInner(processed, preprocessor);
        }

        private SudokuBoard? SolveInner(SudokuBoard board, Preprocessor preprocessor, int bestOffset = 0)
        {
            if (_stop)
                return null;

            Calls++;

            if (bestOffset >= preprocessor.Cardinalities.Count)
            {
                if (board.IsComplete())
                    return board;
                return null;
            }

            var loc = preprocessor.Cardinalities[bestOffset];
            var possibilities = preprocessor.Candidates[loc.X, loc.Y];
            var count = possibilities.Count;
            for (int i = 0; i < count; i++)
            {
                if (possibilities[i].IsLegal(board))
                {
                    possibilities[i].Apply(board);
                    var result = SolveInner(board, preprocessor, bestOffset + 1);
                    if (result != null)
                        return result;
                    possibilities[i].UnApply(board);
                }
            }
            return null;
        }
    }
}
