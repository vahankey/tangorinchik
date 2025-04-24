// File: TangoSolver.cs
using System;
using System.Collections.Generic;

namespace TangoPuzzle
{
    public class TangoSolver
    {
        private const int Size = 6;
        private Random _rand = new Random();

        public char[,] GenerateFullSolution()
        {
            // Dummy solution: alternating X and Y in each row (not necessarily valid in real puzzle)
            var grid = new char[Size, Size];
            for (int i = 0; i < Size; i++)
            {
                int countX = 0, countY = 0;
                for (int j = 0; j < Size; j++)
                {
                    char sym = ((j + i) % 2 == 0) ? 'X' : 'Y';
                    grid[i, j] = sym;
                }
            }
            return grid;
        }

        char RandomConstraint()
        {
            int val = _rand.Next(10);
            return val switch
            {
                0 => 'x',
                1 => '=',
                _ => ' ' // բաց թողած (չկա constraint)
            };
        }
        
        public (char[,], char[,]) GenerateConstraints(char[,] solution)
        {
            var h = new char[Size, Size - 1];
            var v = new char[Size - 1, Size];

            for (int i = 0; i < Size; i++)
            for (int j = 0; j < Size - 1; j++)
                h[i, j] = RandomConstraint();

            for (int i = 0; i < Size - 1; i++)
            for (int j = 0; j < Size; j++)
                v[i, j] = RandomConstraint();

            return (h, v);
        }
    }
}