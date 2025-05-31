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
                if (!solver.Solve())
                    continue;
                
                var minimizer = new PuzzleMinimizer(fullGrid, constraintsH, constraintsV);
                var (minGrid, minH, minV) = minimizer.Minimize();
                return (minGrid, fullGrid, minH, minV);
            }
        }

        private char[,] GenerateFullSolution()
        {
            char[,] grid = new char[Size, Size];
            for (int i = 0; i < Size; i++)
                for (int j = 0; j < Size; j++)
                    grid[i, j] = ' ';
            bool success = FillGrid(grid, 0, 0);
            if (!success)
            {
                throw new Exception("Failed to generate full solution.");
            }
            return grid;
        }

        private bool FillGrid(char[,] grid, int row, int col)
        {
            if (col == Size)
            {
                col = 0;
                row++;
            }

            if (row == Size) return true;

            var order = rand.Next(2) == 0 ? new[] { 'X', 'O' } : new[] { 'O', 'X' };
            foreach (char c in order)
            {
                grid[row, col] = c;
                var validator = new GridValidator(grid);
                if (validator.IsPartialValid(row, col))
                {
                    if (FillGrid(grid, row, col + 1))
                        return true;
                }
            }
            grid[row, col] = ' ';
            return false;
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
