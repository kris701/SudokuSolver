using CommandLine;
using CommandLine.Text;
using SudokuSolver.Models;
using SudokuSolver.Solvers;

namespace SudokuSolver
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var parser = new Parser(with => with.HelpWriter = null);
            var parserResult = parser.ParseArguments<Options>(args);
            parserResult.WithNotParsed(errs => DisplayHelp(parserResult, errs));
            parserResult.WithParsed(Run);
        }

        public static void Run(Options opts)
        {
            if (opts.Board.Length != 81)
                throw new Exception("Board values must be exactly 81 characters long");

            var board = new SudokuBoard(opts.Board);

            if (opts.SolutionFile != "" && File.Exists(opts.SolutionFile))
                File.Delete(opts.SolutionFile);

            Console.WriteLine("Initial board:");
            Console.WriteLine(board.ToString());
            Console.WriteLine();
            Console.WriteLine($"Solving with '{Enum.GetName(typeof(SolverOptions), opts.Solver)}' solver");
            var solver = SolverBuilder.GetSolver(opts.Solver);
            if (opts.TimeOutS != -1)
            {
                Console.WriteLine($"\tTimeout set to {opts.TimeOutS}s");
                solver.Timeout = TimeSpan.FromSeconds(opts.TimeOutS);
            }
            Console.WriteLine("Starting...");
            Console.WriteLine();
            var result = solver.Solve(board);
            Console.WriteLine();
            if (solver.Stop)
            {
                Console.WriteLine("Solver timed out...");
            }
            else if (result != null)
            {
                Console.WriteLine("Board solved!");
                Console.WriteLine("Solved board:");
                Console.WriteLine(result.ToString());
                if (opts.SolutionFile != "")
                    File.WriteAllText(opts.SolutionFile, result.GetBoard());
            }
            else
                Console.WriteLine("Board is unsolvable with given solver!");
        }

        public static void HandleParseError(IEnumerable<Error> errs)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            var sentenceBuilder = SentenceBuilder.Create();
            foreach (var error in errs)
                if (error is not HelpRequestedError)
                    Console.WriteLine(sentenceBuilder.FormatError(error));
            Console.ResetColor();
        }

        public static void DisplayHelp<T>(ParserResult<T> result, IEnumerable<Error> errs)
        {
            var helpText = HelpText.AutoBuild(result, h =>
            {
                h.AddEnumValuesToHelpText = true;
                return h;
            }, e => e, verbsIndex: true);
            Console.WriteLine(helpText);
            HandleParseError(errs);
        }
    }
}