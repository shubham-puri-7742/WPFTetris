using System;

namespace WPFTetris
{
    public class BlockQueue
    {
        // an instance of each type of block
        private readonly Block[] blocks = new Block[]
        {
            new IBlock(),
            new JBlock(),
            new LBlock(),
            new OBlock(),
            new SBlock(),
            new TBlock(),
            new ZBlock()
        };

        // randomiser
        private readonly Random random = new Random();

        // preview for the next block
        public Block nextBlock { get; private set; }

        // start with a random block
        public BlockQueue()
        {
            nextBlock = randomBlock();
        }

        // randomly select a block from all the types
        private Block randomBlock()
        {
            return blocks[random.Next(blocks.Length)];
        }

        public Block getAndUpdate()
        {
            Block block = nextBlock;

            // this do loop can be removed if we allow repeats
            do
            {
                // get a random block...
                nextBlock = randomBlock();
            }
            // ...that is not the same as the current one
            while (block.id == nextBlock.id);

            return block;
        }
    }
}
