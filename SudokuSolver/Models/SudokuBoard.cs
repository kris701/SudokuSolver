using SudokuToolsSharp.Helpers;
using System.Data.Common;
using System.Drawing;
using System.Text;

namespace SudokuToolsSharp.Models
{
    public class SudokuBoard
    {
        public int CellSize { get; }
        private unsafe int[] _values;
        public int this[int x, int y]
        {
            get => this._values[x + y * _size];
            set => this._values[x + y * _size] = value;
        }
        public int BlankNumber { get; set; } = 0;
        public int Cells { get; private set; }

        private readonly int _size;

        public SudokuBoard(int[] values, int cellSize)
        {
            CellSize = cellSize;
            _size = values.Length / (cellSize * cellSize);
            _values = values;
            Cells = _size / cellSize;
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
            for(int i = 0; i < data.Length; i++)
            {
                if (set.Contains(data[i]))
                    return false;
                set.Add(data[i]);
            }

            return true;
        }

        public int[] GetRow(int row) => _values.GetRow(row, _size);
        public int[] GetColumn(int column) => _values.GetColumn(column, _size);

        public bool RowContains(int row, int value)
        {
            for (int x = 0; x < _size; x++)
                if (_values[row * _size + x] == value)
                    return true;
            return false;
        }
        public bool ColumnContains(int column, int value)
        {
            for (int y = 0; y < _size; y++)
                if (_values[y * _size + column] == value)
                    return true;
            return false;
        }

        public int CellX(int x) => (int)Math.Floor((double)x / Cells);
        public int CellY(int y) => (int)Math.Floor((double)y / Cells);

        public bool CellContains(int cellX, int cellY, int value)
        {
            var fromX = cellX * Cells;
            var toX = (cellX + 1) * Cells;
            var fromY = cellY * Cells;
            var toY = (cellY + 1) * Cells;

            for (int x = fromX; x < toX; x++)
                for (int y = fromY; y < toY; y++)
                    if (_values[y * _size + x] == value)
                        return true;
            return false;
        }

        public HashSet<int> GetCellValues(int cellX, int cellY)
        {
            var returnList = new HashSet<int>();
            var fromX = cellX * Cells;
            var toX = (cellX + 1) * Cells;
            var fromY = cellY * Cells;
            var toY = (cellY + 1) * Cells;

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
            return new SudokuBoard(cpyCells, CellSize)
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
                    if ((x + 1) % CellSize == 0)
                        sb.Append(" ");
                }
                sb.AppendLine();
                if ((y + 1) % CellSize == 0)
                    sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
