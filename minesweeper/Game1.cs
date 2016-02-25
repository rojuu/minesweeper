using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace minesweeper
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        GraphicsDevice device;
        SpriteBatch spriteBatch;

        //setting variables to store input states
        KeyboardState keyboardState;
        MouseState mouseState;

        //textures for bomb and grid images
        Texture2D emptyCell;
        Texture2D overlayCell;
        Texture2D bombCell;

        Cell[,] cells;

        SpriteFont arial12;

        //store mouse pressed state
        bool mousePressed = false;
        
        //how big the grid is (gridSize^2) and how many pixels wide each sell is
        int gridSize = 9;
        int cellSize = 24;

        //used to spread out random generation of mines
        int mineSpacing = 6;

        //stores each rectangle position of the cells in the grid. so rectGrid[0,0] would match the rectPos of grid[0,0]
        //Rectangle[,] rectGrid;
        Rectangle[,] overlayGrid;

        //listed containing any cells that have been clicked already
        List<Rectangle> clickedCells;
        List<int[]> clickStack;

        //stores int values for mines in a grid. -1=mine, 0=no mine, n=number of mines touched. used for game logic
        int[,] grid;

        //store resolution info
        int screenWidth;
        int screenHeight;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            device = graphics.GraphicsDevice;
            graphics.IsFullScreen = false;
            
            graphics.PreferredBackBufferHeight = cellSize * gridSize;
            graphics.PreferredBackBufferWidth = cellSize * gridSize;

            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            this.IsMouseVisible = true;

            device = graphics.GraphicsDevice;

            screenWidth = device.PresentationParameters.BackBufferWidth;
            screenHeight = device.PresentationParameters.BackBufferHeight;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //make a new list with one rect outside the playfield
            clickedCells = new List<Rectangle>();
            clickedCells.Add(new Rectangle(-1, -1, 0, 0));

            clickStack = new List<int[]>();

            //load images for empty and bomb cells
            emptyCell = Content.Load<Texture2D>("emptyCell");
            overlayCell = Content.Load<Texture2D>("overlayCell");
            bombCell = Content.Load<Texture2D>("bombCell");
            
            cells = new Cell[gridSize, gridSize];
            
            arial12 = Content.Load<SpriteFont>("Arial12");

            grid = new int[gridSize, gridSize];

            GenerateNewMineGrid();

            //rectGrid = new Rectangle[gridSize, gridSize];
            overlayGrid = new Rectangle[gridSize, gridSize];

            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    //rectGrid[j, i] = new Rectangle(i * cellSize, j * cellSize, cellSize, cellSize);
                    cells[i, j] = new Cell(new Rectangle(i * cellSize, j * cellSize, cellSize, cellSize), j, i, grid[i, j]);
                    overlayGrid[i, j] = new Rectangle(i * cellSize, j * cellSize, cellSize, cellSize);
                }
            }
        }

        void GenerateNewMineGrid()
        {
            // making  the grid = 0
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    grid[i, j] = 0;
                }
            }

            Random rnd = new Random();

            int m = 0;
            int n = mineSpacing;
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    if (rnd.NextDouble() < 0.5 && n >= mineSpacing)
                    {
                        grid[i, j] = -1;
                        n = 0;
                        m++;
                    }
                    else
                    {
                        grid[i, j] = 0;
                    }
                    n++;

                    if (m >= gridSize/* *gridSize */)
                        break;
                }
            }

            CheckNumberOfMinesAdjacent();
        }

        void CheckNumberOfMinesAdjacent()
        {
            int numberOfMines = 0;

            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    if (grid[i, j] == 0)
                    {
                        //check row above for mines if there is one
                        if (i >= 1)
                        {
                            if (j >= 1)
                                if (grid[i - 1, j - 1] == -1)
                                    numberOfMines++;

                            if (grid[i - 1, j] == -1)
                                numberOfMines++;

                            if (j != grid.GetLength(1) -1)
                                if (grid[i - 1, j + 1] == -1)
                                    numberOfMines++;
                        }


                        //check adjacent cells on same row for mines
                        if (j >= 1)
                            if (grid[i, j - 1] == -1)
                                numberOfMines++;

                        if (j != grid.GetLength(1) - 1)
                            if (grid[i, j + 1] == -1)
                                numberOfMines++;


                        //check row below for mines
                        if (i != grid.GetLength(0) - 1)
                        {
                            if (j >= 1)
                                if (grid[i + 1, j - 1] == -1)
                                    numberOfMines++;

                            if (grid[i + 1, j] == -1)
                                numberOfMines++;

                            if (j != grid.GetLength(1) - 1)
                                if (grid[i + 1, j + 1] == -1)
                                    numberOfMines++;
                        }

                        grid[i, j] = numberOfMines;
                        numberOfMines = 0;
                    }
                }
            }
        }

        protected override void Update(GameTime gameTime)
        {
            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
                Exit();


            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                mousePressed = true;
            }
            
            if (mousePressed && Mouse.GetState().LeftButton == ButtonState.Released)
            {
                mousePressed = false;

                for (int i = 0; i < grid.GetLength(0); i++)
                {
                    for (int j = 0; j < grid.GetLength(1); j++)
                    {
                        if (cells[i, j].rectPos.Intersects(new Rectangle(mouseState.X, mouseState.Y, 0, 0)))
                        {
                            cells[i, j].Click(ref overlayGrid, ref cells);
                        }
                    }
                }
            }
            
            base.Update(gameTime);
        }

        //void ClickCell(Rectangle clickedCell, int i, int j)
        //{
        //    Rectangle lastCell = new Rectangle();

        //    if (grid[i, j] == -1)
        //    {
        //        // TODO: hit mine logic
        //    }

        //    Console.WriteLine(clickedCells.Count.ToString());
                
        //    foreach (Rectangle cell in clickedCells)
        //    {

        //        if (cell.X != -1)
        //        {
        //            //if you hadn't clicked the cell add it to the clicked list and click all surrounding cells
        //            if (!clickedCell.Intersects(cell))
        //            {
        //                overlayGrid[i, j].Width = 0;

        //                if (i > 1)
        //                {
        //                    clickStack.Add(new int[] { i - 1, j + 1 });
        //                    clickStack.Add(new int[] { i - 1, j });
        //                    if (j > 1)
        //                        clickStack.Add(new int[] { i - 1, j - 1 });
        //                }
        //                if (j > 1)
        //                {
        //                    clickStack.Add(new int[] { i, j - 1 });
        //                    clickStack.Add(new int[] { i + 1, j - 1 });
        //                }
        //                clickStack.Add(new int[] { i, j + 1 });
        //                clickStack.Add(new int[] { i + 1, j + 1 });
        //                clickStack.Add(new int[] { i + 1, j });

        //                lastCell = clickedCell;
        //                //stack overflow \:D/
        //                //ClickCell(clickedCell, i - 1, j - 1);
        //                //ClickCell(clickedCell, i - 1, j);
        //                //ClickCell(clickedCell, i - 1, j + 1);
        //                //ClickCell(clickedCell, i, j - 1);
        //                //ClickCell(clickedCell, i, j + 1);
        //                //ClickCell(clickedCell, i + 1, j - 1);
        //                //ClickCell(clickedCell, i + 1, j);
        //                //ClickCell(clickedCell, i + 1, j + 1);
        //            }
        //        }
        //    }

        //    if(lastCell != null)
        //        clickedCells.Add(lastCell);
        //}

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    if (grid[i, j] == -1)
                    {
                        spriteBatch.Draw(bombCell, cells[i, j].rectPos, Color.White);
                    }
                    else if (grid[i, j] == 0)
                    {
                        spriteBatch.Draw(emptyCell, cells[i, j].rectPos, Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(emptyCell, cells[i, j].rectPos, Color.White);
                        switch (grid[i, j])
                        {
                            case 1:
                                spriteBatch.DrawString(arial12, grid[i, j].ToString(), new Vector2(cells[i, j].rectPos.X + 6, cells[i, j].rectPos.Y + 4), Color.Green);
                                break;
                            case 2:
                                spriteBatch.DrawString(arial12, grid[i, j].ToString(), new Vector2(cells[i, j].rectPos.X + 6, cells[i, j].rectPos.Y + 4), Color.Blue);
                                break;
                            case 3:
                                spriteBatch.DrawString(arial12, grid[i, j].ToString(), new Vector2(cells[i, j].rectPos.X + 6, cells[i, j].rectPos.Y + 4), Color.Red);
                                break;
                            case 4:
                                spriteBatch.DrawString(arial12, grid[i, j].ToString(), new Vector2(cells[i, j].rectPos.X + 6, cells[i, j].rectPos.Y + 4), Color.Purple);
                                break;
                            case 5:
                                spriteBatch.DrawString(arial12, grid[i, j].ToString(), new Vector2(cells[i, j].rectPos.X + 6, cells[i, j].rectPos.Y + 4), Color.DarkRed);
                                break;
                            case 6:
                                spriteBatch.DrawString(arial12, grid[i, j].ToString(), new Vector2(cells[i, j].rectPos.X + 6, cells[i, j].rectPos.Y + 4), Color.HotPink);
                                break;
                            case 7:
                                spriteBatch.DrawString(arial12, grid[i, j].ToString(), new Vector2(cells[i, j].rectPos.X + 6, cells[i, j].rectPos.Y + 4), Color.LemonChiffon);
                                break;
                            case 8:
                                spriteBatch.DrawString(arial12, grid[i, j].ToString(), new Vector2(cells[i, j].rectPos.X + 6, cells[i, j].rectPos.Y + 4), Color.Tomato);
                                break;
                            default:
                                break;
                        }
                    }
                    if (overlayGrid[i, j].Width != 0 && overlayGrid[i, j].Height != 0)
                        spriteBatch.Draw(overlayCell, overlayGrid[i, j], Color.White * 0.7f);
                }
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
