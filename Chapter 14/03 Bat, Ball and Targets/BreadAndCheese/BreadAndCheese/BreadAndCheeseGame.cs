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

        #region Game Structures

        public struct BatSpriteStruct
        {
            private Texture2D spriteTexture;
            private Rectangle spriteRectangle;
            private float x;
            private float y;
            private float xSpeed;
            private float ySpeed;

            private float minDisplayX;
            private float maxDisplayX;
            private float minDisplayY;
            private float maxDisplayY;

            /// <summary>
            /// Loads the texture into the bat
            /// </summary>
            /// <param name="inSpriteTexture">texture to be loaded</param>
            public void LoadTexture(Texture2D inSpriteTexture)
            {
                spriteTexture = inSpriteTexture;
            }

            /// <summary>
            /// Starts the game running. Works out the size of all the objects on the 
            /// screen and puts them at their starting positions.
            /// </summary>
            /// <param name="widthFactor">Size of the game object, as a fraction of the width
            /// of the screen.</param>
            /// <param name="ticksToCrossScreen">Speed of movement, in number of 60th of a second ticks to 
            /// cross the entire screen.</param>
            /// <param name="inMinDisplayX">minimum X value</param>
            /// <param name="inMaxDisplayX">maximum X value</param>
            /// <param name="inMinDisplayY">minimum Y value</param>
            /// <param name="inMaxDisplayY">maximum Y value</param>
            /// <param name="initialX">start X position for the bat</param>
            /// <param name="initialY">start Y position for the bat</param>

            public void StartGame(
                float widthFactor,
                float ticksToCrossScreen,
                float inMinDisplayX,
                float inMaxDisplayX,
                float inMinDisplayY,
                float inMaxDisplayY,
                float initialX,
                float initialY)
            {
                minDisplayX = inMinDisplayX;
                minDisplayY = inMinDisplayY;
                maxDisplayX = inMaxDisplayX;
                maxDisplayY = inMaxDisplayY;

                float displayWidth = maxDisplayX - minDisplayX;

                spriteRectangle.Width = (int)((displayWidth * widthFactor) + 0.5f);
                float aspectRatio =
                    (float)spriteTexture.Width / spriteTexture.Height;
                spriteRectangle.Height =
                    (int)((spriteRectangle.Width / aspectRatio) + 0.5f);
                x = initialX;
                y = initialY;
                xSpeed = displayWidth / ticksToCrossScreen;
                ySpeed = xSpeed;
            }

            /// <summary>
            /// Draw the bat on the screen.
            /// </summary>
            /// <param name="spriteBatch">Spritebatch to be used to perform the draw</param>
            public void Draw(SpriteBatch spriteBatch)
            {
                spriteBatch.Draw(spriteTexture, spriteRectangle, Color.White);
            }

            /// <summary>
            /// Update the position of the bat using the gamepad to control it.
            /// </summary>
            public void Update(BreadAndCheeseGame game)
            {
                GamePadState gamePad1 = GamePad.GetState(PlayerIndex.One);

                x = x + (xSpeed * gamePad1.ThumbSticks.Left.X);
                y = y - (ySpeed * gamePad1.ThumbSticks.Left.Y);
                spriteRectangle.X = (int)x;
                spriteRectangle.Y = (int)y;
            }

            /// <summary>
            /// Check for collisions between the bat and other objects.
            /// </summary>
            /// <param name="target">Rectangle giving the position of the other object.</param>
            /// <returns>true if the bat has collided with this object</returns>
            public bool CheckCollision(Rectangle target)
            {
                return spriteRectangle.Intersects(target);
            }
        }

        public struct BallSpriteStruct
        {
            private Texture2D spriteTexture;
            private Rectangle spriteRectangle;
            private float x;
            private float y;
            private float xSpeed;
            private float ySpeed;

            private float minDisplayX;
            private float maxDisplayX;
            private float minDisplayY;
            private float maxDisplayY;

            /// <summary>
            /// Loads the texture into the ball
            /// </summary>
            /// <param name="inSpriteTexture">texture to be loaded</param>
            public void LoadTexture(Texture2D inSpriteTexture)
            {
                spriteTexture = inSpriteTexture;
            }

            /// <summary>
            /// Starts the game running. Works out the size of all the objects on the 
            /// screen and puts them at their starting positions.
            /// </summary>
            /// <param name="widthFactor">Size of the game object, as a fraction of the width
            /// of the screen.</param>
            /// <param name="ticksToCrossScreen">Speed of movement, in number of 60th of a second ticks to 
            /// cross the entire screen.</param>
            /// <param name="inMinDisplayX">minimum X value</param>
            /// <param name="inMaxDisplayX">maximum X value</param>
            /// <param name="inMinDisplayY">minimum Y value</param>
            /// <param name="inMaxDisplayY">maximum Y value</param>
            /// <param name="initialX">start X position for the ball</param>
            /// <param name="initialY">start Y position for the ball</param>

            public void StartGame(
                float widthFactor,
                float ticksToCrossScreen,
                float inMinDisplayX,
                float inMaxDisplayX,
                float inMinDisplayY,
                float inMaxDisplayY,
                float initialX,
                float initialY)
            {
                minDisplayX = inMinDisplayX;
                minDisplayY = inMinDisplayY;
                maxDisplayX = inMaxDisplayX;
                maxDisplayY = inMaxDisplayY;

                float displayWidth = maxDisplayX - minDisplayX;

                spriteRectangle.Width = (int)((displayWidth * widthFactor) + 0.5f);
                float aspectRatio =
                    (float)spriteTexture.Width / spriteTexture.Height;
                spriteRectangle.Height =
                    (int)((spriteRectangle.Width / aspectRatio) + 0.5f);
                x = initialX;
                y = initialY;
                xSpeed = displayWidth / ticksToCrossScreen;
                ySpeed = xSpeed;
            }

            /// <summary>
            /// Draw the ball on the screen.
            /// </summary>
            /// <param name="spriteBatch">Spritebatch to be used to perform the draw</param>
            public void Draw(SpriteBatch spriteBatch)
            {
                spriteBatch.Draw(spriteTexture, spriteRectangle, Color.White);
            }

            /// <summary>
            /// Update the ball position. Handle collisions with the bat.
            /// </summary>
            /// <param name="game">Game the ball is part of</param>
            public void Update(BreadAndCheeseGame game)
            {
                x = x + xSpeed;
                y = y + ySpeed;

                // Set the sprite rectangle to the new position
                spriteRectangle.X = (int)(x + 0.5f);
                spriteRectangle.Y = (int)(y + 0.5f);

                // Check to see if the ball has hit the bat
                if (game.BreadBat.CheckCollision(spriteRectangle))
                {
                    // bat has hit the ball.
                    ySpeed = ySpeed * -1;
                }

                if (x + spriteRectangle.Width >= maxDisplayX)
                {
                    // ball has hit the right hand side.
                    xSpeed = Math.Abs(xSpeed) * -1;
                }

                if (x <= minDisplayX)
                {
                    // ball has hit the left hand side.
                    xSpeed = Math.Abs(xSpeed);
                }

                // Check to see if the ball has hit the bottom
                if (y + spriteRectangle.Height >= maxDisplayY)
                {
                    // ball has hit the bottom. Lose a life.
                    ySpeed = Math.Abs(ySpeed) * -1;
                    game.LoseLife();
                }

                if (y <= minDisplayY)
                {
                    // ball has hit the top of the screen.
                    ySpeed = Math.Abs(ySpeed);
                }

                if (game.TomatoTargets.CheckCollision(spriteRectangle))
                {
                    game.UpdateScore(20);
                    ySpeed = ySpeed * -1;
                }

            }
        }

        public struct TargetRowStruct
        {
            private Texture2D targetTexture;
            private int numberOfTargets;
            private float targetWidth;
            private float targetHeight;
            private float targetStepFactor;
            private float targetHeightLimit;
            private Rectangle[] targets;
            private bool[] targetVisibility;

            private float minDisplayX;
            private float maxDisplayX;
            private float minDisplayY;
            private float maxDisplayY;
            private float displayHeight;
            private float displayWidth;

            public void LoadTexture(Texture2D inTargetTexture)
            {
                targetTexture = inTargetTexture;
            }

            public void StartGame(
                int inNumberOfTargets,
                float inTargetStepFactor,
                float inMinDisplayX,
                float inMaxDisplayX,
                float inMinDisplayY,
                float inMaxDisplayY)
            {
                numberOfTargets = inNumberOfTargets;
                targetStepFactor = inTargetStepFactor;

                minDisplayX = inMinDisplayX;
                minDisplayY = inMinDisplayY;
                maxDisplayX = inMaxDisplayX;
                maxDisplayY = inMaxDisplayY;

                displayWidth = maxDisplayX - minDisplayX;
                displayHeight = maxDisplayY - minDisplayY;

                targetWidth = displayWidth / numberOfTargets;

                float aspectRatio = (float)targetTexture.Width / targetTexture.Height;

                targetHeight = targetWidth / aspectRatio;

                targetHeightLimit = minDisplayY + ((maxDisplayY - minDisplayY) / 2);

                targets = new Rectangle[numberOfTargets];
                targetVisibility = new bool[numberOfTargets];

                for (int i = 0; i < numberOfTargets; i++)
                {
                    targets[i].Width = (int)targetWidth;
                    targets[i].Height = (int)targetHeight;

                    targets[i].Y = (int)targetHeight;
                    targets[i].X = (int)(minDisplayX + (i * targetWidth) + 0.5f);

                    targetVisibility[i] = true;
                }
            }

            void resetTargetDisplay()
            {
                targetHeight = targetHeight + (displayHeight * targetStepFactor);

                if (targetHeight > targetHeightLimit)
                {
                    targetHeight = minDisplayY;
                }

                for (int i = 0; i < numberOfTargets; i++)
                {
                    targets[i].Y = (int)targetHeight;
                    targetVisibility[i] = true;
                }
            }

            public void Update(BreadAndCheeseGame game)
            {
                for (int i = 0; i < numberOfTargets; i++)
                {
                    if (targetVisibility[i])
                    {
                        // return if we find a visible target
                        return;
                    }
                }

                // if we get here there are no visible targets

                // Move the target draw position down the screen
                targetHeight = targetHeight + (displayHeight * targetStepFactor);

                // Check to see if we have reached the limit down the display
                if (targetHeight > targetHeightLimit)
                {
                    targetHeight = minDisplayY;
                }

                // Reset all the targets
                for (int i = 0; i < numberOfTargets; i++)
                {
                    targets[i].Y = (int)targetHeight;
                    targetVisibility[i] = true;
                }
            }

            public void Draw(SpriteBatch spriteBatch)
            {
                for (int i = 0; i < numberOfTargets; i++)
                {
                    if (targetVisibility[i])
                    {
                        spriteBatch.Draw(targetTexture, targets[i], Color.White);
                    }
                }
            }

            public bool CheckCollision(Rectangle target)
            {
                for (int i = 0; i < numberOfTargets; i++)
                {
                    if (targetVisibility[i])
                    {
                        // Got a target to collide with
                        if (targets[i].Intersects(target))
                        {
                            // Destroy the tomato
                            targetVisibility[i] = false;
                            // return that we have collided
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        #endregion

        // The Game World
        public GamePadState GamePad1;

        public BatSpriteStruct BreadBat;
        public BallSpriteStruct CheeseBall;
        public TargetRowStruct TomatoTargets;

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

        #endregion

        #region Game State Management

        public void LoseLife()
        {
        }

        public void UpdateScore(int update)
        {
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

            BreadBat.LoadTexture(Content.Load<Texture2D>("Images/Bread"));
            CheeseBall.LoadTexture(Content.Load<Texture2D>("Images/Cheese"));
            TomatoTargets.LoadTexture(Content.Load<Texture2D>("Images/Tomato"));

            CheeseBall.StartGame(
                0.07f,   // a cheese takes 0.07 of the screen width
                200,     // cheese takes 200 ticks to cross the screen
                minDisplayX, maxDisplayX, minDisplayY, maxDisplayY,
                displayWidth / 4,   // a quarter across the screen
                displayHeight / 4); // a quarter down the screen

            BreadBat.StartGame(
                0.166f,   // a bread takes 0.166 of the screen width
                150,      // 150 ticks to cross the screen
                minDisplayX, maxDisplayX, minDisplayY, maxDisplayY,
                displayWidth / 2,   // start half way across the screen
                displayHeight / 2); // start half way up the screen

            TomatoTargets.StartGame(
                20,
                0.1f,
                minDisplayX,
                maxDisplayX,
                minDisplayY,
                maxDisplayY);
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
            GamePad1 = GamePad.GetState(PlayerIndex.One);

            if (GamePad1.Buttons.Back == ButtonState.Pressed)
                this.Exit();

            BreadBat.Update(this);
            CheeseBall.Update(this);
            TomatoTargets.Update(this);

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

            BreadBat.Draw(spriteBatch);
            CheeseBall.Draw(spriteBatch);
            TomatoTargets.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
