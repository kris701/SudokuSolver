# Sudoku Solver
This is a simple project about making an automatic Sudoku solver.
It can be run as a CLI tool, by running:

`dotnet run --project SudokuSolver --configuration Release -- --board {BOARD} --solver {SOLVER} --solution {SOLUTIONFILE}`

Where:
* `BOARD` has to be a string of rows in the sudoku (total of 81 numbers), where 0 represents a blank space
* `SOLVER` is the solver you want. The CLI tool will tell you what options are available
* `SOLUTION` is optional. It describes the file you want the solved board to be outputted to. If none is given, no file will be made.

# Solvers
There are four core solvers in this projects, three brute force (B) and one logical (L).
There are also presets that combine some of these solvers, to simplify the job for the following solver.
## Random Backtrack (B)
This solver is a sort of a "baseline" solver. It is pretty bad, since it selects what cell to try and fill out in random order.
This leads to a lot of weird and bad branches that simply requires more time and a lot more calls.

## Sequential Backtrack (B)
Sequential backtrack selects cells sequentially, column by row.
It can also be visualised in the following example, where the sequence is from A until the end of the board (thanks to [SudokuWiki.org](https://www.sudokuwiki.org/) for the image)

![image](https://github.com/kris701/SudokuSolver/assets/22596587/5cc714bf-0cae-4809-8050-a074cde1d6ee)

Where it will try and assign all legal values to the A cell first, then to the B cell, etc.
If a cell becomes impossible to assign to, we backtrack to the previous cell.

## Cardinality Backtrack (B)
This is a backtrack solver that selects cells based on the cardinality of possibilities for rows.
It sorts its possible cells to select first by fewest possibilities for the entire row and then sort it by the cell in that row that have the fewest assignment possibilities itself.
As an example, one can look at the following image (the small numbers mean the numbers that can legally be assigned to that cell):

![image](https://github.com/kris701/SudokuSolver/assets/22596587/62928f4d-dc54-4013-9a62-cfe14c0a0827)

It can be seen that it is row F that have the fewest total possible assignments, so that row takes priority.
When it comes to selecting a cell in that row, we take the cell in the row that have the fewest possibilities too.
So in this case we will try F1 and F6 before we try F9.

## Logical (L)
This is perhaps the most interesting solver of them all.
It attempts to solve the board by means of logical deduction.
The set of logical strategies that this solver currently implements are:
* [Last Remaining Cell in Box/Row/Column](https://www.sudokuwiki.org/Getting_Started)
* [Naked Pairs](https://www.sudokuwiki.org/Naked_Candidates)
* [Naked Tripples](https://www.sudokuwiki.org/Naked_Candidates)
* [Hidden Pairs](https://www.sudokuwiki.org/Hidden_Candidates)
* [Hidden Tripples](https://www.sudokuwiki.org/Hidden_Candidates)
* [Pointing Pairs/Tripples](https://www.sudokuwiki.org/Intersection_Removal)
* [Box Line Reduction](https://www.sudokuwiki.org/Intersection_Removal)
* [X-Wings](https://www.sudokuwiki.org/X_Wing_Strategy)

Many of the boards cant be solved by logical deduction (at least with the current strategies) but paired with any of the backtrack solvers makes a powerful solver.

# Performance
<!-- This section is auto generated. -->

Benchmark is run on 1971 different Sudoku boards with a 2s time limit.
The results are ordered by solved instances, then by lowest average search time and finally by average calls.

| Solver | **Solved** | **Avg Search (ms)** | **Avg Calls** | Max Search (ms) | Min Search (ms) | Max Calls | Min Calls |
| - | - | - | - | - | - | - | - |
| LogicalWithCardinalityBacktrack | 1971 | 2.99 | 28494.86 | 119.4 | 0.18 | 1156304 | 2 |
| LogicalWithSequentialBacktrack | 1971 | 3.95 | 43804.85 | 118.83 | 0.18 | 1903421 | 2 |
| CardinalityBacktrack | 1971 | 11.6 | 203805.84 | 1889.73 | 0.02 | 32884160 | 42 |
| SequentialBacktrack | 1967 | 22.48 | 421545.84 | 1834.6 | 0.02 | 34058393 | 43 |
| LogicalWithRandomBacktrack | 1372 | 60.58 | 15919082.6 | 2006.83 | 0.11 | 59626445 | 2 |
| Logical | 1024 | 1.56 | 3.12 | 13.43 | 0.34 | 19 | 1 |
| RandomBacktrack | 390 | 460.34 | 41259946.14 | 2001.87 | 0.02 | 62526698 | 73 |

<!-- This section is auto generated. -->
# Credits
I would like to give a special thanks to Andrew Stuart, the owner of the [SudokuWiki.org](https://www.sudokuwiki.org/) website, for giving a bunch of great description of the different strategies that are involved in solving sudokus!
































