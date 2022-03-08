namespace WPFTetris
{
    // Represents a position in the game world
    public class Position
    {
        public int row { get; set; }
        public int col { get; set; }

        // ctor
        public Position(int r, int c)
        {
            row = r;
            col = c;
        }
    }
}
