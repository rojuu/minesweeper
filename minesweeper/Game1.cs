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

        //setting variables to store input states
        KeyboardState keyboardState;
        MouseState mouseState;
        
        //textures for bomb and grid images
        Texture2D emptyCell;
        Texture2D bombCell;

        //store mouse pressed state
        bool mousePressed = false;

        //how big the grid is (gridSize^2) and how many pixels wide each sell is
        int gridSize = 9;
        int cellCize = 24; 

        //used to spread out random generation of mines
        int mineSpacing = 6;

        //stores each rectangle position of the cells in the grid. so rectGrid[0,0] would match the rectpos of grid[0,0]
        Rectangle[,] rectGrid;

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
            graphics.PreferredBackBufferHeight = cellCize * gridSize;
            graphics.PreferredBackBufferWidth = cellCize * gridSize;

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

            emptyCell = Content.Load<Texture2D>("emptyCell");
            bombCell = Content.Load<Texture2D>("bombCell");
            
            grid = new int[gridSize, gridSize];

            rectGrid = new Rectangle[gridSize, gridSize];

            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    rectGrid[j, i] = new Rectangle(i * cellCize, j * cellCize, cellCize, cellCize);
                }
            }

            GenerateNewMineGrid();
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

                    if (m >= gridSize)
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

            if (mousePressed && Mouse.GetState().LeftButton ==  ButtonState.Released)
            {
                mousePressed = false;
                for (int i = 0; i < rectGrid.GetLength(0); i++)
                {
                    for (int j = 0; j < rectGrid.GetLength(1); j++)
                    {
                        if (rectGrid[i, j].Intersects(new Rectangle(mouseState.X, mouseState.Y, 0, 0)))
                        {
                            if (grid[i, j] == -1)
                            {
                                // TODO: hit mine logic
                            }
                            Console.WriteLine("cell value: " + grid[i, j]);
                        }
                    }
                }
            }
            
            base.Update(gameTime);
        }

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
                        spriteBatch.Draw(bombCell, rectGrid[i, j], Color.Red);
                    }
                    else
                        spriteBatch.Draw(emptyCell, rectGrid[i, j], Color.Gray);
                }
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
