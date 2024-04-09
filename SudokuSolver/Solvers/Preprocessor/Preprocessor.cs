using SudokuSolver.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Solvers.Preprocessors
{
    public static class Preprocessor
    {
        public static SearchContext Preprocess(SudokuBoard board)
        {
            var candidates = GenerateCandidates(board);
            var cardinalities = GenerateCardinalities(board, candidates);
            return new SearchContext(
                cardinalities,
                candidates,
                board
                );
        }

        public static List<CellAssignment>[,] GenerateCandidates(SudokuBoard board)
        {
            var actions = new List<CellAssignment>[SudokuBoard.BoardSize, SudokuBoard.BoardSize];

            for (byte x = 0; x < SudokuBoard.BoardSize; x++)
            {
                var column = board.GetColumn(ref x);
                for (byte y = 0; y < SudokuBoard.BoardSize; y++)
                {
                    actions[x, y] = new List<CellAssignment>();
                    if (board[x, y] != SudokuBoard.BlankNumber)
                        continue;

                    var row = board.GetRow(ref y);

                    var blockX = board.BlockX(ref x);
                    var blockY = board.BlockY(ref y);
                    var cellValues = board.GetBlockValues(ref blockX, ref blockY);
                    for (byte i = 1; i <= SudokuBoard.BoardSize; i++)
                    {
                        if (cellValues.Contains(i))
                            continue;
                        if (!row.Contains(i) && !column.Contains(i))
                            actions[x, y].Add(new CellAssignment(x, y, i));
                    }
                }
            }

            return actions;
        }

        public static List<CellPosition> GenerateCardinalities(SudokuBoard board, List<CellAssignment>[,] candidates)
        {
            var cardinalities = new List<CellPosition>();
            for (byte x = 0; x < SudokuBoard.BoardSize; x++)
            {
                for (byte y = 0; y < SudokuBoard.BoardSize; y++)
                {
                    if (board[x, y] != SudokuBoard.BlankNumber)
                        continue;
                    cardinalities.Add(new CellPosition(x, y, candidates[x, y].Count));
                }
            }
            if (cardinalities.Any(x => x.Possibilities == 0))
                throw new Exception("Invalid preprocessing");
            cardinalities = cardinalities.OrderBy(x => x.Possibilities).ToList();
            return cardinalities;
        }
    }
}
