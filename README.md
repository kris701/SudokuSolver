# Sudoku Solver
This is a simple project about making an automatic Sudoku solver.
It can be run as a CLI tool, by running:

`dotnet run --project SudokuSolver --configuration Release -- --board {BOARD} --solver {SOLVER}`

Where:
* `BOARD` has to be a string of rows in the sudoku (total of 81 numbers), where 0 represents a blank space
* `SOLVER` is the solver you want. The CLI tool will tell you what options are available

# Performance
Benchmark is run on 1971 different Sudoku boards with a 2s time limit.


| Solver | Sudokus Solved | Max Search Time (ms) | Min Search Time (ms) | Average Search Time (ms) | Max Calls | Min Calls | Average Calls |
| - | - | - | - | - | - | - | - |
| Logical | 980 | 6.52 | 0.11 | 0.68 | 21 | 0 | 2.31 |
| CardinalityBacktrack | 1970 | 1938.93 | 0.02 | 16.77 | 28219401 | 42 | 201439.14 |
| LogicalWithCardinalityBacktrack | 1971 | 308.78 | 0.12 | 3.82 | 4286183 | 2 | 30773.05 |
| RandomBacktrack | 363 | 2005.2 | 0.05 | 417.95 | 51983571 | 185 | 34942615.26 |
| LogicalWithRandomBacktrack | 1350 | 1999.53 | 0.12 | 49.03 | 51073228 | 2 | 13771623.02 |
| SequentialBacktrack | 1965 | 1997.02 | 0.02 | 27.38 | 32036149 | 43 | 416622.66 |
| LogicalWithSequentialBacktrack | 1970 | 232.95 | 0.11 | 5.62 | 3467757 | 2 | 48380.88 |
