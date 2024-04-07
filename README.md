# Sudoku Solver
This is a simple project about making automatic Sudoku solvers.
It can be run as a CLI tool, by running:

`dotnet run --project SudokuSolver --configuration Release -- --board {BOARD} --size {BOARDCELLSIZE} --solver {SOLVER} --configuration {CONFIGURATION}`

Where:
* `BOARD` has to be a string of rows in the sudoku, where 0 represents a blank space
* `BOARDCELLSIZE` is the size of the board cells (3 in normal sudoku)
* `SOLVER` is the solver you want. The CLI tool will tell you what options are available
* `CONFIGURATION` is the solver configuration. Different solvers can have different configurations.

The preprocessor step for the sudoku solvers remove as many candidates as it can, such as hidden pairs and naked pairs.

# Experiments
There are some benchmark sudokus that can be found in the Benchmark folder.
Currently, the `BackTrack` solver can solve all the instances in there with a time limit of 10 seconds (though most are solved way before that).
The experiments can be run by writing `dotnet test --configuration Release`.
