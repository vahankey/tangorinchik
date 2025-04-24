// File: PuzzleGenerator.cs
using System;

namespace TangoPuzzle
{
    public class PuzzleGenerator
    {
        public (char[,], char[,], char[,]) Generate()
        {
            var solver = new TangoSolver();
            var solution = solver.GenerateFullSolution();
            var (h, v) = solver.GenerateConstraints(solution);

            var minimizer = new PuzzleMinimizer(solution, h, v);
            var (hMin, vMin) = minimizer.Minimize();

            return (solution, hMin, vMin);
        }
    }
}