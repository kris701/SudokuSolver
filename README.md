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
| BruteForceBacktrack | 1968 | 3202.37 | 0.02 | 27.84 | 82811709 | 43 | 483057.07 |
| LogicalWithBruteForceBacktrack | 1971 | 175.8 | 0.1 | 4.76 | 3467757 | 2 | 56147.83 |
| CardinalityBacktrack | 1953 | 4878.51 | 0.02 | 128.79 | 117278932 | 42 | 3516241.68 |
| LogicalWithCardinalityBacktrack | 1966 | 4586.75 | 0.09 | 70.96 | 121007670 | 2 | 1835779.05 |
| Logical | 906 | 17.58 | 0.22 | 2.01 | 21 | 0 | 1.97 |
