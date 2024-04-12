using CommandLine;
using SudokuSolver.Solvers;

namespace SudokuSolver
{
    public class Options
    {
        [Option("board", Required = true, HelpText = "The soduko board in a one dimentional array, row after row.")]
        public string Board { get; set; } = "";
        [Option("solver", Required = true, HelpText = "What solver to use.")]
        public SolverOptions Solver { get; set; }

        [Option("timeout", Required = false, HelpText = "How many seconds should the search be allowed to use.", Default = -1)]
        public int TimeOutS { get; set; } = -1;
        [Option("solution", Required = false, HelpText = "The file to output the solution to. Empty means no file will be made.")]
        public string SolutionFile { get; set; } = "";
    }
}
