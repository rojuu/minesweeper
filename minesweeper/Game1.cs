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

        Texture2D gridCell;
        Color[] cellColor;

        int gridSize = 9;
        int cellCize = 24;

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
            // TODO: Add your initialization logic here

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
            
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            spriteBatch.Draw(gridCell, new Rectangle(0, 0, cellCize, cellCize), Color.Blue);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
