using SudokuSolver.Helpers;
using System.Text;

namespace SudokuSolver.Models
{
    public class SudokuBoard
    {
        public byte BlockSize;
        public byte BlankNumber = 0;
        public byte Blocks;
        public byte BoardSize;
        public byte this[byte x, byte y]
        {
            get => _values[x + y * BoardSize];
            set
            {
                var current = _values[x + y * BoardSize];
                _blocks[_blockRefs[x + y * BoardSize] * (BoardSize + 1) + current] = false;
                _rows[y * (BoardSize + 1) + current] = false;
                _columns[x * (BoardSize + 1) + current] = false;

                _values[x + y * BoardSize] = value;
                _blocks[_blockRefs[x + y * BoardSize] * (BoardSize + 1) + value] = true;
                _rows[y * (BoardSize + 1) + value] = true;
                _columns[x * (BoardSize + 1) + value] = true;
            }
        }

        private readonly unsafe byte[] _values;
        private readonly unsafe bool[] _blocks;
        private readonly unsafe bool[] _rows;
        private readonly unsafe bool[] _columns;
        private readonly unsafe byte[] _blockRefs;

        public SudokuBoard(byte[] values, byte blockSize)
        {
            BlockSize = blockSize;
            BoardSize = (byte)(values.Length / (blockSize * blockSize));
            _values = new byte[BoardSize * BoardSize];
            _blockRefs = new byte[BoardSize * BoardSize];
            Blocks = (byte)(BoardSize / blockSize);

            _blocks = new bool[Blocks * Blocks * (BoardSize + 1)];
            _rows = new bool[(BoardSize + 1) * (BoardSize + 1)];
            _columns = new bool[(BoardSize + 1) * (BoardSize + 1)];

            for (byte x = 0; x < BoardSize; x++)
            {
                for (byte y = 0; y < BoardSize; y++)
                {
                    _blockRefs[x + y * BoardSize] = (byte)((x / Blocks) + (y / Blocks) * Blocks);
                    this[x, y] = values[x + y * BoardSize];
                }
            }
        }

        internal SudokuBoard(byte[] values, byte blockSize, byte size, byte blockCount, bool[] blocks, bool[] rows, bool[] columns, byte[] blockRefs)
        {
            _values = values;
            BlockSize = blockSize;
            BoardSize = size;
            Blocks = blockCount;
            _blocks = blocks;
            _rows = rows;
            _columns = columns;
            _blockRefs = blockRefs;
        }

        public bool IsComplete()
        {
            for (int i = 0; i < BoardSize * BoardSize; i++)
                if (_values[i] == BlankNumber)
                    return false;
            return IsLegal();
        }

        public bool IsLegal()
        {
            for (byte x = 0; x < BoardSize; x++)
                if (!GetColumn(ref x).IsUnique())
                    return false;
            for (byte y = 0; y < BoardSize; y++)
                if (!GetRow(ref y).IsUnique())
                    return false;

            return true;
        }

        public byte[] GetRow(ref byte row) => _values.GetRow(row, BoardSize);
        public byte[] GetColumn(ref byte column) => _values.GetColumn(column, BoardSize);

        public bool RowContains(ref byte row, ref byte value) => _rows[row * (BoardSize + 1) + value];
        public bool ColumnContains(ref byte column, ref byte value) => _columns[column * (BoardSize + 1) + value];

        public int BlockX(ref byte x) => (int)Math.Floor((double)x / Blocks);
        public int BlockY(ref byte y) => (int)Math.Floor((double)y / Blocks);

        public bool BlockContains(ref byte x, ref byte y, ref byte value) => _blocks[_blockRefs[x + y * BoardSize] * (BoardSize + 1) + value];

        public HashSet<int> GetBlockValues(ref int cellX, ref int cellY)
        {
            var returnList = new HashSet<int>();
            var fromX = cellX * Blocks;
            var toX = (cellX + 1) * Blocks;
            var fromY = cellY * Blocks;
            var toY = (cellY + 1) * Blocks;

            for (int x = fromX; x < toX; x++)
                for (int y = fromY; y < toY; y++)
                    if (_values[y * BoardSize + x] != BlankNumber)
                        returnList.Add(_values[y * BoardSize + x]);
            return returnList;
        }

        public unsafe SudokuBoard Copy()
        {
            var cpyCells = new byte[BoardSize * BoardSize];
            Buffer.BlockCopy(_values, 0, cpyCells, 0, BoardSize * BoardSize * sizeof(byte));
            var cpyBlocks = new bool[Blocks * Blocks * (BoardSize + 1)];
            Buffer.BlockCopy(_blocks, 0, cpyBlocks, 0, Blocks * Blocks * (BoardSize + 1) * sizeof(bool));
            var cpyRows = new bool[(BoardSize + 1) * (BoardSize + 1)];
            Buffer.BlockCopy(_rows, 0, cpyRows, 0, (BoardSize + 1) * (BoardSize + 1) * sizeof(bool));
            var cpyColumns = new bool[(BoardSize + 1) * (BoardSize + 1)];
            Buffer.BlockCopy(_columns, 0, cpyColumns, 0, (BoardSize + 1) * (BoardSize + 1) * sizeof(bool));
            return new SudokuBoard(cpyCells, BlockSize, BoardSize, Blocks, cpyBlocks, cpyRows, cpyColumns, _blockRefs)
            {
                BlankNumber = BlankNumber
            };
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            for (int y = 0; y < BoardSize; y++)
            {
                for (int x = 0; x < BoardSize; x++)
                {
                    if (_values[y * BoardSize + x] == BlankNumber)
                        sb.Append("_");
                    else
                        sb.Append($"{_values[y * BoardSize + x]}");
                    if ((x + 1) % BlockSize == 0)
                        sb.Append(" ");
                }
                if (y != BoardSize - 1)
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
