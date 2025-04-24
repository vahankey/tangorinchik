namespace Tangorinchik
{
    public class PuzzleGenerator
    {
        private readonly int Size = 6;
        private Random rand = new Random();

        public (char[,], char[,], char[,], char[,]) Generate()
        {
            while (true)
            {
                var fullGrid = GenerateFullSolution();
                var (constraintsH, constraintsV) = GenerateFullConstraints(fullGrid);
                
                var solver = new TangoSolver(fullGrid, constraintsH, constraintsV);
                if (!solver.Solve()) continue;
                
                var minimizer = new PuzzleMinimizer(fullGrid, constraintsH, constraintsV);
                var (minGrid, minH, minV) = minimizer.Minimize();
                return (minGrid, fullGrid, minH, minV);
            }
        }

        private char[,] GenerateFullSolution()
        {
            char[,] grid = new char[Size, Size];
            bool success = FillGrid(grid, 0, 0);
            return success ? grid : throw new Exception("Failed to generate full solution.");
        }

        private bool FillGrid(char[,] grid, int row, int col)
        {
            if (col == Size)
            {
                col = 0;
                row++;
            }

            if (row == Size) return true;

            foreach (char c in rand.Next(2) == 0 ? new[] { 'X', 'O' } : new[] { 'O', 'X' })
            {
                grid[row, col] = c;
                if (IsValid(grid, row, col))
                {
                    if (FillGrid(grid, row, col + 1))
                        return true;
                }

                grid[row, col] = ' ';
            }

            return false;
        }

        private bool IsValid(char[,] grid, int row, int col)
        {
            int xCountRow = 0, yCountRow = 0;
            int xCountCol = 0, yCountCol = 0;

            for (int i = 0; i < Size; i++)
            {
                if (grid[row, i] == 'X') xCountRow++;
                if (grid[row, i] == 'O') yCountRow++;
                if (grid[i, col] == 'X') xCountCol++;
                if (grid[i, col] == 'O') yCountCol++;
            }

            if (xCountRow > 3 || yCountRow > 3 || xCountCol > 3 || yCountCol > 3)
                return false;

            if (col >= 2 && grid[row, col] == grid[row, col - 1] && grid[row, col] == grid[row, col - 2])
                return false;
            if (row >= 2 && grid[row, col] == grid[row - 1, col] && grid[row, col] == grid[row - 2, col])
                return false;

            return true;
        }

        private (char[,], char[,]) GenerateFullConstraints(char[,] grid)
        {
            char[,] h = new char[Size, Size - 1];
            char[,] v = new char[Size - 1, Size];

            for (int i = 0; i < Size; i++)
            for (int j = 0; j < Size - 1; j++)
                h[i, j] = grid[i, j] == grid[i, j + 1] ? '=' : 'x';

            for (int i = 0; i < Size - 1; i++)
            for (int j = 0; j < Size; j++)
                v[i, j] = grid[i, j] == grid[i + 1, j] ? '=' : 'x';

            return (h, v);
        }
    }
}
