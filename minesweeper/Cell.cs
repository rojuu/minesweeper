using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace minesweeper
{
    class Cell
    {
        //textures for bomb and grid images
        //Texture2D emptyCell;
        //Texture2D overlayCell;
        //Texture2D bombCell;

        public Rectangle rectPos;
        
        bool isClicked = false;
        bool isBomb = false;
        
        int gridX, gridY;
        
        public bool IsBomb
        {
            get { return isBomb; }
            set { isBomb = value;  }
        }

        public Cell(Rectangle RectPos, int GridX, int GridY)
        {
            rectPos = RectPos;
            gridX = GridX;
            gridY = GridY;
        }

        public void Click(ref Rectangle[,] OverlayGrid)
        {
            isClicked = true;
            OverlayGrid[gridY, gridX].Width = 0;
            OverlayGrid[gridY, gridX].Height = 0;
        }
    }
}
