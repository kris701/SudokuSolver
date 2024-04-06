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
            var values = new List<byte>();
            foreach (var c in opts.Board)
                values.Add(byte.Parse($"{c}"));

            if (values.Count % opts.BlockSize != 0)
                throw new Exception("Blocksize is not divisible with the board values!");

            var board = new SudokuBoard(values.ToArray(), (byte)opts.BlockSize);

            Console.WriteLine("Initial board:");
            Console.WriteLine(board.ToString());
            Console.WriteLine();
            Console.WriteLine($"Solving with '{Enum.GetName(typeof(Solvers.Solvers), opts.Solver)}' solver");
            var solver = SolverBuilder.GetSolver(opts.Solver);
            if (opts.Configuration != "")
            {
                Console.WriteLine($"\tConfiguration set to '{opts.Configuration}'");
                solver.Configuration = opts.Configuration;
            }
            if (opts.TimeOutS != -1)
            {
                Console.WriteLine($"\tTimeout set to {opts.TimeOutS}s");
                solver.Timeout = TimeSpan.FromSeconds(opts.TimeOutS);
            }
            Console.WriteLine("Starting...");
            Console.WriteLine();
            var result = solver.Solve(board);
            Console.WriteLine();
            if (solver.TimedOut)
            {
                Console.WriteLine("Solver timed out...");
            }
            else if (result != null)
            {
                Console.WriteLine("Board solved!");
                Console.WriteLine("Solved board:");
                Console.WriteLine(result.ToString());
            }
            else
                Console.WriteLine("Board is unsolvable!");
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