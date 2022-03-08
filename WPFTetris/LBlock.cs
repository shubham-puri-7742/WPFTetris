﻿namespace WPFTetris
{
    public class LBlock : Block
    {
        private readonly Position[][] tiles = new Position[][]
        {
            /*
            0 0 3
            3 3 3
            0 0 0
            */
            new Position[] { new(0,2), new(1,0), new(1,1), new(1,2) },
            
            /*
            0 3 0
            0 3 0
            0 3 3
            */
            new Position[] { new(0,1), new(1,1), new(2,1), new(2,2) },
            
            /*
            0 0 0
            3 3 3
            3 0 0
            */
            new Position[] { new(1,0), new(1,1), new(1,2), new(2,0) },
            
            /*
            3 3 0
            0 3 0
            0 3 0
            */
            new Position[] { new(0,0), new(0,1), new(1,1), new(2,1) }
        };

        public override int id => 3;
        protected override Position[][] Tiles => tiles;
        protected override Position startOffset => new Position(0, 3);
    }
}