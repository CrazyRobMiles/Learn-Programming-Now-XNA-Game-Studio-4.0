using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace BreadAndCheese
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class BreadAndCheeseGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Game World
        Texture2D cheeseTexture;
        Rectangle cheeseRectangle;
        float cheeseX;
        float cheeseXSpeed;
        float cheeseY;
        float cheeseYSpeed;
        float cheeseWidthFactor = 0.05f;
        float cheeseTicksToCrossScreen = 200.0f;

        // Display settings
        float displayWidth;
        float displayHeight;
        float overScanPercentage = 10.0f;
        float minDisplayX;
        float maxDisplayX;
        float minDisplayY;
        float maxDisplayY;
        float getPercentage(float percentage, float inputValue)
        {
            return (inputValue * percentage) / 100;
        }

        private void setupScreen()
        {
            displayWidth = graphics.GraphicsDevice.Viewport.Width;
            displayHeight = graphics.GraphicsDevice.Viewport.Height;
            float xOverscanMargin =
                 getPercentage(overScanPercentage, displayWidth) / 2.0f;
            float yOverscanMargin =
                 getPercentage(overScanPercentage, displayHeight) / 2.0f;

            minDisplayX = xOverscanMargin;
            minDisplayY = yOverscanMargin;

            maxDisplayX = displayWidth - xOverscanMargin;
            maxDisplayY = displayHeight - yOverscanMargin;
        }

        void scaleSprites()
        {
            cheeseRectangle.Width = (int)((displayWidth * cheeseWidthFactor) + 0.5f);
            float aspectRatio = (float)cheeseTexture.Width / cheeseTexture.Height;
            cheeseRectangle.Height = (int)((cheeseRectangle.Width / aspectRatio) + 0.5f);
            cheeseX = minDisplayX;
            cheeseY = minDisplayY;
            cheeseXSpeed = displayWidth / cheeseTicksToCrossScreen;
            cheeseYSpeed = cheeseXSpeed;
        }

        public BreadAndCheeseGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            setupScreen();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            cheeseTexture = Content.Load<Texture2D>("Images/Cheese");

            scaleSprites();

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            cheeseX = cheeseX + cheeseXSpeed;
            cheeseY = cheeseY + cheeseYSpeed;
            cheeseRectangle.X = (int)(cheeseX + 0.5f);
            cheeseRectangle.Y = (int)(cheeseY + 0.5f);

            if (cheeseX + cheeseRectangle.Width >= maxDisplayX)
            {
                cheeseXSpeed = cheeseXSpeed * -1;
            }

            if (cheeseX <= minDisplayX)
            {
                cheeseXSpeed = cheeseXSpeed * -1;
            }

            if (cheeseY + cheeseRectangle.Height >= maxDisplayY)
            {
                cheeseYSpeed = cheeseYSpeed * -1;
            }

            if (cheeseY <= minDisplayY)
            {
                cheeseYSpeed = cheeseYSpeed * -1;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            spriteBatch.Draw(cheeseTexture, cheeseRectangle, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
