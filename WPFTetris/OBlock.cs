namespace WPFTetris
{
    public class OBlock : Block
    {
        private readonly Position[][] tiles = new Position[][]
        {
            /*
            4 4
            4 4
            */
            new Position[] { new(0,0), new(0,1), new(1,0), new(1,1) },
        };

        public override int id => 4;
        protected override Position[][] Tiles => tiles;
        protected override Position startOffset => new Position(0, 4);
    }
}