namespace Tangorinchik
{
    public class PuzzleMinimizer
    {
        private readonly int Size = 6;
        private char[,] grid;
        private char[,] constraintsH;
        private char[,] constraintsV;

        private Random rand = new Random();
        private bool GetRandomWithProbability(int x, int y)
        {
            return rand.Next(y) < x;
        }
        
        public PuzzleMinimizer(char[,] grid, char[,] h, char[,] v)
        {
            this.grid = (char[,])grid.Clone();
            this.constraintsH = (char[,])h.Clone();
            this.constraintsV = (char[,])v.Clone();
        }

        public (char[,], char[,], char[,]) Minimize(int filledPercentage)
        {
            var solver = new TangoSolver(grid, constraintsH, constraintsV);
            if (solver.Solve() != 1)
            {
                throw new Exception("Puzzle must have exactly one solution to minimize.");
            }

            char[,] bestGrid = (char[,])grid.Clone();
            char[,] bestH = (char[,])constraintsH.Clone();
            char[,] bestV = (char[,])constraintsV.Clone();

            // Clear cells randomly
            var filledCellCount = 0;
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    if (!GetRandomWithProbability(filledPercentage, 100))
                        bestGrid[i, j] = ' ';
                    else filledCellCount++;
                }
            }

            if (filledCellCount == 0)
            {
                int i = rand.Next(0, Size);
                int j = rand.Next(0, Size);
                bestGrid[i, j] = grid[i, j];
            }

            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size - 1; j++)
                {
                    // Remove horizontal constraint
                    char backup = bestH[i, j];
                    bestH[i, j] = ' ';
                    solver = new TangoSolver(bestGrid, bestH, bestV);
                    if (solver.Solve() != 1)
                        bestH[i, j] = backup;
                    
                    // Remove vertical constraint
                    backup = bestV[j, i];
                    bestV[j, i] = ' ';
                    solver = new TangoSolver(bestGrid, bestH, bestV);
                    if (solver.Solve() != 1)
                        bestV[j, i] = backup;
                }
            }
            
            return (bestGrid, bestH, bestV);
        }
    }
}