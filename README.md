# Sudoku Solver
This is a simple project about making an automatic Sudoku solver.
It can be run as a CLI tool, by running:

`dotnet run --project SudokuSolver --configuration Release -- --board {BOARD} --solver {SOLVER}`

Where:
* `BOARD` has to be a string of rows in the sudoku (total of 81 numbers), where 0 represents a blank space
* `SOLVER` is the solver you want. The CLI tool will tell you what options are available

# Experiments
There are some benchmark sudokus that can be found in the Benchmark folder.
Currently, the `BackTrack` solver can solve all the instances in there with a time limit of 10 seconds (though most are solved way before that).
The experiments can be run by writing `dotnet test --configuration Release`.

# Performance
Benchmark is run on 460 different Sudoku boards with a 5s time limit.


| Solver | Sudokus Solved | Max Search Time (ms) | Min Search Time (ms) | Average Search Time (ms) |
| - | - | - | - | - |
| CardinalityBacktrack | 454 | 4448.65 | 0 | 126.44 |
| LogicalWithCardinalityBacktrack | 460 | 2461.32 | 0 | 39.01 |
| Logical | 222 | 14.55 | 0 | 1.83 |
| BruteForceBacktrack | 0 | 0 | 0 | 0 |
