namespace Tangorinchik
{
    public class Game
    {
        private const int Size = 6;
        private char[,] grid;
        private char[,] constraintsH;
        private char[,] constraintsV;
        private char tmp = ' ';
        private int? lastRow = null;
        private int? lastCol = null;
        private bool lastMoveValid = true;
        private char[,] firstGrid;

        private Stack<char[,]> gridHistory;
        
        public void Play()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("                      NEW GAME");
            Console.WriteLine("    Easy[E]    Medium[M]    Hard[H]    Advanced[A]");
            Console.Write("                       > ");
            Console.ResetColor();
            var filledPercentage = 0;
            do
            {
                var inputLevel = Console.ReadLine()?.Trim().ToUpper();
                if (inputLevel == null) continue;
                Dictionary<string,int> LevelDifficulty = new Dictionary<string, int>
                {
                    { "E", 40 },
                    { "M", 25 },
                    { "H", 10 },
                    { "A", 5 },
                }; 
                if (!LevelDifficulty.ContainsKey(inputLevel)) continue;
                filledPercentage = LevelDifficulty[inputLevel];
                break;
            }
            while (true);
            
            var generator = new PuzzleGenerator();
            (grid, var solution, constraintsH, constraintsV) = generator.Generate(filledPercentage);
            gridHistory = new Stack<char[,]>();
            firstGrid = (char[,])grid.Clone();
            
            while (true)
            {
                gridHistory.Push((char[,])grid.Clone());

                Console.Clear();
                Console.Write("                  ");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("TANGO PUZZLE GAME");
                Console.ResetColor();
                Console.WriteLine();
                PrintGrid();
                Console.WriteLine();
                
                if (IsComplete(solution))
                {
                    Console.Write("                  ");
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("Puzzle Solved!!!\n");
                    Console.ResetColor();
                    break;
                }

                Console.Write("Enter row (1-6), col (1-6), symbol (C/O) [ U - undo ] [R - restart]: ");
                var input = Console.ReadLine()?.Trim().ToUpper().Split();
                if (input != null && input.Length == 1)
                {
                    if (input[0] == "R")
                    {
                        Console.Clear();
                        break;
                    }
                    else if (input[0] == "U")
                    {
                        gridHistory.Pop();
                        grid = gridHistory.Pop();
                        lastRow = null;
                        lastCol = null;
                        continue;
                    }
                }
                
                if (input == null || input.Length != 3
                    || !int.TryParse(input[0], out int row)
                    || !int.TryParse(input[1], out int col))
                {
                    Console.WriteLine("Invalid input. Format: row col symbol");
                    Console.ReadKey();
                    continue;
                }

                row--; col--;
                if (row < 0 || row >= Size || col < 0 || col >= Size)
                {
                    Console.WriteLine("Out of bounds.");
                    Console.ReadKey();
                    continue;
                }
                
                if (firstGrid[row, col] != ' ')
                {
                    Console.WriteLine("Selected cell value cannot be edited.");
                    Console.ReadKey();
                    continue;
                }

                char sym = input[2][0];
                tmp = sym;
                if (sym == '0') sym = 'O';
                if (sym != 'C' && sym != 'O')
                {
                    Console.WriteLine("Symbol must be C or O.");
                    Console.ReadKey();
                    continue;
                }
                
                lastRow = row;
                lastCol = col;

                char oldSym = grid[row, col];
                grid[row, col] = sym;
                if (!IsValidMove(row, col))
                {
                    lastMoveValid = false;
                    Console.WriteLine("Invalid move.");
                    Console.ReadKey();
                    grid[row, col] = oldSym;
                    continue;
                }
                lastMoveValid = true;
            }
        }
        
        private bool IsValidMove(int row, int col)
        {
            char sym = grid[row, col];

            // 3 in a row check
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size - 2; j++)
                {
                    if (grid[i, j] == sym && grid[i, j + 1] == sym && grid[i, j + 2] == sym)
                        return false;
                    if (grid[j, i] == sym && grid[j + 1, i] == sym && grid[j + 2, i] == sym)
                        return false;
                }
            }

            // Max 3 C/O per row/col
            int rowCount = 0, colCount = 0;
            for (int i = 0; i < Size; i++)
            {
                if (grid[row, i] == sym) rowCount++;
                if (grid[i, col] == sym) colCount++;
            }
            if (rowCount > 3 || colCount > 3) return false;

            // Constraint check
            if (col < Size - 1 && constraintsH[row, col] != '\0')
            {
                if (constraintsH[row, col] == '=' && grid[row, col + 1] != ' ' && grid[row, col + 1] != sym)
                    return false;
                if (constraintsH[row, col] == 'x' && grid[row, col + 1] != ' ' && grid[row, col + 1] == sym)
                    return false;
            }
            if (col > 0 && constraintsH[row, col - 1] != '\0')
            {
                if (constraintsH[row, col - 1] == '=' && grid[row, col - 1] != ' ' && grid[row, col - 1] != sym)
                    return false;
                if (constraintsH[row, col - 1] == 'x' && grid[row, col - 1] != ' ' && grid[row, col - 1] == sym)
                    return false;
            }
            if (row < Size - 1 && constraintsV[row, col] != '\0')
            {
                if (constraintsV[row, col] == '=' && grid[row + 1, col] != ' ' && grid[row + 1, col] != sym)
                    return false;
                if (constraintsV[row, col] == 'x' && grid[row + 1, col] != ' ' && grid[row + 1, col] == sym)
                    return false;
            }
            if (row > 0 && constraintsV[row - 1, col] != '\0')
            {
                if (constraintsV[row - 1, col] == '=' && grid[row - 1, col] != ' ' && grid[row - 1, col] != sym)
                    return false;
                if (constraintsV[row - 1, col] == 'x' && grid[row - 1, col] != ' ' && grid[row - 1, col] == sym)
                    return false;
            }

            return true;
        }

        private bool IsComplete(char[,] solution)
        {
            for (int i = 0; i < Size; i++)
                for (int j = 0; j < Size; j++)
                    if (grid[i, j] != solution[i, j])
                        return false;
            return true;
        }

        private void PrintGrid()
        {
            // Print column numbers
            Console.Write("        ");
            for (int j = 0; j < Size; j++)
                Console.Write($"   {j + 1}  ");
            Console.WriteLine();
            
            // Print hbar
            Console.Write("        ");
            Console.Write("╔═════");
            for (int j = 1; j < Size; j++)
                Console.Write("╦═════");
            Console.WriteLine("╗");

            for (int i = 0; i < Size; i++)
            {
                // Print row number
                Console.Write($"      {i + 1} ║");
                
                // Print values and horizontal constraints of current row
                for (int j = 0; j < Size; j++)
                {
                    if (lastRow == i && lastCol == j)
                    {
                        if (lastMoveValid)
                        {
                            if (grid[i, j] == 'C')
                            {
                                Console.ForegroundColor = ConsoleColor.DarkBlue;
                            }
                            else if (grid[i, j] == 'O')
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                            }
                            Console.Write($"  {grid[i, j]}  ");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write($"  {tmp}  ");
                            gridHistory.Pop();
                        }
                        
                        Console.ResetColor();
                    }
                    else
                    {
                        if (grid[i, j] == 'C')
                        {
                            Console.ForegroundColor = ConsoleColor.DarkBlue;
                        }
                        else if (grid[i, j] == 'O')
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                        }

                        if (grid[i, j] == firstGrid[i, j] && grid[i, j] != ' ')
                        {
                            Console.Write("\x1b[48;2;44;44;44m");
                        }
                        
                        Console.Write($"  {grid[i, j]}  ");
                        Console.ResetColor();
                    }
                    
                    if (j < Size - 1)
                    {
                        char c = constraintsH[i, j];
                        if (c == ' ')
                        {
                            c = '║';
                        }
                        Console.Write(c);
                    }
                }
                Console.WriteLine("║");
                
                // Print vertical constraints
                if (i < Size - 1)
                {
                    Console.Write("        ");
                    for (var j = 0; j < Size; j++)
                    {
                        Console.Write(j != 0 ? "╬" : "╠");
                        char c = constraintsV[i, j];
                        if (c == ' ')
                            Console.Write($"═════");
                        else
                            Console.Write($"  {c}  ");
                    }
                    Console.WriteLine("╣");
                }
            }
            
            // Print hbar
            Console.Write("        ");
            Console.Write("╚═════");
            for (int j = 1; j < Size; j++)
                Console.Write("╩═════");
            Console.WriteLine("╝");
        }
    }
}