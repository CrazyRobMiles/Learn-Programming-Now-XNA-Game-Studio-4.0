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
        struct GameSpriteStruct
        {
            public Texture2D SpriteTexture;
            public Rectangle SpriteRectangle;
            public float X;
            public float Y;
            public float XSpeed;
            public float YSpeed;
            public float WidthFactor;
            public float TicksToCrossScreen;
        }

        GameSpriteStruct cheese;
        GameSpriteStruct bread;


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
            float xOverscanMargin = getPercentage(overScanPercentage, displayWidth) / 2.0f;
            float yOverscanMargin = getPercentage(overScanPercentage, displayHeight) / 2.0f;

            minDisplayX = xOverscanMargin;
            minDisplayY = yOverscanMargin;

            maxDisplayX = displayWidth - xOverscanMargin;
            maxDisplayY = displayHeight - yOverscanMargin;
        }


        void scaleSprites()
        {
            cheese.TicksToCrossScreen = 200.0f;
            cheese.WidthFactor = 0.05f;

            cheese.SpriteRectangle.Width =
               (int)((displayWidth * cheese.WidthFactor) + 0.5f);
            float aspectRatio =
                (float)cheese.SpriteTexture.Width / cheese.SpriteTexture.Height;
            cheese.SpriteRectangle.Height =
                (int)((cheese.SpriteRectangle.Width / aspectRatio) + 0.5f);
            cheese.X = minDisplayX;
            cheese.Y = minDisplayY;
            cheese.XSpeed = displayWidth / cheese.TicksToCrossScreen;
            cheese.YSpeed = cheese.XSpeed;

            bread.WidthFactor = 0.15f;
            bread.TicksToCrossScreen = 120.0f;

            bread.SpriteRectangle.Width =
               (int)((displayWidth * bread.WidthFactor) + 0.5f);
            aspectRatio =
                (float)bread.SpriteTexture.Width / bread.SpriteTexture.Height;
            bread.SpriteRectangle.Height =
                (int)((bread.SpriteRectangle.Width / aspectRatio) + 0.5f);
            bread.X = displayWidth / 2;
            bread.Y = displayHeight / 2;
            bread.XSpeed = displayWidth / bread.TicksToCrossScreen;
            bread.YSpeed = bread.XSpeed;
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

            cheese.SpriteTexture = Content.Load<Texture2D>("Images/Cheese");
            bread.SpriteTexture = Content.Load<Texture2D>("Images/Bread");

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
            GamePadState gamePad1 = GamePad.GetState(PlayerIndex.One);
            // Allows the game to exit
            if (gamePad1.Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // Move the bread

            bread.X = bread.X + (bread.XSpeed * gamePad1.ThumbSticks.Left.X);
            bread.Y = bread.Y - (bread.YSpeed * gamePad1.ThumbSticks.Left.Y);
            bread.SpriteRectangle.X = (int)bread.X;
            bread.SpriteRectangle.Y = (int)bread.Y;

            // Move the cheese

            cheese.X = cheese.X + cheese.XSpeed;
            cheese.Y = cheese.Y + cheese.YSpeed;
            cheese.SpriteRectangle.X = (int)(cheese.X + 0.5f);
            cheese.SpriteRectangle.Y = (int)(cheese.Y + 0.5f);

            if (cheese.X + cheese.SpriteRectangle.Width >= maxDisplayX)
            {
                cheese.XSpeed = cheese.XSpeed * -1;
            }

            if (cheese.X <= minDisplayX)
            {
                cheese.XSpeed = cheese.XSpeed * -1;
            }

            if (cheese.Y + cheese.SpriteRectangle.Height >= maxDisplayY)
            {
                cheese.YSpeed = cheese.YSpeed * -1;
            }

            if (cheese.Y <= minDisplayY)
            {
                cheese.YSpeed = cheese.YSpeed * -1;
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

            spriteBatch.Draw(cheese.SpriteTexture, cheese.SpriteRectangle, Color.White);
            spriteBatch.Draw(bread.SpriteTexture, bread.SpriteRectangle, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
