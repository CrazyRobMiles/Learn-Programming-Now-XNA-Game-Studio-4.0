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

        GamePadState gamePad1;

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
            public bool Visible;
        }

        #region Game Management

        int score;
        int lives;

        void setupGame()
        {
            score = 0;
            lives = 3;
        }

        #endregion

        #region Display dimension values

        float displayWidth;
        float displayHeight;
        float overScanPercentage = 10.0f;
        float minDisplayX;
        float maxDisplayX;
        float minDisplayY;
        float maxDisplayY;

        private void setScreenSizes()
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


        #endregion

        #region Utility methods

        /// <summary>
        /// Calculates percentages
        /// </summary>
        /// <param name="percentage">the percentage to be calculated</param>
        /// <param name="inputValue">the value to be worked on</param>
        /// <returns>the resulting value</returns>
        float getPercentage(float percentage, float inputValue)
        {
            return (inputValue * percentage) / 100;
        }

        void setupSprite(
            ref GameSpriteStruct sprite,
            float widthFactor,
            float ticksToCrossScreen,
            float initialX,
            float initialY,
            bool initialVisibility)
        {
            sprite.WidthFactor = widthFactor;
            sprite.TicksToCrossScreen = ticksToCrossScreen;
            sprite.SpriteRectangle.Width = (int)((displayWidth * widthFactor) + 0.5f);
            float aspectRatio =
                (float)sprite.SpriteTexture.Width / sprite.SpriteTexture.Height;
            sprite.SpriteRectangle.Height =
                (int)((sprite.SpriteRectangle.Width / aspectRatio) + 0.5f);
            sprite.X = initialX;
            sprite.Y = initialY;
            sprite.XSpeed = displayWidth / ticksToCrossScreen;
            sprite.YSpeed = sprite.XSpeed;
            sprite.Visible = initialVisibility;
        }

        #endregion

        #region Text drawing

        SpriteFont font;

        private void loadFont()
        {
            font = Content.Load<SpriteFont>("SpriteFont1");
        }

        /// <summary>
        /// Draws text on the screen
        /// </summary>
        /// <param name="text">text to write</param>
        /// <param name="textColor">color of text</param>
        /// <param name="x">left hand edge of text</param>
        /// <param name="y">top of text</param>
        void drawText(string text, Color textColor, float x, float y)
        {
            int layer;
            Vector2 textVector = new Vector2(x, y);

            // Draw the shadow
            Color backColor = new Color(0, 0, 0, 20);
            for (layer = 0; layer < 10; layer++)
            {
                spriteBatch.DrawString(font, text, textVector, backColor);
                textVector.X++;
                textVector.Y++;
            }

            // Draw the solid part of the characters
            backColor = new Color(190, 190, 190);
            for (layer = 0; layer < 5; layer++)
            {
                spriteBatch.DrawString(font, text, textVector, backColor);
                textVector.X++;
                textVector.Y++;
            }

            // Draw the top of the characters
            spriteBatch.DrawString(font, text, textVector, textColor);
        }

        #endregion

        #region Bread code and data

        GameSpriteStruct bread;

        private void loadBreadContent()
        {
            bread.SpriteTexture = Content.Load<Texture2D>("Images/Bread");
            setupSprite(ref bread, 0.15f, 120.0f, displayWidth / 2, displayHeight / 2, true);
        }

        void updateBread()
        {
            bread.X = bread.X + (bread.XSpeed * gamePad1.ThumbSticks.Left.X);
            bread.Y = bread.Y - (bread.YSpeed * gamePad1.ThumbSticks.Left.Y);
            bread.SpriteRectangle.X = (int)bread.X;
            bread.SpriteRectangle.Y = (int)bread.Y;

            if (bread.SpriteRectangle.Intersects(cheese.SpriteRectangle))
            {
                cheese.YSpeed = cheese.YSpeed * -1;
            }
        }

        private void drawBread()
        {
            spriteBatch.Draw(bread.SpriteTexture, bread.SpriteRectangle, Color.White);
        }

        #endregion

        #region Cheese code and data

        GameSpriteStruct cheese;

        private void loadCheeseContent()
        {
            cheese.SpriteTexture = Content.Load<Texture2D>("Images/Cheese");
            setupSprite(ref cheese, 0.05f, 200.0f, 200, 100, true);
        }

        void updateCheese()
        {
            cheese.X = cheese.X + cheese.XSpeed;
            cheese.Y = cheese.Y + cheese.YSpeed;
            cheese.SpriteRectangle.X = (int)(cheese.X + 0.5f);
            cheese.SpriteRectangle.Y = (int)(cheese.Y + 0.5f);

            if (cheese.X + cheese.SpriteRectangle.Width >= maxDisplayX)
            {
                cheese.XSpeed = Math.Abs(cheese.XSpeed) * -1;
            }

            if (cheese.X <= minDisplayX)
            {
                cheese.XSpeed = Math.Abs(cheese.XSpeed);
            }

            if (cheese.Y + cheese.SpriteRectangle.Height >= maxDisplayY)
            {
                cheese.YSpeed = Math.Abs(cheese.YSpeed) * -1;
                if (lives > 0)
                {
                    lives--;
                }
            }

            if (cheese.Y <= minDisplayY)
            {
                cheese.YSpeed = Math.Abs(cheese.YSpeed);
            }
        }

        private void drawCheese()
        {
            spriteBatch.Draw(cheese.SpriteTexture, cheese.SpriteRectangle, Color.White);
        }

        #endregion

        #region Tomato code and data

        Texture2D tomatoTexture;
        GameSpriteStruct[] tomatoes;
        int numberOfTomatoes = 20;
        float tomatoHeight;
        float tomatoStepFactor = 0.1f;
        float tomatoHeightLimit;

        private void loadTomatoContent()
        {
            tomatoTexture = Content.Load<Texture2D>("Images/Tomato");
            tomatoHeight = minDisplayY;
            tomatoHeightLimit = minDisplayY + ((maxDisplayY - minDisplayY) / 2);
            tomatoes = new GameSpriteStruct[numberOfTomatoes];

            float tomatoSpacing = (maxDisplayX - minDisplayX) / numberOfTomatoes;

            for (int i = 0; i < numberOfTomatoes; i++)
            {
                tomatoes[i].SpriteTexture = tomatoTexture;
                setupSprite(
                    ref tomatoes[i],
                    0.05f,  // 20 tomatos across the screen
                    1000,   // 1000 ticks to move across the screen
                    minDisplayX + (i * tomatoSpacing), minDisplayY,
                    true  // initially visible
                    );
            }
        }

        void resetTomatoDisplay()
        {
            tomatoHeight = tomatoHeight + (displayHeight * tomatoStepFactor);

            if (tomatoHeight > tomatoHeightLimit)
            {
                tomatoHeight = minDisplayY;
            }

            for (int i = 0; i < numberOfTomatoes; i++)
            {
                tomatoes[i].Visible = true;
                tomatoes[i].Y = tomatoHeight;
            }
        }

        void updateTomatoes()
        {
            bool noTomatoes = true;

            for (int i = 0; i < numberOfTomatoes; i++)
            {
                if (tomatoes[i].Visible)
                {
                    noTomatoes = false;
                    if (cheese.SpriteRectangle.Intersects(tomatoes[i].SpriteRectangle))
                    {
                        cheese.YSpeed = cheese.YSpeed * -1;
                        score = score + 10;
                        tomatoes[i].Visible = false;
                        break;
                    }
                }

                tomatoes[i].SpriteRectangle.X = (int)tomatoes[i].X;
                tomatoes[i].SpriteRectangle.Y = (int)tomatoes[i].Y;
            }

            if (noTomatoes)
            {
                resetTomatoDisplay();
            }
        }

        void drawTomatoes()
        {

            for (int i = 0; i < numberOfTomatoes; i++)
            {
                if (tomatoes[i].Visible)
                {
                    spriteBatch.Draw(tomatoes[i].SpriteTexture, tomatoes[i].SpriteRectangle, Color.White);
                }
            }

        }

        #endregion

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
            setScreenSizes();

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

            loadCheeseContent();
            loadBreadContent();
            loadTomatoContent();
            loadFont();
            setupGame();
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
            gamePad1 = GamePad.GetState(PlayerIndex.One);
            // Allows the game to exit
            if (gamePad1.Buttons.Back == ButtonState.Pressed)
                this.Exit();

            updateCheese();

            if (lives == 0)
            {
                return;
            }

            updateBread();

            updateTomatoes();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            drawCheese();

            drawBread();

            drawTomatoes();

            drawText(
                "Score : " + score.ToString() + " Lives : " + lives.ToString(),
                Color.White,
                minDisplayX,
                maxDisplayY - 50);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
