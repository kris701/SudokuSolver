﻿namespace SudokuSolver.Solvers.BacktrackSolvers
{
    public class SearchOptions
    {
        public bool EnableLog { get; set; } = true;
        public bool GroundLegalCandidatesOnly { get; set; } = true;
        public bool PruneCertains { get; set; } = true;
        public bool PruneHiddenPairs { get; set; } = true;
        public bool PruneNakedPairs { get; set; } = true;
    }
}
