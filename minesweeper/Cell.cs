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

        //cellSize in pixels
        //int cellSize;

        bool isClicked = false;
        bool isBomb = false;
        
        public bool IsBomb
        {
            get { return isBomb; }
            set { isBomb = value;  }
        }

        public Cell(/*Texture2D EmptyCell, Texture2D OverlayCell, Texture2D BombCell, int CellSize,*/Rectangle RectanglePosition)
        {
            //emptyCell = EmptyCell;
            //overlayCell = OverlayCell;
            //bombCell = BombCell;
            //cellSize = CellSize;
            rectPos = RectanglePosition;
        }

        public void Click()
        {

        }
    }
}
