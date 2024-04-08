﻿using SudokuSolver.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Solvers.BacktrackSolvers.Pruners
{
    public class PointingPairsPruner : BasePruner
    {
        public override bool Prune(SearchContext context)
        {
            var any = false;
            while (PrunePointingPairs(context)) { any = true; }
            return any;
        }

        private bool PrunePointingPairs(SearchContext context)
        {
            var pruned = 0;

            for (byte blockX = 0; blockX < context.Board.Blocks; blockX++)
            {
                for (byte blockY = 0; blockY < context.Board.Blocks; blockY++)
                {
                    var cellPossibilities = GetAssignmentsFromBlock(context, blockX, blockY);

                    for (byte i = 1; i <= context.Board.BoardSize; i++)
                    {
                        var valueAssignments = cellPossibilities.Where(x => x.Value == i).ToList();
                        if (IsRowAlligned(valueAssignments))
                            pruned += PruneValueCandidatesFromColumn(context, valueAssignments, i); 
                        else if (IsColumnAlligned(valueAssignments))
                            pruned += PruneValueCandidatesFromRow(context, valueAssignments, i);
                    }
                }
            }

            if (pruned > 0)
                Console.WriteLine($"Removed {pruned} candidates because of pointing pairs");
            return pruned > 0;
        }
    }
}
