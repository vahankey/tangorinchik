namespace Tangorinchik
{
    class TangoSolver
    {
        private const int Size = 6;
        private char[,] grid = new char[Size, Size];
        private char[,] constraintsH;
        private char[,] constraintsV;
        private char[,] solution = new char[Size, Size];

        public TangoSolver(char[,] inputGrid, char[,] hConstraints, char[,] vConstraints)
        {
            // Copy input
            for (int i = 0; i < Size; i++)
            for (int j = 0; j < Size; j++)
                grid[i, j] = inputGrid[i, j];

            constraintsH = hConstraints;
            constraintsV = vConstraints;
        }

        public char[,] GetSolution()
        {
            return solution;
        }

        public bool Solve(int row = 0, int col = 0)
        {
            if (row == Size)
            {
                // Grid is fully filled
                if (!IsValidGrid()) return false;
                CloneGrid(solution);
                return true;
            }

            var nextRow = row;
            var nextCol = (col + 1) % Size;
            if (nextCol == 0)
            {
                nextRow = row + 1;
            }

            var solutionCount = 0;
            foreach (char symbol in new char[] { 'X', 'O' })
            {
                grid[row, col] = symbol;
                if (IsPartialValid(row, col))
                {
                    if (Solve(nextRow, nextCol))
                    {
                        solutionCount++;
                    }
                }
            }

            return solutionCount == 1;
        }

        private bool IsPartialValid(int row, int col)
        {
            return !HasThreeInARow(row, col)
                   && RowColCountsValid(row, col)
                   && CheckConstraints(row, col);
        }

        private bool IsValidGrid()
        {
            for (int i = 0; i < Size; i++)
            {
                if (grid[i, i] != 'X' && grid[i, i] != 'O') return false;
                int xRow = 0, yRow = 0, xCol = 0, yCol = 0;
                for (int j = 0; j < Size; j++)
                {
                    if (grid[i, j] == 'X') xRow++;
                    if (grid[i, j] == 'O') yRow++;
                    if (grid[j, i] == 'X') xCol++;
                    if (grid[j, i] == 'O') yCol++;
                }

                if (xRow != 3 || yRow != 3 || xCol != 3 || yCol != 3)
                    return false;
            }

            return true;
        }

        private bool HasThreeInARow(int row, int col)
        {
            char s = grid[row, col];
            if (s == ' ') return false;

            // Check horizontally
            if (col >= 2 && grid[row, col - 1] == s && grid[row, col - 2] == s) return true;
            if (col >= 1 && col < Size - 1 && grid[row, col - 1] == s && grid[row, col + 1] == s) return true;
            if (col < Size - 2 && grid[row, col + 1] == s && grid[row, col + 2] == s) return true;

            // Check vertically
            if (row >= 2 && grid[row - 1, col] == s && grid[row - 2, col] == s) return true;
            if (row >= 1 && row < Size - 1 && grid[row - 1, col] == s && grid[row + 1, col] == s) return true;
            if (row < Size - 2 && grid[row + 1, col] == s && grid[row + 2, col] == s) return true;

            return false;
        }

        private bool RowColCountsValid(int row, int col)
        {
            int rowX = 0, rowO = 0, colX = 0, colO = 0;
            for (int i = 0; i < Size; i++)
            {
                if (grid[row, i] == 'X') rowX++;
                if (grid[row, i] == 'O') rowO++;
                if (grid[i, col] == 'X') colX++;
                if (grid[i, col] == 'O') colO++;
            }

            return rowX <= 3 && rowO <= 3 && colX <= 3 && colO <= 3;
        }

        private bool CheckConstraints(int row, int col)
        {
            char s = grid[row, col];

            // Horizontal
            if (col < Size - 1 && grid[row, col + 1] != ' ')
            {
                char next = grid[row, col + 1];
                char c = constraintsH[row, col];
                if (c == '=' && s != next) return false;
                if (c == 'x' && s == next) return false;
            }

            if (col > 0 && grid[row, col - 1] != ' ')
            {
                char prev = grid[row, col - 1];
                char c = constraintsH[row, col - 1];
                if (c == '=' && s != prev) return false;
                if (c == 'x' && s == prev) return false;
            }

            // Vertical
            if (row < Size - 1 && grid[row + 1, col] != ' ')
            {
                char next = grid[row + 1, col];
                char c = constraintsV[row, col];
                if (c == '=' && s != next) return false;
                if (c == 'x' && s == next) return false;
            }

            if (row > 0 && grid[row - 1, col] != ' ')
            {
                char prev = grid[row - 1, col];
                char c = constraintsV[row - 1, col];
                if (c == '=' && s != prev) return false;
                if (c == 'x' && s == prev) return false;
            }

            return true;
        }

        private char[,] CloneGrid()
        {
            char[,] copy = new char[Size, Size];
            for (int i = 0; i < Size; i++)
            for (int j = 0; j < Size; j++)
                copy[i, j] = grid[i, j];
            return copy;
        }

        private void CloneGrid(char[,] copy)
        {
            for (int i = 0; i < Size; i++)
            for (int j = 0; j < Size; j++)
                copy[i, j] = grid[i, j];
        }
    }
}
