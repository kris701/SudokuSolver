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
| SequentialBacktrack | 1967 | 1561.8 | 0.02 | 19.66 | 39587512 | 43 | 431681.88 |
| LogicalWithSequentialBacktrack | 1971 | 361.49 | 0.11 | 3.87 | 3467757 | 2 | 48381.09 |
| Logical | 980 | 6.52 | 0.11 | 0.68 | 21 | 0 | 2.31 |
| RandomBacktrack | 202 | 1958.63 | 0.07 | 433.67 | 21840169 | 0 | 16689656.29 |
| LogicalWithRandomBacktrack | 1295 | 1989.25 | 0.11 | 40.31 | 21498951 | 0 | 6720735.16 |
| CardinalityBacktrack | 1970 | 1938.93 | 0.02 | 16.77 | 28219401 | 42 | 201439.14 |
| LogicalWithCardinalityBacktrack | 1971 | 308.78 | 0.12 | 3.82 | 4286183 | 2 | 30773.05 |
