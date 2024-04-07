using SudokuSolver.Models;

namespace SudokuSolver.Preprocessors
{
    public interface IPreprocessor
    {
        public byte BoardSize { get; }
        public List<CellPosition> Cardinalities { get; }
        public List<CellAssignment>[,] Candidates { get; }
        public Dictionary<int, int>[,] CandidatesPrValue { get; }

        public SudokuBoard Preprocess(SudokuBoard board);
    }
}
