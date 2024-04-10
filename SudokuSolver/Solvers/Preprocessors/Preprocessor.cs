using SudokuSolver.Models;

namespace SudokuSolver.Solvers.Preprocessors
{
    public static class Preprocessor
    {
        public static SearchContext Preprocess(SudokuBoard board)
        {
            var candidates = GenerateCandidates(board);
            return new SearchContext(
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
    }
}
