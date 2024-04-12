# Sudoku Solver
This is a simple project about making an automatic Sudoku solver.
It can be run as a CLI tool, by running:

`dotnet run --project SudokuSolver --configuration Release -- --board {BOARD} --solver {SOLVER} --solution {SOLUTIONFILE}`

Where:
* `BOARD` has to be a string of rows in the sudoku (total of 81 numbers), where 0 represents a blank space
* `SOLVER` is the solver you want. The CLI tool will tell you what options are available
* `SOLUTION` is optional. It describes the file you want the solved board to be outputted to. If none is given, no file will be made.

# Performance
Benchmark is run on 1971 different Sudoku boards with a 2s time limit.
The results are ordered by solved instances, then by lowest average search time and finally by average calls.

| Solver | **Solved** | **Avg Search (ms)** | **Avg Calls** | Max Search (ms) | Min Search (ms) | Max Calls | Min Calls |
| - | - | - | - | - | - | - | - |
| LogicalWithCardinalityBacktrack | 1971 | 4.5 | 28627.22 | 140.82 | 0.14 | 1156304 | 2 |
| LogicalWithSequentialBacktrack | 1971 | 5.83 | 43845.58 | 165.72 | 0.15 | 1903421 | 2 |
| CardinalityBacktrack | 1970 | 14.05 | 201274.26 | 1534.58 | 0.03 | 27894422 | 42 |
| SequentialBacktrack | 1967 | 27.88 | 415761.72 | 1890.12 | 0.02 | 32280614 | 43 |
| LogicalWithRandomBacktrack | 1377 | 68.56 | 15148498.02 | 1970.49 | 0.15 | 56647667 | 2 |
| Logical | 978 | 2.13 | 4.41 | 17.96 | 0.27 | 21 | 1 |
| RandomBacktrack | 363 | 417.95 | 34942615.26 | 2005.2 | 0.05 | 51983571 | 185 |
