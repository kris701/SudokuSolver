using CommandLine;
using SudokuSolver.Preprocessors;
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

        [Option("configuration", Required = false, HelpText = "Configuration to set the solver to.")]
        public string Configuration { get; set; } = "";
        [Option("preprocessor", Required = false, HelpText = "What preprocessor to use.")]
        public PreprocessorOptions Preprocessor { get; set; } = PreprocessorOptions.Full;
        [Option("timeout", Required = false, HelpText = "How many seconds should the search be allowed to use.")]
        public int TimeOutS { get; set; } = -1;
    }
}
