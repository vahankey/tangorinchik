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

            // Clear cells randomly
            var filledCellProbability = 35;
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
                    // Remove horizontal constraint
                    char backup = bestH[i, j];
                    bestH[i, j] = ' ';
                    var tempSolver = new TangoSolver(bestGrid, bestH, bestV);
                    if (!tempSolver.Solve())
                        bestH[i, j] = backup;
                    
                    // Remove vertical constraint
                    backup = bestV[j, i];
                    bestV[j, i] = ' ';
                    tempSolver = new TangoSolver(bestGrid, bestH, bestV);
                    if (!tempSolver.Solve())
                        bestV[j, i] = backup;
                }
            }

            return (bestGrid, bestH, bestV);
        }
    }
}