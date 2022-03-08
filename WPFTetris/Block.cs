using System.Collections.Generic;

namespace WPFTetris
{
    // Base abstract class for a block. Each type of block is derived from it
    public abstract class Block
    {
        // position array (tile positions in four rotation states)
        protected abstract Position[][] Tiles { get; }
        // spawnpoint in terms of an (x, y) offset
        protected abstract Position startOffset { get; }
        // block ID for setting assets 
        public abstract int id { get; }
        // each block has four possible rotations
        private int rotationState;
        // current offset
        private Position offset;

        // ctor
        public Block()
        {
            // initialise with the start offset
            offset = new Position(startOffset.row, startOffset.col);
        }

        // set the tile position
        public IEnumerable<Position> tilePositions()
        {
            // for each tile position
            foreach (Position p in Tiles[rotationState])
            {
                // add the row and column offset
                yield return new Position(p.row + offset.row, p.col + offset.col);
            }
        }
        
        // rotate clockwise
        public void rotateCW()
        {
            // increment the rotation state; wrap around the edge
            rotationState = (rotationState + 1) % Tiles.Length;
        }

        // rotate counterclockwise
        public void rotateCCW()
        {
            if (rotationState == 0)
            {
                rotationState = Tiles.Length - 1;
            }
            else
            {
                --rotationState;
            }
        }

        // move by (r, c)
        public void move(int r, int c)
        {
            offset.row += r;
            offset.col += c;
        }

        // reset the transform
        public void reset()
        {
            rotationState = 0;
            offset.row = startOffset.row;
            offset.col = startOffset.col;
        }
    }
}
