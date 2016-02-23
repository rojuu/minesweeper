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

        Texture2D gridCell;
        Color[] cellColor;
        
        int gridSize = 9;
        int cellCize = 24;

        int mineSpacing = 6;

        Rectangle[,] rectGrid;

        int[,] grid;

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
            mineSpacing = 6;
            this.IsMouseVisible = true;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            device = graphics.GraphicsDevice;

            screenWidth = device.PresentationParameters.BackBufferWidth;
            screenHeight = device.PresentationParameters.BackBufferHeight;

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
            for (int i = 0; i < grid.GetLength(1); i++)
            {
                for (int j = 0; j < grid.GetLength(0); j++)
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
            for (int i = 0; i < grid.GetLength(1); i++)
            {
                for (int j = 0; j < grid.GetLength(0); j++)
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

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
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
                        spriteBatch.Draw(gridCell, rectGrid[i, j], Color.Red); //spriteBatch.Draw(gridCell, new Rectangle(i*cellCize, j*cellCize, cellCize, cellCize), Color.Red);
                    }
                    else
                        spriteBatch.Draw(gridCell, rectGrid[i, j], Color.Gray); //spriteBatch.Draw(gridCell, new Rectangle(i * cellCize, j * cellCize, cellCize, cellCize), Color.Gray);
                }
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
