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

    #region Game Sprites

    public class BaseSprite
    {
        protected Texture2D spriteTexture;
        protected Rectangle spriteRectangle;

        /// <summary>
        /// Loads the texture to be used in the sprite.
        /// </summary>
        /// <param name="inSpriteTexture">The texture to be used.</param>
        public void LoadTexture(Texture2D inSpriteTexture)
        {
            spriteTexture = inSpriteTexture;
        }

        /// <summary>
        /// Loads the rectangle to be used as the destination for 
        /// draw operations for this background sprite.
        /// </summary>
        /// <param name="inSpriteRectangle">The rectangle to be used.</param>
        public void SetRectangle(Rectangle inSpriteRectangle)
        {
            spriteRectangle = inSpriteRectangle;
        }

        /// <summary>
        /// Draws the texture using the rectangle that dimensions it.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch to be used
        /// for the drawing operation.</param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(spriteTexture, spriteRectangle, Color.White);
        }

        /// <summary>
        /// Update behaviour for the title. The base sprite does not have
        /// any update behaviour, but other sprites might.
        /// </summary>
        /// <param name="game">Game to be controlled.</param>
        public virtual void Update(BreadAndCheeseGame game)
        {
        }
    }

    public class TitleSprite : BaseSprite
    {
        /// <summary>
        /// Update behaviour for the title. The A button is tested and the game
        /// is started if it is pressed.
        /// </summary>
        /// <param name="game">Game to be controlled.</param>
        public override void Update(BreadAndCheeseGame game)
        {
            if (game.GamePad1.Buttons.A == ButtonState.Pressed)
            {
                game.StartGame();
            }
        }
    }

    public class MovingSprite : BaseSprite
    {
        protected float x;
        protected float y;
        protected float xSpeed;
        protected float ySpeed;

        protected float minDisplayX;
        protected float maxDisplayX;
        protected float minDisplayY;
        protected float maxDisplayY;

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
        /// <param name="initialX">start X position for the sprite</param>
        /// <param name="initialY">start Y position for the sprite</param>

        public virtual void StartGame(
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
        /// Check for collisions between the sprite and other objects.
        /// </summary>
        /// <param name="ballPosition">Rectangle giving the position of the other object.</param>
        /// <returns>true if the sprite has collided with this object</returns>
        public virtual bool CheckCollision(Rectangle target)
        {
            return spriteRectangle.Intersects(target);
        }
    }

    public class BatSprite : MovingSprite
    {
        /// <summary>
        /// Update the position of the bat using the gamepad to control it.
        /// </summary>
        public override void Update(BreadAndCheeseGame game)
        {
            x = x + (xSpeed * game.GamePad1.ThumbSticks.Left.X);
            y = y - (ySpeed * game.GamePad1.ThumbSticks.Left.Y);
            spriteRectangle.X = (int)x;
            spriteRectangle.Y = (int)y;
        }
    }

    public class BallSprite : MovingSprite
    {

        /// <summary>
        /// Update the ball position. Handle collisions with the bat
        /// and targets. 
        /// </summary>
        /// <param name="game">Game the ball is part of</param>
        public override void Update(BreadAndCheeseGame game)
        {
            x = x + xSpeed;
            y = y + ySpeed;

            spriteRectangle.X = (int)(x + 0.5f);
            spriteRectangle.Y = (int)(y + 0.5f);

            if (game.BreadBat.CheckCollision(spriteRectangle))
            {
                // ball has hit the bat.
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

    public class TargetRowSprite : BaseSprite
    {
        private int numberOfTargets;
        private float targetWidth;
        private float targetHeight;
        private float targetStepFactor;
        private float targetHeightLimit;
        private Rectangle[] Targets;
        private bool[] TargetVisibility;

        private float minDisplayX;
        private float maxDisplayX;
        private float minDisplayY;
        private float maxDisplayY;
        private float displayHeight;
        private float displayWidth;

        /// <summary>
        /// Starts a game. Sets up the targets and makes them ready for use.
        /// </summary>
        /// <param name="inNumberOfTargets">Number of targets across the screen.</param>
        /// <param name="inTargetStepFactor">The propotion of the screen to move the targets
        /// down when a row has been destroyed.</param>
        /// <param name="inMinDisplayX"></param>
        /// <param name="inMaxDisplayX"></param>
        /// <param name="inMinDisplayY"></param>
        /// <param name="inMaxDisplayY"></param>
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

            float aspectRatio = (float)spriteTexture.Width / spriteTexture.Height;

            targetHeight = targetWidth / aspectRatio;

            targetHeightLimit = minDisplayY + ((maxDisplayY - minDisplayY) / 2);

            Targets = new Rectangle[numberOfTargets];
            TargetVisibility = new bool[numberOfTargets];

            for (int i = 0; i < numberOfTargets; i++)
            {
                Targets[i].Width = (int)targetWidth;
                Targets[i].Height = (int)targetHeight;

                Targets[i].Y = (int)targetHeight;
                Targets[i].X = (int)(minDisplayX + (i * targetWidth) + 0.5f);

                TargetVisibility[i] = true;
            }
        }

        /// <summary>
        /// Updates the ballPosition display. If all the targets have been destroyed
        /// the row is repositioned and the targets made visible again.
        /// </summary>
        /// <param name="game">The game containing this ballPosition row. Not presently used
        /// but could be used to allow the ballPosition to speed up the bat
        /// each time a row is destroyed.</param>
        public override void Update(BreadAndCheeseGame game)
        {
            for (int i = 0; i < numberOfTargets; i++)
            {
                if (TargetVisibility[i])
                {
                    // return if we find a visible ballPosition
                    return;
                }
            }

            // if we get here there are no visible targets

            // Move the ballPosition draw position down the screen
            targetHeight = targetHeight + (displayHeight * targetStepFactor);

            // Check to see if we have reached the limit down the display
            if (targetHeight > targetHeightLimit)
            {
                targetHeight = minDisplayY;
            }

            // Reset all the targets
            for (int i = 0; i < numberOfTargets; i++)
            {
                Targets[i].Y = (int)targetHeight;
                TargetVisibility[i] = true;
            }
        }

        /// <summary>
        /// Draws all the visible targets.
        /// </summary>
        /// <param name="spriteBatch">Spritebatch to be used for drawing.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < numberOfTargets; i++)
            {
                if (TargetVisibility[i])
                {
                    spriteBatch.Draw(spriteTexture, Targets[i], Color.White);
                }
            }
        }

        /// <summary>
        /// Checks to see if the ball has collided with a ballPosition. If it has
        /// the ballPosition is destroyed.
        /// </summary>
        /// <param name="ballPosition">The position of the ball.</param>
        /// <returns>true if a collision has occured and a target has been removed.</returns>
        public bool CheckCollision(Rectangle ballPosition)
        {
            for (int i = 0; i < numberOfTargets; i++)
            {
                if (TargetVisibility[i])
                {
                    // Got a target to collide with
                    if (Targets[i].Intersects(ballPosition))
                    {
                        // Destroy the target
                        TargetVisibility[i] = false;
                        // return that we have collided
                        return true;
                    }
                }
            }
            return false;
        }
    }

    #endregion

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class BreadAndCheeseGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // The Game World
        public GamePadState GamePad1;

        public BatSprite BreadBat;
        public BallSprite CheeseBall;
        public TargetRowSprite TomatoTargets;
        public TitleSprite Title;
        public BaseSprite Background;

        #region Display dimension values

        int displayWidth;
        int displayHeight;
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
            Color backColor = new Color(0, 0, 0, 10);
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

        #region Score Display

        void drawScore()
        {
            drawText("Score : " + score + " Lives : " + lives, Color.Blue, minDisplayX, maxDisplayY - 50);
        }

        void drawHighScore()
        {
            drawText("Highscore : " + highScore + "Press A to play", Color.Blue, minDisplayX, minDisplayY);
        }

        #endregion

        #region Game and Score Management

        int score;
        int lives;
        int highScore;

        public enum GameState
        {
            titleScreen,
            playingGame
        }

        GameState state = GameState.titleScreen;

        public void StartGame()
        {
            score = 0;
            lives = 3;

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

            state = GameState.playingGame;
        }

        void gameOver()
        {
            if (score > highScore)
            {
                highScore = score;
            }
            state = GameState.titleScreen;
        }

        public void LoseLife()
        {
            lives--;
            if (lives == 0)
            {
                gameOver();
            }
        }

        public void UpdateScore(int update)
        {
            score = score + update;
        }

        public int GetScore()
        {
            return score;
        }

        public int GetLives()
        {
            return lives;
        }

        public GameState GetState()
        {
            return state;
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

            BreadBat = new BatSprite();
            CheeseBall = new BallSprite();
            TomatoTargets = new TargetRowSprite();
            Title = new TitleSprite();
            Background = new BaseSprite();

            Title.SetRectangle(
                new Rectangle(
                    (int)minDisplayX, (int)minDisplayY,
                    (int)(maxDisplayX - minDisplayX),
                    (int)(maxDisplayY - minDisplayY)
                 ));

            Background.SetRectangle(
                new Rectangle(
                    (int)minDisplayX, (int)minDisplayY,
                    (int)(maxDisplayX - minDisplayX),
                    (int)(maxDisplayY - minDisplayY)
                 ));

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
            Title.LoadTexture(Content.Load<Texture2D>("Images/Title"));
            Background.LoadTexture(Content.Load<Texture2D>("Images/Background"));
            loadFont();
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

            switch (state)
            {
                case GameState.titleScreen:
                    Title.Update(this);
                    break;
                case GameState.playingGame:
                    BreadBat.Update(this);
                    CheeseBall.Update(this);
                    TomatoTargets.Update(this);
                    break;
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

            switch (state)
            {
                case GameState.titleScreen:
                    Title.Draw(spriteBatch);
                    drawHighScore();
                    break;
                case GameState.playingGame:
                    Background.Draw(spriteBatch);
                    BreadBat.Draw(spriteBatch);
                    CheeseBall.Draw(spriteBatch);
                    TomatoTargets.Draw(spriteBatch);
                    drawScore();
                    break;
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
