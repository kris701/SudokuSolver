using SudokuToolsSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuToolsSharp.Solvers.BacktrackSolvers
{
    public class Preprocessor
    {
        public SearchOptions Options { get; set; }
        public List<Position> Cardinalities { get; internal set; }
        public List<PossibleAssignment>[,] Candidates { get; internal set; }
        public bool Failed { get; internal set; } = false;

        private int _size = 0;

        public Preprocessor(SearchOptions options, int boardSize)
        {
            Options = options;
            Cardinalities = new List<Position>();
            Candidates = new List<PossibleAssignment>[boardSize, boardSize];
            _size = boardSize;
        }

        public SudokuBoard Preprocess(SudokuBoard board)
        {
            Candidates = Ground(board, Options.GroundLegalCandidatesOnly);

            // Prune certains (and add them to the board)
            if (Options.PruneCertains)
            {
                var pruned = 0;
                bool any = true;
                while (any)
                {
                    any = false;
                    for (int cellX = 0; cellX < board.Cells; cellX++)
                    {
                        for (int cellY = 0; cellY < board.Cells; cellY++)
                        {
                            var fromX = cellX * board.Cells;
                            var toX = (cellX + 1) * board.Cells;
                            var fromY = cellY * board.Cells;
                            var toY = (cellY + 1) * board.Cells;
                            var cellPossibilities = new List<PossibleAssignment>();
                            for (int x = fromX; x < toX; x++)
                                for (int y = fromY; y < toY; y++)
                                    cellPossibilities.AddRange(Candidates[x, y]);

                            for (int i = 1; i < board.CellSize * board.CellSize; i++)
                            {
                                if (cellPossibilities.Count(x => x.Value == i) == 1)
                                {
                                    var first = cellPossibilities.First(x => x.Value == i);
                                    first.Apply(board);
                                    Candidates[first.X, first.Y].Clear();
                                    any = true;
                                    pruned++;
                                }
                            }
                        }
                    }
                    Candidates = Ground(board, Options.GroundLegalCandidatesOnly);
                }
                if (Options.EnableLog)
                    Console.WriteLine($"Removed {pruned} certains");
            }

            // Prune hidden pairs
            if (Options.PruneHiddenPairs)
            {
                var pruned = 0;
                bool any = true;
                while (any)
                {
                    any = false;
                    for (int cellX = 0; cellX < board.Cells; cellX++)
                    {
                        for (int cellY = 0; cellY < board.Cells; cellY++)
                        {
                            var fromX = cellX * board.Cells;
                            var toX = (cellX + 1) * board.Cells;
                            var fromY = cellY * board.Cells;
                            var toY = (cellY + 1) * board.Cells;
                            var cellPossibilities = new List<PossibleAssignment>();
                            for (int x = fromX; x < toX; x++)
                                for (int y = fromY; y < toY; y++)
                                    cellPossibilities.AddRange(Candidates[x, y]);

                            for (int i = 1; i < board.CellSize * board.CellSize; i++)
                            {
                                if (cellPossibilities.Count(x => x.Value == i) == 2)
                                {
                                    for (int j = 1; j < board.CellSize * board.CellSize; j++)
                                    {
                                        if (i == j)
                                            continue;
                                        if (cellPossibilities.Count(x => x.Value == j) == 2)
                                        {
                                            if (cellPossibilities.Where(x => x.Value == i).All(
                                                x => cellPossibilities.Where(x => x.Value == j).Any(
                                                    y => y.X == x.X && y.Y == x.Y)))
                                            {
                                                foreach(var possibility in cellPossibilities.Where(x => x.Value == i))
                                                {
                                                    var preCount = Candidates[possibility.X, possibility.Y].Count;
                                                    Candidates[possibility.X, possibility.Y].RemoveAll(x => x.Value != i && x.Value != j);
                                                    var change = preCount - Candidates[possibility.X, possibility.Y].Count;
                                                    pruned += change;
                                                    if (change > 0)
                                                        any = true;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (Options.EnableLog)
                    Console.WriteLine($"Removed {pruned} candidates because of hidden pairs");
            }

            // Reduces hidden pairs
            //if (Options.ReduceHiddenPairs)
            //{
            //    bool any = true;
            //    while (any)
            //    {
            //        any = false;
            //        for (int cellX = 0; cellX < from.Cells; cellX++)
            //        {
            //            for (int cellY = 0; cellY < from.Cells; cellY++)
            //            {
            //                for (int i = 1; i < from.CellSize * from.CellSize; i++)
            //                {
            //                    if (_possibles[cellX, cellY][i].Count == 2)
            //                    {
            //                        for (int j = 1; j < from.CellSize * from.CellSize; j++)
            //                        {
            //                            if (i == j)
            //                                continue;
            //                            if (_possibles[cellX, cellY][j].Count == 2)
            //                            {
            //                                var covered = true;
            //                                foreach (var possible in _possibles[cellX, cellY][i])
            //                                    if (!_possibles[cellX, cellY][j].Any(x => x.X == possible.X && x.Y == possible.Y))
            //                                        covered = false;
            //                                if (covered)
            //                                {
            //                                    for(int l = 1; l < from.CellSize * from.CellSize; l++)
            //                                    {
            //                                        if (l == i || l == j)
            //                                            continue;
            //                                        var preCount = _possibles[cellX, cellY][l].Count;
            //                                        _possibles[cellX, cellY][l].RemoveAll(x => _possibles[cellX, cellY][i].Any(y => x.X == y.X && x.Y == y.Y));
            //                                        if (preCount != _possibles[cellX, cellY][l].Count)
            //                                            any = true;
            //                                    }
            //                                }
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}

            int total = 0;
            for (int x = 0; x < board.CellSize * board.CellSize; x++)
            {
                for (int y = 0; y < board.CellSize * board.CellSize; y++)
                {
                    if (board[x, y] != board.BlankNumber)
                        continue;
                    total += Candidates[x, y].Count;
                    Cardinalities.Add(new Position(x, y, Candidates[x, y].Count));
                }
            }
            Cardinalities = Cardinalities.OrderBy(x => x.Possibilities).ToList();

            if (Options.EnableLog)
                Console.WriteLine($"Total Possibilities: {total}");

            return board;
        }

        private List<PossibleAssignment>[,] Ground(SudokuBoard board, bool legalOnly)
        {
            var actions = new List<PossibleAssignment>[_size, _size];

            for (int x = 0; x < _size; x++)
            {
                var column = board.GetColumn(x);
                for (int y = 0; y < _size; y++)
                {
                    actions[x, y] = new List<PossibleAssignment>();
                    if (board[x, y] != board.BlankNumber)
                        continue;

                    var row = board.GetRow(y);

                    var cellX = board.CellX(x);
                    var cellY = board.CellY(y);
                    var cellValues = board.GetCellValues(cellX, cellY);
                    for (int i = 1; i <= _size; i++)
                    {
                        if (cellValues.Contains(i))
                            continue;
                        if (!legalOnly || (!row.Contains(i) && !column.Contains(i)))
                            actions[x, y].Add(new PossibleAssignment(x, y, i));
                    }
                }
            }

            return actions;
        }
    }
}
