namespace Tangorinchik;

public class GridValidator
{
    private const int Size = 6;
    private char[,] grid;
    private char[,] constraintsH;
    private char[,] constraintsV;
    private bool hasConstraints;
    
    public GridValidator(char[,] grid)
    {
        this.grid = grid;
        this.hasConstraints = false;
    }
    
    public GridValidator(char[,] grid, char[,] constraintsH, char[,] constraintsV)
    {
        this.grid = grid;
        this.constraintsH = constraintsH;
        this.constraintsV = constraintsV;
        this.hasConstraints = true;
    }
    
    public bool IsFullValid()
    {
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                if (!IsPartialValid(i, j)) return false;
            }
        }

        return true;
    }
    
    public bool IsPartialValid(int row, int col)
    {
        if (grid[row, col] != 'C' && grid[row, col] != 'O')
        {
            return false;
        }
        return !HasThreeInARow(row, col)
               && RowColCountsValid(row, col)
               && (!this.hasConstraints || CheckConstraints(row, col));
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
            if (grid[row, i] == 'C') rowX++;
            if (grid[row, i] == 'O') rowO++;
            if (grid[i, col] == 'C') colX++;
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
}