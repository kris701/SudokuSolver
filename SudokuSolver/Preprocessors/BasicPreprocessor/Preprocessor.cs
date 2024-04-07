using SudokuSolver.Models;

namespace SudokuSolver.Preprocessors.BasicPreprocessor
{
    public class Preprocessor : IPreprocessor
    {
        public byte BoardSize { get; }
        public List<CellPosition> Cardinalities { get; internal set; }
        public List<CellAssignment>[,] Candidates { get; internal set; }
        public Dictionary<int, int>[,] CandidatesPrValue { get; internal set; }
        public BasicPreprocessorOptions Options { get; set; }

        public Preprocessor(BasicPreprocessorOptions options, byte boardSize)
        {
            Options = options;
            Cardinalities = new List<CellPosition>();
            Candidates = new List<CellAssignment>[boardSize, boardSize];
            CandidatesPrValue = new Dictionary<int, int>[boardSize, boardSize];
            BoardSize = boardSize;
        }

        public SudokuBoard Preprocess(SudokuBoard board)
        {
            Cardinalities.Clear();
            Candidates = Ground(board, Options.GroundLegalCandidatesOnly);
            for (byte x = 0; x < BoardSize; x++)
            {
                for (byte y = 0; y < BoardSize; y++)
                {
                    CandidatesPrValue[x, y] = new Dictionary<int, int>();
                    for (int i = 1; i <= board.BlockSize * board.BlockSize; i++)
                        CandidatesPrValue[x, y].Add(i, 0);

                    foreach (var option in Candidates[x, y])
                        CandidatesPrValue[x, y][option.Value]++;
                }
            }

            // Prune certains (and add them to the board)
            if (Options.PruneCertains)
                PruneCertains(board);

            // Prune hidden pairs
            if (Options.PruneHiddenPairs)
                PruneHiddenPairs(board);

            // Prune naked pairs
            if (Options.PruneNakedPairs)
                PruneNakedPairs(board);

            // Count and sort cardinalities
            int total = 0;
            for (byte x = 0; x < BoardSize; x++)
            {
                for (byte y = 0; y < BoardSize; y++)
                {
                    if (board[x, y] != board.BlankNumber)
                        continue;
                    total += Candidates[x, y].Count;
                    Cardinalities.Add(new CellPosition(x, y, Candidates[x, y].Count));
                }
            }
            Cardinalities = Cardinalities.OrderBy(x => x.Possibilities).ToList();

            Console.WriteLine($"Total Possibilities: {total}");

            return board;
        }

        private List<CellAssignment>[,] Ground(SudokuBoard board, bool legalOnly)
        {
            var actions = new List<CellAssignment>[BoardSize, BoardSize];

            for (byte x = 0; x < BoardSize; x++)
            {
                var column = board.GetColumn(ref x);
                for (byte y = 0; y < BoardSize; y++)
                {
                    actions[x, y] = new List<CellAssignment>();
                    if (board[x, y] != board.BlankNumber)
                        continue;

                    var row = board.GetRow(ref y);

                    var cellX = board.BlockX(ref x);
                    var cellY = board.BlockY(ref y);
                    var cellValues = board.GetBlockValues(ref cellX, ref cellY);
                    for (byte i = 1; i <= BoardSize; i++)
                    {
                        if (cellValues.Contains(i))
                            continue;
                        if (!legalOnly || !row.Contains(i) && !column.Contains(i))
                            actions[x, y].Add(new CellAssignment(x, y, i));
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
                for (int cellX = 0; cellX < board.Blocks; cellX++)
                {
                    for (int cellY = 0; cellY < board.Blocks; cellY++)
                    {
                        var fromX = cellX * board.Blocks;
                        var toX = (cellX + 1) * board.Blocks;
                        var fromY = cellY * board.Blocks;
                        var toY = (cellY + 1) * board.Blocks;
                        var cellPossibilities = new List<CellAssignment>();
                        for (int x = fromX; x < toX; x++)
                            for (int y = fromY; y < toY; y++)
                                cellPossibilities.AddRange(Candidates[x, y]);

                        for (int i = 1; i < board.BlockSize * board.BlockSize; i++)
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
            Console.WriteLine($"Removed {pruned} certains");
        }

        private void PruneHiddenPairs(SudokuBoard board)
        {
            var pruned = 0;
            bool any = true;
            while (any)
            {
                any = false;
                for (int cellX = 0; cellX < board.Blocks; cellX++)
                {
                    for (int cellY = 0; cellY < board.Blocks; cellY++)
                    {
                        var fromX = cellX * board.Blocks;
                        var toX = (cellX + 1) * board.Blocks;
                        var fromY = cellY * board.Blocks;
                        var toY = (cellY + 1) * board.Blocks;
                        var cellPossibilities = new List<CellAssignment>();
                        for (int x = fromX; x < toX; x++)
                            for (int y = fromY; y < toY; y++)
                                cellPossibilities.AddRange(Candidates[x, y]);

                        for (int i = 1; i < board.BlockSize * board.BlockSize; i++)
                        {
                            if (cellPossibilities.Count(x => x.Value == i) == 2)
                            {
                                for (int j = 1; j < board.BlockSize * board.BlockSize; j++)
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
            Console.WriteLine($"Removed {pruned} candidates because of hidden pairs");
        }

        private void PruneNakedPairs(SudokuBoard board)
        {
            var pruned = 0;
            bool any = true;
            while (any)
            {
                any = false;
                for (int column = 0; column < BoardSize; column++)
                {
                    var cellPossibilities = new List<List<CellAssignment>>();
                    for (int row = 0; row < BoardSize; row++)
                        cellPossibilities.Add(GetBinaryAssignments(column, row));

                    if (cellPossibilities.Count(x => x.Count == 2) > 0)
                    {
                        cellPossibilities = RemoveUnpaired(cellPossibilities);

                        if (cellPossibilities.Any(x => x.Count > 0))
                        {
                            var removeValues = new HashSet<int>();
                            var protectedRows = new List<int>();
                            for (int row = 0; row < BoardSize; row++)
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
                                    for (int row = 0; row < BoardSize; row++)
                                        if (!protectedRows.Contains(row))
                                            removed += Candidates[column, row].RemoveAll(x => x.Value == value);
                                pruned += removed;
                                if (removed > 0)
                                    any = true;
                            }
                        }
                    }
                }

                for (int row = 0; row < BoardSize; row++)
                {
                    var cellPossibilities = new List<List<CellAssignment>>();
                    for (int column = 0; column < BoardSize; column++)
                        cellPossibilities.Add(GetBinaryAssignments(column, row));

                    if (cellPossibilities.Count(x => x.Count == 2) > 0)
                    {
                        cellPossibilities = RemoveUnpaired(cellPossibilities);

                        if (cellPossibilities.Any(x => x.Count > 0))
                        {
                            var removeValues = new HashSet<int>();
                            var protectedRows = new List<int>();
                            for (int column = 0; column < BoardSize; column++)
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
                                    for (int column = 0; column < BoardSize; column++)
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
            Console.WriteLine($"Removed {pruned} candidates because of naked pairs");
        }

        private List<CellAssignment> GetBinaryAssignments(int column, int row)
        {
            var result = new List<CellAssignment>();
            if (Candidates[column, row].Count == 2)
                result.AddRange(Candidates[column, row]);
            return result;
        }

        private List<List<CellAssignment>> RemoveUnpaired(List<List<CellAssignment>> cellPossibilities)
        {
            for (int i = 0; i < BoardSize; i++)
            {
                bool remove = true;
                if (cellPossibilities[i].Count > 0)
                {
                    for (int j = 0; j < BoardSize; j++)
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
