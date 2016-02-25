using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace minesweeper
{
    class Cell
    {
        public Rectangle rectPos;

        List<Cell> adjacentCells;

        bool isClicked = false;
        
        int gridX, gridY;

        int cellVal;

        public bool IsClicked
        {
            get { return isClicked; }
        }

        public int CellValue
        {
            get { return cellVal; }
        }


        /// <summary>
        /// Stores cell information and handles cell logic
        /// </summary>
        /// <param name="RectPos">Rectangle position of cell</param>
        /// <param name="GridX">X position of cell in int[] grid</param>
        /// <param name="GridY">Y position of cell in int[] grid</param>
        /// <param name="CellValue">Value of cell in int[] grid</param>
        public Cell(Rectangle RectPos, int GridX, int GridY, int CellValue)
        {
            rectPos = RectPos;
            gridX = GridX;
            gridY = GridY;
            cellVal = CellValue;

            adjacentCells = new List<Cell>();
        }

        /// <summary>
        /// Clicks on self. If cell was empty, clicks any adjacent cells that are not bombs. Returns true if bomb was hit.
        /// </summary>
        /// <param name="OverlayGrid">overlayGrid array</param>
        /// <param name="CellGrid">cellGrid array</param>
        /// <returns></returns>
        public bool Click(ref Rectangle[,] OverlayGrid, ref Cell[,] CellGrid)
        {
            isClicked = true;
            OverlayGrid[gridY, gridX].Width = 0;
            OverlayGrid[gridY, gridX].Height = 0;
            
            if (cellVal == -1)
            {
                return true;
            }
            else if (cellVal == 0)
            {
                FindAdjacentCells(ref CellGrid);
                foreach (Cell c in adjacentCells)
                {
                    if(!c.IsClicked && c.CellValue != -1)
                        c.Click(ref OverlayGrid, ref CellGrid);
                }
            }


            return false;
        }

        private void FindAdjacentCells(ref Cell[,] CellGrid)
        {
            if (gridY > 0)
            {
                if(gridX > 0)
                    adjacentCells.Add(CellGrid[gridY - 1, gridX - 1]);
                if(gridX < CellGrid.GetLength(1) - 1)
                    adjacentCells.Add(CellGrid[gridY - 1, gridX + 1]);
                adjacentCells.Add(CellGrid[gridY - 1, gridX]);
            }

            if (gridY < CellGrid.GetLength(0) - 1)
            {
                if (gridX > 0)
                    adjacentCells.Add(CellGrid[gridY + 1, gridX - 1]);

                if (gridX < CellGrid.GetLength(1) -1 )
                    adjacentCells.Add(CellGrid[gridY + 1, gridX + 1]);
                adjacentCells.Add(CellGrid[gridY + 1, gridX]);
            }

            if(gridX > 0)
                adjacentCells.Add(CellGrid[gridY, gridX - 1]);

            if (gridX < CellGrid.GetLength(1) - 1)
                adjacentCells.Add(CellGrid[gridY, gridX + 1]);
        }
    }
}
