﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Models
{
    public class SearchContext
    {
        public List<CellPosition> Cardinalities { get; set; }
        public List<CellAssignment>[,] Candidates { get; set; }
        public SudokuBoard Board { get; set; }

        public SearchContext(List<CellPosition> cardinalities, List<CellAssignment>[,] candidates, SudokuBoard board)
        {
            Cardinalities = cardinalities;
            Candidates = candidates;
            Board = board;
        }
    }
}