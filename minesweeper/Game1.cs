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

        //setting variables to store grid texture information
        Texture2D gridCell;
        Color[] cellColor;
        
        //how big the grid is (gridSize^2) and how many pixels wide each sell is
        int gridSize = 9;
        int cellCize = 24; 

        //used to spread out random generation of mines
        int mineSpacing = 6;

        //stores each rectangle position of the cells in the grid. so rectGrid[0,0] would match the rectpos of grid[0,0]
        Rectangle[,] rectGrid;

        //stores int values for mines in a grid. 1=mine, 0=no mine. used for game logic
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

            Color[] cellColor = new Color[cellCize * cellCize];
            for (int i = 0; i < cellColor.Length; i++)
            {
                cellColor[i] = Color.White;
            }

            gridCell = new Texture2D(device, cellCize, cellCize, false, SurfaceFormat.Color); 
            gridCell.SetData<Color>(cellColor);

            grid = new int[gridSize, gridSize];

            rectGrid = new Rectangle[gridSize, gridSize];

            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    rectGrid[j, i] = new Rectangle(i * cellCize, j * cellCize, cellCize, cellCize);
                }
            }

            // making  the grid = 0
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    grid[i, j] = 0;
                }
            }


            //grid = new int[,] { {0, 1, 0, 0, 0, 0, 0, 0, 0},
            //                    {0, 0, 0, 0, 0, 0, 0, 0, 0},
            //                    {0, 0, 0, 0, 0, 0, 0, 0, 0},
            //                    {0, 0, 1, 0, 0, 0, 0, 0, 0},
            //                    {0, 0, 0, 0, 1, 0, 0, 0, 0},
            //                    {0, 0, 0, 0, 0, 0, 0, 0, 0},
            //                    {0, 0, 0, 0, 1, 0, 0, 0, 0},
            //                    {0, 0, 0, 0, 0, 0, 0, 0, 0},
            //                    {0, 0, 0, 0, 1, 0, 0, 0, 0} };  

            Random rnd = new Random();

            int m = 0;
            int n = mineSpacing;
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {

                    if (rnd.NextDouble() < 0.5 && n >= mineSpacing)
                    {
                        grid[i, j] = 1;
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

        }

        protected override void Update(GameTime gameTime)
        {
            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                for (int i = 0; i < rectGrid.GetLength(0); i++)
                {
                    for (int j = 0; j < rectGrid.GetLength(1); j++)
                    {
                        if (rectGrid[i, j].Intersects(new Rectangle(mouseState.X, mouseState.Y, 0, 0)))
                        {
                            if (grid[i, j] == 1)
                            {
                                Console.WriteLine("Hit mine " + i + " " + j);
                            }
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
                    if (grid[i, j] == 1)
                    {
                        spriteBatch.Draw(gridCell, rectGrid[i, j], Color.Red);
                    }
                    else
                        spriteBatch.Draw(gridCell, rectGrid[i, j], Color.Gray);
                }
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
