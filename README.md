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
Benchmark is run on 1971 different Sudoku boards with a 5s time limit.


| Solver | Sudokus Solved | Max Search Time (ms) | Min Search Time (ms) | Average Search Time (ms) | Max Calls | Min Calls | Average Calls |
| - | - | - | - | - | - | - | - |
| BruteForceBacktrack | 1968 | 2686.53 | 0.01 | 22.09 | 85713756 | 43 | 507968.93 |
| LogicalWithBruteForceBacktrack | 1971 | 156.58 | 0.11 | 3.64 | 3467757 | 2 | 48381.09 |
| CardinalityBacktrack | 1957 | 4916.15 | 0.02 | 113.86 | 144169437 | 42 | 3726145.61 |
| LogicalWithCardinalityBacktrack | 1968 | 4700.23 | 0.11 | 62.66 | 143331310 | 2 | 1853831.48 |
| Logical | 980 | 6.05 | 0.11 | 0.71 | 21 | 0 | 2.31 |
