namespace WPFTetris
{
    // Represents the game grid
    public class GameGrid
    {
        // row, col
        private readonly int[,] grid;
        public int rows { get; }
        public int cols { get; }

        // access the grid by grid[r, c]
        public int this[int r, int c]
        {
            get => grid[r, c];
            set => grid[r, c] = value;
        }

        // parameterised ctor
        public GameGrid(int r, int c)
        {
            rows = r;
            cols = c;

            grid = new int[rows, cols];
        }

        // check if the coordinate lies inside the grid
        public bool isInside(int r, int c)
        {
            return r >= 0 && r < rows && c >= 0 && c < cols;
        }

        // check if a cell is empty
        public bool isEmpty(int r, int c)
        {
            return isInside(r, c) && grid[r, c] == 0;
        }

        // CONVENTION: 0 = empty, nonzero = occupied
        // check if a row is full
        public bool rowFull(int r)
        {
            // for each column
            for (int c = 0; c < cols; ++c)
            {
                // if a cell in that row and col is empty
                if (grid[r, c] == 0)
                {
                    return false;
                }
            }
            return true;
        }

        // check if a row is empty (similar to rowFull)
        public bool rowEmpty(int r)
        {
            // for each column
            for (int c = 0; c < cols; ++c)
            {
                // if a cell in that row is filled
                if(grid[r, c] != 0)
                {
                    return false;
                }
            }
            return true;
        }

        // clear a row
        private void clearRow(int r)
        {
            // for each column
            for (int c = 0; c < cols; ++c)
            {
                // set the cell content to 0 (see the convention)
                grid[r, c] = 0;
            }
        }

        // move a row (r) down by a given number (delta)
        private void moveRowDown(int r, int delta)
        {
            // for each cell in the given row
            for (int c = 0; c < cols; ++c)
            {
                // copy the data to the cell delta rows below the current one
                grid[r + delta, c] = grid[r, c];
                // clear the current cell
                grid[r, c] = 0;
            }
        }

        // CORE TETRIS MECHANIC: clear rows that have been filled
        public int clearFullRows()
        {
            // initialise a counter for cleared rows
            int clearedRows = 0;

            // for each row in the grid starting from the bottom
            for (int r = rows - 1; r >= 0; --r)
            {
                // if it is full
                if (rowFull(r))
                {
                    // clear the full row
                    clearRow(r);
                    // increment the number of cleared rows
                    ++clearedRows;
                }
                // if the row is not full but row(s) has(have) been cleared before
                else if (clearedRows > 0)
                {
                    // move the current row down by the number of rows cleared thus far
                    moveRowDown(r, clearedRows);
                }
            }

            return clearedRows;
        }
    }
}
