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
            Cardinalities = GenerateCardinalities(board);
            Console.WriteLine($"Initial Possibilities: {Cardinalities.Sum(x => x.Possibilities)}");

            bool any = true;
            while (any)
            {
                any = false;
                // Prune certains (and add them to the board)
                if (Options.PruneCertains)
                    while (PruneCertains(board)) { any = true; }

                // Prune naked pairs
                if (Options.PruneNakedPairs)
                    while (PruneNakedPairs(board)) { any = true; }

                // Prune hidden pairs
                if (Options.PruneHiddenPairs)
                    while (PruneHiddenPairs(board)) { any = true; }
            }

            Cardinalities = GenerateCardinalities(board);

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

            Console.WriteLine($"Final Possibilities: {Cardinalities.Sum(x => x.Possibilities)}");

            return board;
        }

        private List<CellPosition> GenerateCardinalities(SudokuBoard board)
        {
            var cardinalities = new List<CellPosition>();
            for (byte x = 0; x < BoardSize; x++)
            {
                for (byte y = 0; y < BoardSize; y++)
                {
                    if (board[x, y] != board.BlankNumber)
                        continue;
                    cardinalities.Add(new CellPosition(x, y, Candidates[x, y].Count));
                }
            }
            if (cardinalities.Any(x => x.Possibilities == 0))
                throw new Exception("Invalid preprocessing");
            cardinalities = cardinalities.OrderBy(x => x.Possibilities).ToList();
            return cardinalities;
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

                    var blockX = board.BlockX(ref x);
                    var blockY = board.BlockY(ref y);
                    var cellValues = board.GetBlockValues(ref blockX, ref blockY);
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

        private bool PruneCertains(SudokuBoard board)
        {
            var pruned = 0;
            for (byte x = 0; x < board.BoardSize; x++)
                for (byte y = 0; y < board.BoardSize; y++)
                    if (Candidates[x, y].Count == 1)
                        pruned += RemoveCandidate(board, Candidates[x, y][0]);

            for (byte blockX = 0; blockX < board.Blocks; blockX++)
            {
                for (byte blockY = 0; blockY < board.Blocks; blockY++)
                {
                    var fromX = blockX * board.Blocks;
                    var toX = (blockX + 1) * board.Blocks;
                    var fromY = blockY * board.Blocks;
                    var toY = (blockY + 1) * board.Blocks;
                    var cellPossibilities = new List<CellAssignment>();
                    for (byte x = (byte)fromX; x < toX; x++)
                        for (byte y = (byte)fromY; y < toY; y++)
                            cellPossibilities.AddRange(Candidates[x, y]);

                    for (byte i = 1; i < board.BlockSize * board.BlockSize; i++)
                        if (cellPossibilities.Count(x => x.Value == i) == 1)
                            pruned += RemoveCandidate(board, cellPossibilities.First(x => x.Value == i));
                }
            }
            if (pruned > 0)
                Console.WriteLine($"Removed {pruned} certains");
            return pruned > 0;
        }

        private int RemoveCandidate(SudokuBoard board, CellAssignment cell)
        {
            var pruned = 0;
            cell.Apply(board);
            Candidates[cell.X, cell.Y].Clear();
            pruned++;

            for (byte x2 = 0; x2 < board.BoardSize; x2++)
                pruned += Candidates[x2, cell.Y].RemoveAll(z => z.Value == cell.Value);
            for (byte y2 = 0; y2 < board.BoardSize; y2++)
                pruned += Candidates[cell.X, y2].RemoveAll(z => z.Value == cell.Value);

            var blockX = board.BlockX(ref cell.X);
            var blockY = board.BlockY(ref cell.Y);
            var fromX = blockX * board.Blocks;
            var toX = (blockX + 1) * board.Blocks;
            var fromY = blockY * board.Blocks;
            var toY = (blockY + 1) * board.Blocks;

            for (byte x = (byte)fromX; x < toX; x++)
                for (byte y = (byte)fromY; y < toY; y++)
                    pruned += Candidates[x, y].RemoveAll(z => z.Value == cell.Value);
            return pruned;
        }

        private bool PruneHiddenPairs(SudokuBoard board)
        {
            var pruned = 0;
            bool any = false;
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
                                            var removed = Candidates[possibility.X, possibility.Y].RemoveAll(x => x.Value != i && x.Value != j);
                                            pruned += removed;
                                            if (removed > 0)
                                                any = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (pruned > 0)
                Console.WriteLine($"Removed {pruned} candidates because of hidden pairs");
            return any;
        }

        private bool PruneNakedPairs(SudokuBoard board)
        {
            var pruned = 0;
            bool any = false;
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
                        var protectedColumn = new List<int>();
                        for (int column = 0; column < BoardSize; column++)
                        {
                            if (cellPossibilities[column].Count > 0)
                            {
                                protectedColumn.Add(column);
                                foreach (var value in cellPossibilities[column])
                                    removeValues.Add(value.Value);
                            }
                        }
                        if (removeValues.Count > 0)
                        {
                            var removed = 0;
                            foreach (var value in removeValues)
                                for (int column = 0; column < BoardSize; column++)
                                    if (!protectedColumn.Contains(column))
                                        removed += Candidates[column, row].RemoveAll(x => x.Value == value);
                            pruned += removed;
                            if (removed > 0)
                            {
                                any = true;
                            }
                        }
                    }
                }
            }
            if (pruned > 0)
                Console.WriteLine($"Removed {pruned} candidates because of naked pairs");
            return any;
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
