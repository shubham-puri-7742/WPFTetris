namespace WPFTetris
{
    public class GameState
    {
        private Block currentBlock;

        public Block CurrentBlock
        {
            // gets the private member currentBlock
            get => currentBlock;
            private set
            {
                // sets the current block to the specified value
                currentBlock = value;
                // and resets the block transform
                currentBlock.reset();
                // move the block down by two rows if nothing is in the way
                for (int i = 0; i < 2; ++i)
                {
                    currentBlock.move(1, 0);

                    // ... if it fits
                    if (!blockFits())
                    {
                        currentBlock.move(-1, 0);
                    }
                }
            }
        }

        // game variables
        // the grid (play area)
        public GameGrid gameGrid { get; }
        // the block queue
        public BlockQueue blockQueue { get; }
        // a boolean for the game state
        public bool gameOver { get; private set; }
        // an int score value
        public int score { get; private set; }
        // the block currently held
        public Block heldBlock { get; private set; }
        // boolean for whether the player can hold a block (we only allow holding one block)
        public bool canHold { get; private set; }

        // ctor
        public GameState()
        {
            // 22 x 10 grid
            gameGrid = new GameGrid(22, 10);
            // initialise a new queue
            blockQueue = new BlockQueue();
            // and get a block from it
            CurrentBlock = blockQueue.getAndUpdate();
            // start empty so the player can hold one block
            canHold = true;
        }

        // check if the block fits. Used to validate transforms
        private bool blockFits()
        {
            // for each position in the tile positions of the block
            foreach (Position p in CurrentBlock.tilePositions())
            {
                // if any of the corresponding cells is filled, i.e.
                // if any part of the block is outside the grid or overlap another tile
                if (!gameGrid.isEmpty(p.row, p.col))
                {
                    return false;
                }
            }
            return true;
        }

        public void holdBlock()
        {
            // if we can't hold (e.g. already holding a block), do nothing
            if (!canHold)
            {
                return;
            }

            // if we are not holding anything
            if (heldBlock == null)
            {
                // hold the current block
                heldBlock = currentBlock;
                // get a new block from the queue
                currentBlock = blockQueue.getAndUpdate();
            }
            // if the held block is not null
            else
            {
                // swap the current block with the held block
                Block temp = currentBlock;
                currentBlock = heldBlock;
                heldBlock = temp;
            }

            // cannot hold anymore
            canHold = false;
        }

        // Rotates a block clockwise
        public void rotateBlockCW()
        {
            CurrentBlock.rotateCW();

            // ...but only if it fits
            if (!blockFits())
            {
                // Otherwise, undo the rotation
                CurrentBlock.rotateCCW();
            }
        }

        // Rotates a block counterclockwise
        public void rotateBlockCCW()
        {
            CurrentBlock.rotateCCW();

            // ...but only if it fits
            if (!blockFits())
            {
                // Otherwise, undo the rotation
                CurrentBlock.rotateCW();
            }
        }

        // Moves a block left
        public void moveBlockLeft()
        {
            CurrentBlock.move(0, -1);

            // ...but only if it fits
            if (!blockFits())
            {
                CurrentBlock.move(0, 1);
            }
        }

        // Moves a block right
        public void moveBlockRight()
        {
            CurrentBlock.move(0, 1);

            // ...but only if it fits
            if (!blockFits())
            {
                CurrentBlock.move(0, -1);
            }
        }

        // checks for game over
        private bool isGameOver()
        {
            // which is when either of the top two (hidden) rows is non-empty
            return !(gameGrid.rowEmpty(0) && gameGrid.rowEmpty(1));
        }

        // Places blocks. Called when the block cannot be moved down
        private void placeBlock()
        {
            // for each of the cells in the block
            foreach (Position p in CurrentBlock.tilePositions())
            {
                // place it in the grid by setting the respective IDs
                gameGrid[p.row, p.col] = CurrentBlock.id;
            }

            // clear any rows that have been filled and add the number of cleared rows (= return value of clearFullRows) to the score
            score += gameGrid.clearFullRows();

            // if the game is over
            if (isGameOver())
            {
                // set the state as such
                gameOver = true;
            }
            else
            {
                // otherwise, get the next block from the queue
                CurrentBlock = blockQueue.getAndUpdate();
                // can hold again
                canHold = true;
            }
        }

        // Moves a block down and places it at the lowest point it fits
        public void moveBlockDown()
        {
            CurrentBlock.move(1, 0);

            // the last move down doesn't fit
            if (!blockFits())
            {
                // undo it
                CurrentBlock.move(-1, 0);
                // and place the block at the last position
                placeBlock();
            }
        }

        // returns how many rows to move to 'drop' a tile instantly
        private int tileDropDistance(Position p)
        {
            // initialise a counter
            int drop = 0;

            // as long as the corresponding column in the next row is empty
            while (gameGrid.isEmpty(p.row + drop + 1, p.col))
            {
                // keep incrementing the counter
                ++drop;
            }

            return drop;
        }

        // returns how many rows to move to 'drop' a block instantly
        public int blockDropDistance()
        {
            // initialise the drop as the number of rows (this is the max we can go down - from the starting position to the bottom row)
            int drop = gameGrid.rows;

            // for each tile in the current block
            foreach (Position p in CurrentBlock.tilePositions())
            {
                // update the drop distance based on the distance that tile has to drop to fit
                drop = System.Math.Min(drop, tileDropDistance(p));
            }

            return drop;
        }

        // drops a block by moving it as many rows down as possible and places it
        public void dropBlock()
        {
            // move it down by the block drop distance as calculated above
            currentBlock.move(blockDropDistance(), 0);
            // and place it
            placeBlock();
        }
    }
}
