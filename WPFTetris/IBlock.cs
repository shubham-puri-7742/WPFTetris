namespace WPFTetris
{
    public class IBlock : Block
    {
        private readonly Position[][] tiles = new Position[][]
        {
            /*
            0 0 0 0
            1 1 1 1
            0 0 0 0
            0 0 0 0
            */
            new Position[] { new(1,0), new(1,1), new(1,2), new(1,3) },
            
            /*
            0 0 1 0
            0 0 1 0
            0 0 1 0
            0 0 1 0
            */
            new Position[] { new(0,2), new(1,2), new(2,2), new(3,2) },
            
            /*
            0 0 0 0
            0 0 0 0
            1 1 1 1
            0 0 0 0
            */
            new Position[] { new(2,0), new(2,1), new(2,2), new(2,3) },
            
            /*
            0 1 0 0
            0 1 0 0
            0 1 0 0
            0 1 0 0
            */
            new Position[] { new(0,1), new(1,1), new(2,1), new(3,1) }
        };

        public override int id => 1;
        protected override Position[][] Tiles => tiles;
        protected override Position startOffset => new Position(-1, 3);
    }
}
