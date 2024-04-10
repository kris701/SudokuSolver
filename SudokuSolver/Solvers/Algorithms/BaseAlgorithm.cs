using SudokuSolver.Models;

namespace SudokuSolver.Solvers.Algorithms
{
    public abstract class BaseAlgorithm : IAlgorithm
    {
        public string Name { get; }
        public bool Stop { get; set; }
        public int Calls { get; internal set; }

        protected BaseAlgorithm(string name)
        {
            Name = name;
        }

        public abstract SearchContext Solve(SearchContext context);
    }
}
