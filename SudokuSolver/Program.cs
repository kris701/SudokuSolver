using SudokuToolsSharp.Models;
using SudokuToolsSharp.Solvers.BacktrackSolvers;
using System;

namespace SudokuToolsSharp
{
    internal class Program
    {
        private static List<Benchmark> _benchmarks = new List<Benchmark>()
        {
            new Benchmark("../../../../Benchmarks/11puzzles.txt", 3),
            new Benchmark("../../../../Benchmarks/timan.txt", 3),
            new Benchmark("../../../../Benchmarks/mypuzzles.txt", 3),
            new Benchmark("../../../../Benchmarks/2x2.txt", 2),
            new Benchmark("../../../../Benchmarks/95puzzles.txt", 3),
        };

        static void Main(string[] args)
        {
            //SingleBenchmarkAverage(_benchmarks[0]);
            //return;

            var count = 1;
            foreach (var benchmark in _benchmarks)
            {
                Console.WriteLine($"Benchmark set {count++} out of {_benchmarks.Count}");
                RunBenchmark(benchmark);
            }
        }

        private static void SingleBenchmarkAverage(Benchmark benchmark)
        {
            if (!File.Exists(benchmark.File))
                throw new Exception();

            var lines = File.ReadAllLines(benchmark.File);
            var values = new List<int>();
            foreach (var c in lines[2])
                values.Add(int.Parse($"{c}"));
            var board = new SudokuBoard(values.ToArray(), benchmark.CellSize);
            var solver = new BacktrackSolver(benchmark.CellSize * benchmark.CellSize)
            {
                Timeout = TimeSpan.FromSeconds(10)
            };
            solver.Solve(board);
            Console.WriteLine($"Total Calls: {solver.Calls}");
        }

        private static void RunBenchmark(Benchmark benchmark)
        {
            if (!File.Exists(benchmark.File))
                throw new Exception();

            double totalCalls = 0;
            TimeSpan totalTime = TimeSpan.Zero;

            var lines = File.ReadAllLines(benchmark.File);
            int count = 1;
            foreach (var line in lines)
            {
                if (line == "")
                    continue;

                Console.WriteLine($"\tRun {count++} out of {lines.Length}");

                var values = new List<int>();
                foreach (var c in line)
                    values.Add(int.Parse($"{c}"));
                var board = new SudokuBoard(values.ToArray(), benchmark.CellSize);

                var solver = new BacktrackSolver(benchmark.CellSize * benchmark.CellSize);
                var result = solver.Solve(board);
                if (result == null)
                    throw new Exception("Unsolvable");
                totalCalls += solver.Calls;
                totalTime += solver.SearchTime;
            }
            Console.WriteLine();
            Console.WriteLine($"\tAvr calls: {Math.Round(totalCalls / lines.Length, 2)}, Avr time: {totalTime / lines.Length}");
            Console.WriteLine();
        }
    }
}