/*// File: PuzzleMinimizer.cs
namespace TangoPuzzle
{
    public class PuzzleMinimizer
    {
        private readonly char[,] solution;
        private readonly char[,] hConstraints;
        private readonly char[,] vConstraints;

        public PuzzleMinimizer(char[,] solution, char[,] h, char[,] v)
        {
            this.solution = solution;
            this.hConstraints = h;
            this.vConstraints = v;
        }

        public (char[,], char[,]) Minimize()
        {
            // For now, return original constraints (no minimization logic yet)
            return (hConstraints, vConstraints);
        }
    }
}*/
using System;
using System.Collections.Generic;

namespace TangoPuzzle
{
    public class PuzzleMinimizer
    {
        private readonly int Size;
        private char[,] grid;
        private char[,] constraintsH;
        private char[,] constraintsV;

        private Random _rand = new Random();
        private bool GetRandomWithProbability(int x, int y)
        {
            return _rand.Next(y) < x ? true : false;
        }
        
        public PuzzleMinimizer(char[,] grid, char[,] h, char[,] v)
        {
            this.Size = grid.GetLength(0);
            this.grid = (char[,])grid.Clone();
            this.constraintsH = (char[,])h.Clone();
            this.constraintsV = (char[,])v.Clone();
        }

        public (char[,], char[,], char[,]) Minimize()
        {
            var solver = new TangoSolver(grid, constraintsH, constraintsV);
            if (!solver.Solve())
            {
                throw new Exception("Puzzle must have exactly one solution to minimize.");
            }

            char[,] bestGrid = (char[,])grid.Clone();
            char[,] bestH = (char[,])constraintsH.Clone();
            char[,] bestV = (char[,])constraintsV.Clone();

            var filledCellProbability = 40;
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    if (!GetRandomWithProbability(filledCellProbability, 100))
                        bestGrid[i, j] = ' ';
                }
            }

            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size - 1; j++)
                {
                    char backup = bestH[i, j];
                    bestH[i, j] = ' ';
                    var tempSolver = new TangoSolver(bestGrid, bestH, bestV);
                    if (!tempSolver.Solve())
                        bestH[i, j] = backup;
                }
            }

            for (int i = 0; i < Size - 1; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    char backup = bestV[i, j];
                    bestV[i, j] = ' ';
                    var tempSolver = new TangoSolver(bestGrid, bestH, bestV);
                    if (!tempSolver.Solve())
                        bestV[i, j] = backup;

                }
            }

            return (bestGrid, bestH, bestV);
        }
    }
}