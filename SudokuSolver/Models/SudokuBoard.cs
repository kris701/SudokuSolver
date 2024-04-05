using SudokuToolsSharp.Helpers;
using System.Data.Common;
using System.Drawing;
using System.Text;

namespace SudokuToolsSharp.Models
{
    public class SudokuBoard
    {
        public int CellSize { get; }
        public int[,] Values { get; }
        public int BlankNumber { get; set; } = 0;
        public int Cells { get; private set; }

        private readonly int _size;

        public SudokuBoard(int[] values, int cellSize)
        {
            CellSize = cellSize;
            _size = values.Length / (cellSize * cellSize);
            Values = new int[_size, _size];
            int y = 0;
            for (int i = 0; i < values.Length; i++)
            {
                var x = i % _size;
                if (x == 0 && i>0)
                    y++;
                Values[x, y] = values[i];
            }

            Cells = _size / cellSize;
        }

        public SudokuBoard(int[,] values, int cellSize, bool flip = false)
        {
            CellSize = cellSize;
            if (flip)
            {
                var size = values.GetLength(0);
                Values = new int[size, size];
                for (int x = 0; x < size; x++)
                    for (int y = 0; y < size; y++)
                        Values[x, y] = values[y, x];
            }
            else
                Values = values;

            if (values.GetLength(0) % cellSize != 0)
                throw new Exception("Invalid cell size!");

            _size = values.GetLength(0);
            Cells = _size / cellSize;
        }

        public bool IsComplete()
        {
            for (int x = 0; x < _size; x++)
                for (int y = 0; y < _size; y++)
                    if (Values[x, y] == BlankNumber)
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

        public int[] GetRow(int row) => Values.GetRow(row);
        public int[] GetColumn(int column) => Values.GetColumn(column);

        public bool RowContains(int row, int value)
        {
            for (int x = 0; x < _size; x++)
                if (Values[x, row] == value)
                    return true;
            return false;
        }
        public bool ColumnContains(int column, int value)
        {
            for (int y = 0; y < _size; y++)
                if (Values[column, y] == value)
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
                    if (Values[x, y] == value)
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
                    if (Values[x, y] != BlankNumber)
                        returnList.Add(Values[x, y]);
            return returnList;
        }

        public SudokuBoard Copy()
        {
            var cpyCells = new int[_size, _size];
            //Array.Copy(Values, cpyCells, _size * _size);
            Buffer.BlockCopy(Values, 0, cpyCells, 0, _size * _size * sizeof(int));
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
                    if (Values[x, y] == BlankNumber)
                        sb.Append("_");
                    else
                        sb.Append($"{Values[x, y]}");
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
