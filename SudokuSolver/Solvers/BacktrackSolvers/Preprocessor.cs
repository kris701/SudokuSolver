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
                PruneCertains(board);

            // Prune hidden pairs
            if (Options.PruneHiddenPairs)
                PruneHiddenPairs(board);

            // Prune naked pairs
            if (Options.PruneNakedPairs)
                PruneNakedPairs(board);

            int total = 0;
            for (int x = 0; x < _size; x++)
            {
                for (int y = 0; y < _size; y++)
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

        private void PruneCertains(SudokuBoard board)
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

        private void PruneHiddenPairs(SudokuBoard board)
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
                                            foreach (var possibility in cellPossibilities.Where(x => x.Value == i))
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

        private void PruneNakedPairs(SudokuBoard board)
        {
            var pruned = 0;
            bool any = true;
            while (any)
            {
                any = false;
                for (int column = 0; column < _size; column++)
                {
                    var cellPossibilities = new List<List<PossibleAssignment>>();
                    for (int row = 0; row < _size; row++)
                        cellPossibilities.Add(GetBinaryAssignments(column, row));

                    if (cellPossibilities.Count(x => x.Count == 2) > 0)
                    {
                        cellPossibilities = RemoveUnpaired(cellPossibilities);

                        if (cellPossibilities.Any(x => x.Count > 0))
                        {
                            var removeValues = new HashSet<int>();
                            var protectedRows = new List<int>();
                            for (int row = 0; row < _size; row++)
                            {
                                if (cellPossibilities[row].Count > 0)
                                {
                                    protectedRows.Add(row);
                                    foreach (var value in cellPossibilities[row])
                                        removeValues.Add(value.Value);
                                }
                            }
                            if (removeValues.Count > 0)
                            {
                                var removed = 0;
                                foreach (var value in removeValues)
                                    for (int row = 0; row < _size; row++)
                                        if (!protectedRows.Contains(row))
                                            removed += Candidates[column, row].RemoveAll(x => x.Value == value);
                                pruned += removed;
                                if (removed > 0)
                                    any = true;
                            }
                        }
                    }
                }

                for (int row = 0; row < _size; row++)
                {
                    var cellPossibilities = new List<List<PossibleAssignment>>();
                    for (int column = 0; column < _size; column++)
                        cellPossibilities.Add(GetBinaryAssignments(column, row));

                    if (cellPossibilities.Count(x => x.Count == 2) > 0)
                    {
                        cellPossibilities = RemoveUnpaired(cellPossibilities);

                        if (cellPossibilities.Any(x => x.Count > 0))
                        {
                            var removeValues = new HashSet<int>();
                            var protectedRows = new List<int>();
                            for (int column = 0; column < _size; column++)
                            {
                                if (cellPossibilities[column].Count > 0)
                                {
                                    protectedRows.Add(column);
                                    foreach (var value in cellPossibilities[column])
                                        removeValues.Add(value.Value);
                                }
                            }
                            if (removeValues.Count > 0)
                            {
                                var removed = 0;
                                foreach (var value in removeValues)
                                    for (int column = 0; column < _size; column++)
                                        if (!protectedRows.Contains(column))
                                            removed += Candidates[column, row].RemoveAll(x => x.Value == value);
                                pruned += removed;
                                if (removed > 0)
                                    any = true;
                            }
                        }
                    }
                }
            }
            if (Options.EnableLog)
                Console.WriteLine($"Removed {pruned} candidates because of naked pairs");
        }

        private List<PossibleAssignment> GetBinaryAssignments(int column, int row)
        {
            var result = new List<PossibleAssignment>();
            if (Candidates[column, row].Count == 2)
                result.AddRange(Candidates[column, row]);
            return result;
        }

        private List<List<PossibleAssignment>> RemoveUnpaired(List<List<PossibleAssignment>> cellPossibilities)
        {
            for (int i = 0; i < _size; i++)
            {
                bool remove = true;
                if (cellPossibilities[i].Count > 0)
                {
                    for (int j = 0; j < _size; j++)
                    {
                        if (i == j)
                            continue;
                        if (cellPossibilities[j].Count > 0)
                        {
                            if (cellPossibilities[j].All(x => cellPossibilities[i].Any(y => y.Value == x.Value)))
                            {
                                remove = false;
                                break;
                            }
                        }
                    }
                }
                if (remove)
                    cellPossibilities[i].Clear();
            }
            return cellPossibilities;
        }
    }
}
