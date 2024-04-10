namespace SudokuSolver.Models
{
    public class SearchContext
    {
        public List<CellAssignment>[,] Candidates { get; set; }
        public SudokuBoard Board { get; set; }

        public SearchContext(List<CellAssignment>[,] candidates, SudokuBoard board)
        {
            Candidates = candidates;
            Board = board;
        }

        public SearchContext Copy() => new SearchContext(Candidates, Board.Copy());
    }
}
