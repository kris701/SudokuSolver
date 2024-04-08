using CommandLine;
using SudokuSolver.Solvers;

namespace SudokuSolver
{
    public class Options
    {
        [Option("board", Required = true, HelpText = "The soduko board in a one dimentional array, row after row.")]
        public string Board { get; set; } = "";
        [Option("size", Required = true, HelpText = "The size of the blocks in the puzzle.")]
        public int BlockSize { get; set; }
        [Option("solver", Required = true, HelpText = "What solver to use.")]
        public SolverOptions Solver { get; set; }

        [Option("timeout", Required = false, HelpText = "How many seconds should the search be allowed to use.")]
        public int TimeOutS { get; set; } = -1;
    }
}
