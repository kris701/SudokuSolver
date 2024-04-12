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
| SequentialBacktrack | 1967 | 1890.12 | 0.02 | 27.88 | 32280614 | 43 | 415761.72 |
| CardinalityBacktrack | 1970 | 1534.58 | 0.03 | 14.05 | 27894422 | 42 | 201274.26 |
| Logical | 982 | 17.43 | 0.31 | 2.54 | 21 | 0 | 2.33 |
| LogicalWithSequentialBacktrack | 1971 | 123.4 | 0.14 | 3.86 | 1903421 | 2 | 43845.58 |
| LogicalWithCardinalityBacktrack | 1971 | 64.47 | 0.15 | 2.87 | 1156304 | 2 | 28627.22 |
| LogicalWithRandomBacktrack | 1377 | 1970.49 | 0.15 | 68.56 | 56647667 | 2 | 15148498.02 |
