using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace minesweeper
{
    class Cell
    {
        //position and size of this cell in pixels
        public Rectangle rectPos;

        //contains all adjacent cells to this cell
        List<Cell> adjacentCells;

        bool isClicked = false;
        bool drawFlag = false;
        
        //position as coordinate values; (0,0), (0,1), etc.
        int gridX, gridY;

        //value of cell in int[] grid: -1 is bomb, 0 is empty, n is number of mines adjacent
        int cellVal;

        //public getters and setters
        public bool IsClicked
        {
            get { return isClicked; }
        }

        public bool DrawFlag
        {
            get { return drawFlag; }
            set { drawFlag = value; }
        }

        public int CellValue
        {
            get { return cellVal; }
        }

        public int X
        {
            get { return gridX; }
        }

        public int Y
        {
            get { return gridY; }
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

        // adds any adjacent cells to adjacentCells list
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
