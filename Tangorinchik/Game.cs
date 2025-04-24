// File: Game.cs
using System;

namespace TangoPuzzle
{
    public class Game
    {
        private const int Size = 6;
        private char[,] grid = new char[Size, Size];
        private char[,] constraintsH;
        private char[,] constraintsV;
        private char[,] solution;

        public void Play()
        {
            var generator = new PuzzleGenerator();
            (solution, constraintsH, constraintsV) = generator.Generate();

            for (int i = 0; i < Size; i++)
                for (int j = 0; j < Size; j++)
                    grid[i, j] = '.';

            while (true)
            {
                Console.Clear();
                Console.WriteLine("TANGO PUZZLE GAME");
                PrintGrid();

                Console.Write("Enter row (1-6), col (1-6), symbol (X/Y): ");
                var input = Console.ReadLine()?.Trim().ToUpper().Split();
                if (input == null || input.Length != 3 ||
                    !int.TryParse(input[0], out int row) ||
                    !int.TryParse(input[1], out int col))
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

                char sym = input[2][0];
                if (sym != 'X' && sym != 'Y')
                {
                    Console.WriteLine("Symbol must be X or Y.");
                    Console.ReadKey();
                    continue;
                }

                char old_sym = grid[row, col];
                grid[row, col] = sym;
                if (!IsValidMove(row, col))
                {
                    Console.WriteLine("Invalid move.");
                    Console.ReadKey();
                    grid[row, col] = old_sym;
                    continue;
                }

                if (IsComplete())
                {
                    Console.Clear();
                    PrintGrid();
                    Console.WriteLine("\n🎉 Puzzle Solved!");
                    break;
                }
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

            // Max 3 X/Y per row/col
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
                if (constraintsH[row, col] == '=' && grid[row, col + 1] != '.' && grid[row, col + 1] != sym)
                    return false;
                if (constraintsH[row, col] == 'x' && grid[row, col + 1] != '.' && grid[row, col + 1] == sym)
                    return false;
            }
            if (col > 0 && constraintsH[row, col - 1] != '\0')
            {
                if (constraintsH[row, col - 1] == '=' && grid[row, col - 1] != '.' && grid[row, col - 1] != sym)
                    return false;
                if (constraintsH[row, col - 1] == 'x' && grid[row, col - 1] != '.' && grid[row, col - 1] == sym)
                    return false;
            }
            if (row < Size - 1 && constraintsV[row, col] != '\0')
            {
                if (constraintsV[row, col] == '=' && grid[row + 1, col] != '.' && grid[row + 1, col] != sym)
                    return false;
                if (constraintsV[row, col] == 'x' && grid[row + 1, col] != '.' && grid[row + 1, col] == sym)
                    return false;
            }
            if (row > 0 && constraintsV[row - 1, col] != '\0')
            {
                if (constraintsV[row - 1, col] == '=' && grid[row - 1, col] != '.' && grid[row - 1, col] != sym)
                    return false;
                if (constraintsV[row - 1, col] == 'x' && grid[row - 1, col] != '.' && grid[row - 1, col] == sym)
                    return false;
            }

            return true;
        }

        private bool IsComplete()
        {
            for (int i = 0; i < Size; i++)
                for (int j = 0; j < Size; j++)
                    if (grid[i, j] == '.')
                        return false;
            return true;
        }

        private void PrintGrid()
        {
            // Print column numbers
            Console.Write("  ");
            for (int j = 0; j < Size; j++)
                Console.Write($" {j + 1}");
            Console.WriteLine();

            for (int i = 0; i < Size; i++)
            {
                // Print row number
                Console.Write($"{i + 1}  ");
                
                // Print values and horizontal constraints of current row
                for (int j = 0; j < Size; j++)
                {
                    Console.Write(grid[i, j]);
                    if (j < Size - 1)
                        Console.Write(constraintsH ? [i, j] != '\0' ? constraintsH[i, j] : ' ');
                }
                Console.WriteLine();
                
                // Print vertical constraints
                if (i < Size - 1)
                {
                    Console.Write("   ");
                    for (int j = 0; j < Size; j++)
                    {
                        Console.Write(constraintsV?[i, j] != '\0' ? constraintsV[i, j] : ' ');
                        Console.Write(' ');
                    }
                    Console.WriteLine();
                }
            }
        }
    }
}