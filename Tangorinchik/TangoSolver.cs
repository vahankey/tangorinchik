using System.Reflection.Metadata.Ecma335;

namespace Tangorinchik
{
    class TangoSolver
    {
        private const int Size = 6;
        private char[,] grid = new char[Size, Size];
        private char[,] constraintsH;
        private char[,] constraintsV;

        public TangoSolver(char[,] inputGrid, char[,] hConstraints, char[,] vConstraints)
        {
            for (int i = 0; i < Size; i++)
                for (int j = 0; j < Size; j++)
                    grid[i, j] = inputGrid[i, j];

            constraintsH = hConstraints;
            constraintsV = vConstraints;
        }

        public int Solve(int row = 0, int col = 0)
        {
            if (row == Size)
            {
                // Grid is fully filled
                var validator = new GridValidator(grid, constraintsH, constraintsV);
                return validator.IsFullValid() ? 1 : 0;
            }

            var nextRow = row;
            var nextCol = col + 1;
            if (nextCol == Size)
            {
                nextRow = row + 1;
                nextCol = 0;
            }

            if (grid[row, col] != ' ')
                return Solve(nextRow, nextCol);
            
            var solutionCount = 0;
            foreach (char symbol in new char[] { 'C', 'O' })
            {
                grid[row, col] = symbol;
                var validator = new GridValidator(grid, constraintsH, constraintsV);
                if (validator.IsPartialValid(row, col))
                {
                    solutionCount += Solve(nextRow, nextCol);
                }
                grid[row, col] = ' ';
            }
            return solutionCount;
        }
    }
}
