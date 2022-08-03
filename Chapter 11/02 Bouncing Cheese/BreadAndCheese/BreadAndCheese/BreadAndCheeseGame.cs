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

        // Display settings
        float displayWidth;
        float displayHeight;

        // Cheese Speed
        float cheeseWidthFactor = 0.05f;
        float cheeseTicksToCrossScreen = 200.0f;

        float cheeseX;
        float cheeseXSpeed;
        float cheeseY;
        float cheeseYSpeed;

        void scaleSprites()
        {
            cheeseRectangle.Width = (int)((displayWidth * cheeseWidthFactor) + 0.5f);

            float aspectRatio = (float)cheeseTexture.Width / cheeseTexture.Height;
            cheeseRectangle.Height = (int)((cheeseRectangle.Width / aspectRatio) + 0.5f);

            cheeseX = 0;
            cheeseY = 0;
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
            displayWidth = GraphicsDevice.Viewport.Width;
            displayHeight = GraphicsDevice.Viewport.Height;

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

            if (cheeseX + cheeseRectangle.Width >= displayWidth)
            {
                cheeseXSpeed = cheeseXSpeed * -1;
            }

            if (cheeseX <= 0)
            {
                cheeseXSpeed = cheeseXSpeed * -1;
            }

            if (cheeseY + cheeseRectangle.Height >= displayHeight)
            {
                cheeseYSpeed = cheeseYSpeed * -1;
            }

            if (cheeseY <= 0)
            {
                cheeseYSpeed = cheeseYSpeed * -1;
            }

            cheeseRectangle.X = (int)(cheeseX + 0.5f);
            cheeseRectangle.Y = (int)(cheeseY + 0.5f);

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
