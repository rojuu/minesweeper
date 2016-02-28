using System;
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

        KeyboardState keyboardState;
        MouseState mouseState;

        Texture2D emptyCell;
        Texture2D overlayCell;
        Texture2D bombCell;
        Texture2D lostScreen;
        Texture2D wonScreen;
        Texture2D flag;

        //used to draw number of cells adjacent to an empty cell
        SpriteFont arial12;

        //all cells in play grid
        Cell[,] cells;

        //positions for overlay images (covers any bombs or other information under it)
        Rectangle[,] overlayGrid;

        //stores int values for mines in a grid. -1=mine, 0=no mine, n=number of mines adjacent. used for game logic
        int[,] grid;

        bool mousePressed = false;
        bool mouse2Pressed = false;
        bool wonGame = false;
        bool hitBomb = false;
        
        //how big the grid is (gridSize^2) and how many pixels wide each sell is
        int gridSize = 12;
        int cellSize = 24;

        //difficulty determines the number of mines (gridSize*difficulty)
        int difficulty = 2;
        
        //store resolution info
        int screenWidth;
        int screenHeight;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            device = graphics.GraphicsDevice;
            graphics.IsFullScreen = false;
            
            //set resolution based on the grid size
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
            
            //load all textures
            emptyCell = Content.Load<Texture2D>("emptyCell");
            overlayCell = Content.Load<Texture2D>("overlayCell");
            bombCell = Content.Load<Texture2D>("bombCell");
            lostScreen = Content.Load<Texture2D>("lostScreen");
            wonScreen = Content.Load<Texture2D>("wonScreen");
            flag = Content.Load<Texture2D>("flag");

            arial12 = Content.Load<SpriteFont>("Arial12");

            cells = new Cell[gridSize, gridSize];
            overlayGrid = new Rectangle[gridSize, gridSize];
            grid = new int[gridSize, gridSize];

            //generates new mines into the grid
            GenerateNewMineGrid();


            //init overaly and cell grid. cell grid takes the cell position and cell 
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    cells[i, j] = new Cell(new Rectangle(i * cellSize, j * cellSize, cellSize, cellSize), j, i, grid[i, j]);
                    overlayGrid[i, j] = new Rectangle(i * cellSize, j * cellSize, cellSize, cellSize);
                }
            }
        }

        //generates new mines into the grid
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

            int numberOfMines = gridSize * difficulty;

            for (int i = 0; i < numberOfMines; i++)
            {
                if (grid[rnd.Next(0, grid.GetLength(0)), rnd.Next(0, grid.GetLength(1))] != -1)
                {
                    grid[rnd.Next(0, grid.GetLength(0)), rnd.Next(0, grid.GetLength(1))] = -1;
                }
                else
                    i--;
            }

            CheckNumberOfMinesAdjacent();
        }

        //changes empty values in grid to represent number of mines adjacent
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
            else if (mouseState.RightButton == ButtonState.Pressed)
            {
                mouse2Pressed = true;
            }
            
            if (mousePressed && Mouse.GetState().LeftButton == ButtonState.Released)
            {
                mousePressed = false;

                if (!hitBomb && !wonGame)
                {
                    for (int i = 0; i < grid.GetLength(0); i++)
                    {
                        for (int j = 0; j < grid.GetLength(1); j++)
                        {
                            if (cells[i, j].rectPos.Intersects(new Rectangle(mouseState.X, mouseState.Y, 0, 0)))
                            {
                                hitBomb = cells[i, j].Click(ref overlayGrid, ref cells);
                            }
                        }
                    } 
                }
                //if game has ended in win or lose and we click the mouse, reset game
                else if (hitBomb)
                {
                    //reset game resets any variables used in the game and generates new mine positions
                    ResetGame();
                }
                else if (wonGame)
                {
                    ResetGame();
                }

                //if all non-bomb cells have been clicked on, set game wonGame to true
                foreach (Cell c in cells)
                {
                    if (c.CellValue != -1)
                    {
                        if (!c.IsClicked)
                        {
                            wonGame = false;
                            break;
                        }
                        wonGame = true;
                    }
                }
            }

            //pressing mouse2 places a flag on clicked cell, if it hasn't been clicked on
            if (mouse2Pressed && Mouse.GetState().RightButton == ButtonState.Released)
            {
                mouse2Pressed = false;
                for (int i = 0; i < grid.GetLength(0); i++)
                {
                    for (int j = 0; j < grid.GetLength(1); j++)
                    {
                        if (cells[i, j].rectPos.Intersects(new Rectangle(mouseState.X, mouseState.Y, 0, 0)))
                        {
                            if (!cells[i, j].IsClicked)
                            {
                                cells[i, j].DrawFlag = !cells[i, j].DrawFlag;
                            }
                        }
                    }
                } 
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DimGray);

            spriteBatch.Begin();

            //loop through all cells in the grid
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    //draw bombs
                    if (grid[i, j] == -1 && !cells[i, j].IsClicked)
                    {
                        spriteBatch.Draw(bombCell, cells[i, j].rectPos, Color.White);
                    }
                    //if bomb has been clicked, draw it as red
                    else if (grid[i, j] == -1 && cells[i, j].IsClicked)
                    {
                        spriteBatch.Draw(bombCell, cells[i, j].rectPos, Color.Red);
                    }
                    //draw empty cells
                    else if (grid[i, j] == 0)
                    {
                        spriteBatch.Draw(emptyCell, cells[i, j].rectPos, Color.White);
                    }
                    else
                    {
                        //draw number of bombs adjacent on top of empty cells, if needed
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
                    //if we haven't set overlaygrid width or height to 0, draw overlaygrid cell
                    if (overlayGrid[i, j].Width != 0 && overlayGrid[i, j].Height != 0 && !hitBomb)
                    {
                        spriteBatch.Draw(overlayCell, overlayGrid[i, j], Color.White);
                        //if this cell has drawflag as true, draw flag 
                        if (cells[i, j].DrawFlag)
                        {
                            spriteBatch.Draw(flag, overlayGrid[i, j], Color.White);
                        }
                    }
                }
            }

            //wongame / lostgame screens
            if (wonGame)
            {
                spriteBatch.Draw(wonScreen, GraphicsDevice.Viewport.TitleSafeArea, Color.Gray * 0.7f);
            }
            else if (hitBomb)
            {
                spriteBatch.Draw(lostScreen, GraphicsDevice.Viewport.TitleSafeArea, Color.Gray * 0.7f);
            }
            
            spriteBatch.End();

            base.Draw(gameTime);
        }

        //reset game resets any variables used in the game and generates new mine positions
        void ResetGame()
        {
            mousePressed = false;
            mouse2Pressed = false;
            hitBomb = false;
            wonGame = false;
                        
            cells = new Cell[gridSize, gridSize];
            
            grid = new int[gridSize, gridSize];

            GenerateNewMineGrid();

            overlayGrid = new Rectangle[gridSize, gridSize];

            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    cells[i, j] = new Cell(new Rectangle(i * cellSize, j * cellSize, cellSize, cellSize), j, i, grid[i, j]);
                    overlayGrid[i, j] = new Rectangle(i * cellSize, j * cellSize, cellSize, cellSize);
                }
            }
        }
    }
}
