namespace WPFTetris
{
    public class TBlock : Block
    {
        private readonly Position[][] tiles = new Position[][]
        {
            /*
            0 6 0
            6 6 6
            0 0 0
            */
            new Position[] { new(0,1), new(1,0), new(1,1), new(1,2) },
            
            /*
            0 6 0
            0 6 6
            0 6 0
            */
            new Position[] { new(0,1), new(1,1), new(1,2), new(2,1) },
            
            /*
            0 0 0
            6 6 6
            0 6 0
            */
            new Position[] { new(1,0), new(1,1), new(1,2), new(2,1) },
            
            /*
            0 6 0
            6 6 0
            0 6 0
            */
            new Position[] { new(0,1), new(1,0), new(1,1), new(2,1) }
        };

        public override int id => 6;
        protected override Position[][] Tiles => tiles;
        protected override Position startOffset => new Position(0, 3);
    }
}