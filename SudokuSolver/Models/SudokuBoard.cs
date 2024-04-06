using SudokuSolver.Helpers;
using System.Collections;
using System.Text;

namespace SudokuSolver.Models
{
    public class SudokuBoard
    {
        public int BlockSize { get; }
        private readonly unsafe int[] _values;
        private readonly unsafe bool[] _blocks;
        private readonly unsafe bool[] _rows;
        private readonly unsafe bool[] _columns;
        public int this[int x, int y]
        {
            get => _values[x + y * _size];
            set {
                _values[x + y * _size] = value;
                _blocks[BlockX(x) * (_size + 1) + BlockY(y) * (_size + 1) * Blocks + value] = true;
                _rows[y * (_size + 1) + value] = true;
                _columns[x * (_size + 1) + value] = true;
            }
        }
        public int BlankNumber { get; set; } = 0;
        public int Blocks { get; private set; }

        private readonly int _size;

        public SudokuBoard(int[] values, int blockSize)
        {
            BlockSize = blockSize;
            _size = values.Length / (blockSize * blockSize);
            _values = new int[_size * _size];
            Blocks = _size / blockSize;

            _blocks = new bool[Blocks * Blocks * (_size + 1)];
            _rows = new bool[(_size + 1) * (_size + 1)];
            _columns = new bool[(_size + 1) * (_size + 1)];

            for (int x = 0; x < _size; x++)
                for (int y = 0; y < _size; y++)
                    this[x, y] = values[x + y * _size];
        }

        internal SudokuBoard(int[] values, int blockSize, int size, int blockCount, bool[] blocks, bool[] rows, bool[] columns)
        {
            _values = values;
            BlockSize = blockSize;
            _size = size;
            Blocks = blockCount;
            _blocks = blocks;
            _rows = rows;
            _columns = columns;
        }

        public bool IsComplete()
        {
            for (int i = 0; i < _size * _size; i++)
                if (_values[i] == BlankNumber)
                    return false;
            return IsLegal();
        }

        public bool IsLegal()
        {
            for (int x = 0; x < _size; x++)
                if (!UniqueOnly(GetColumn(x)))
                    return false;
            for (int y = 0; y < _size; y++)
                if (!UniqueOnly(GetRow(y)))
                    return false;

            return true;
        }

        private bool UniqueOnly(int[] data)
        {
            var set = new HashSet<int>();
            for (int i = 0; i < data.Length; i++)
            {
                if (set.Contains(data[i]))
                    return false;
                set.Add(data[i]);
            }

            return true;
        }

        public int[] GetRow(int row) => _values.GetRow(row, _size);
        public int[] GetColumn(int column) => _values.GetColumn(column, _size);

        public bool RowContains(int row, int value) => _rows[row * (_size + 1) + value];
        public bool ColumnContains(int column, int value) => _columns[column * (_size + 1) + value];

        public int BlockX(int x) => (int)Math.Floor((double)x / Blocks);
        public int BlockY(int y) => (int)Math.Floor((double)y / Blocks);

        public bool BlockContains(int cellX, int cellY, int value) => _blocks[cellX * (_size + 1) + cellY * (_size + 1) * Blocks + value];

        public HashSet<int> GetBlockValues(int cellX, int cellY)
        {
            var returnList = new HashSet<int>();
            var fromX = cellX * Blocks;
            var toX = (cellX + 1) * Blocks;
            var fromY = cellY * Blocks;
            var toY = (cellY + 1) * Blocks;

            for (int x = fromX; x < toX; x++)
                for (int y = fromY; y < toY; y++)
                    if (_values[y * _size + x] != BlankNumber)
                        returnList.Add(_values[y * _size + x]);
            return returnList;
        }

        public SudokuBoard Copy()
        {
            var cpyCells = new int[_size * _size];
            Buffer.BlockCopy(_values, 0, cpyCells, 0, _size * _size * sizeof(int));
            var cpyBlocks = new bool[Blocks * Blocks * (_size + 1)];
            Buffer.BlockCopy(_blocks, 0, cpyBlocks, 0, Blocks * Blocks * (_size + 1) * sizeof(bool));
            var cpyRows = new bool[(_size + 1) * (_size + 1)];
            Buffer.BlockCopy(_rows, 0, cpyRows, 0, (_size + 1) * (_size + 1) * sizeof(bool));
            var cpyColumns = new bool[(_size + 1) * (_size + 1)];
            Buffer.BlockCopy(_columns, 0, cpyColumns, 0, (_size + 1) * (_size + 1) * sizeof(bool));
            return new SudokuBoard(cpyCells, BlockSize, _size, Blocks, cpyBlocks, cpyRows, cpyColumns)
            {
                BlankNumber = BlankNumber
            };
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            for (int y = 0; y < _size; y++)
            {
                for (int x = 0; x < _size; x++)
                {
                    if (_values[y * _size + x] == BlankNumber)
                        sb.Append("_");
                    else
                        sb.Append($"{_values[y * _size + x]}");
                    if ((x + 1) % BlockSize == 0)
                        sb.Append(" ");
                }
                if (y != _size - 1)
                {
                    sb.AppendLine();
                    if ((y + 1) % BlockSize == 0)
                        sb.AppendLine();
                }
            }

            return sb.ToString();
        }
    }
}
