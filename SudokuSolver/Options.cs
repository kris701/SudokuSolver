using CommandLine;
using SudokuSolver.Solvers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    public class Options
    {
        [Option("board", Required = true, HelpText = "The soduko board in a one dimentional array, row after row.")]
        public string Board { get; set; } = "";
        [Option("solver", Required = true, HelpText = "What solver to use.")]
        public Solvers.Solvers Solver { get; set; }

        [Option("configuration", Required = false, HelpText = "Configuration to set the solver to.")]
        public string Configuration { get; set; } = "";
        [Option("timeout", Required = false, HelpText = "How many seconds should the search be allowed to use.")]
        public int TimeOutS { get; set; } = -1;
    }
}
