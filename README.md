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
| RandomBacktrack | 363 | 2005.2 | 0.05 | 417.95 | 51983571 | 185 | 34942615.26 |
| LogicalWithRandomBacktrack | 1350 | 1999.53 | 0.12 | 49.03 | 51073228 | 2 | 13771623.02 |
| SequentialBacktrack | 1967 | 1890.12 | 0.02 | 27.88 | 32280614 | 43 | 415761.72 |
| LogicalWithSequentialBacktrack | 1971 | 206.77 | 0.13 | 4.92 | 3467757 | 2 | 48398.96 |
| CardinalityBacktrack | 1970 | 1534.58 | 0.03 | 14.05 | 27894422 | 42 | 201274.26 |
| LogicalWithCardinalityBacktrack | 1971 | 341.35 | 0.13 | 3.79 | 4286183 | 2 | 30773.05 |
| Logical | 980 | 6.91 | 0.12 | 1.13 | 21 | 0 | 2.31 |
