using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace minesweeper
{
    public class EndScreen : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        int screenWidth, screenHeight;

        bool mousePressed = false;

        public EndScreen(Game1 Game, int ScreenWidth, int ScreenHeigth)
        {
            Game.Exit();

            screenWidth = ScreenWidth;
            screenHeight = ScreenHeigth; 
            
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = false;

            graphics.PreferredBackBufferHeight = ScreenHeigth;
            graphics.PreferredBackBufferWidth = screenWidth;

            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            this.IsMouseVisible = true;
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                mousePressed = true;
            }

            if (mousePressed && Mouse.GetState().LeftButton == ButtonState.Released)
            {
                Game1 game = new Game1(this);
                game.Run();
                mousePressed = false;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Gray);

            spriteBatch.Begin();
            spriteBatch.DrawString(Content.Load<SpriteFont>("Arial12"), "LOST GAME", new Vector2(0, 0), Color.Red);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
