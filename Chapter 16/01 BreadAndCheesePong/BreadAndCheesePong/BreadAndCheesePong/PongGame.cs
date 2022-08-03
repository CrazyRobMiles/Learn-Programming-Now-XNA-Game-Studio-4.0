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
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace BreadAndCheesePong
{

    public interface ISpriteBasedGame
    {
        void SetMessage(string message);
        GamePadState GetGamePad(PlayerIndex player);
        void StartGame();
        void EndGame();
    }

    #region Sprites

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
        /// Called when the game starts running
        /// </summary>
        public virtual void StartGame()
        {
        }

        /// <summary>
        /// Called when the game stops running
        /// </summary>
        public virtual void EndGame()
        {
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
        public virtual void Update(ISpriteBasedGame game)
        {
        }

        public BaseSprite(Texture2D inSpriteTexture, Rectangle inRectangle)
        {
            LoadTexture(inSpriteTexture);
            SetRectangle(inRectangle);
        }

    }

    public class TitleSprite : BaseSprite
    {
        /// <summary>
        /// Update behaviour for the title. The A button is tested and the game
        /// is started if it is pressed.
        /// </summary>
        /// <param name="game">Game to be controlled.</param>
        public override void Update(ISpriteBasedGame game)
        {
            if (game.GetGamePad(PlayerIndex.One).Buttons.A == ButtonState.Pressed)
            {
                game.StartGame();
            }
        }

        public TitleSprite(Texture2D inSpriteTexture, Rectangle inRectangle)
            : base(inSpriteTexture, inRectangle)
        {
        }
    }

    public class MovingSprite : BaseSprite
    {
        protected float x;
        protected float y;
        protected float xSpeed;
        protected float ySpeed;

        protected float initialX;
        protected float initialY;

        protected float minDisplayX;
        protected float maxDisplayX;

        protected float minDisplayY;
        protected float maxDisplayY;

        float displayWidth;
        float displayHeight;

        public float XPos
        {
            get
            {
                return x;
            }
        }

        public float YPos
        {
            get
            {
                return y;
            }
        }


        /// <summary>
        /// Check for collisions between the sprite and other objects.
        /// </summary>
        /// <param name="target">Rectangle giving the position of the other object.</param>
        /// <returns>true if the bread has collided with this object</returns>
        public virtual bool CheckCollision(Rectangle target)
        {
            return spriteRectangle.Intersects(target);
        }

        /// <summary>
        /// Creates a moving sprite
        /// </summary>
        /// <param name="inSpriteTexture">Texture to use for the sprite</param>
        /// <param name="widthFactor">Size of the game object, as a fraction of the width
        /// of the screen.</param>
        /// <param name="ticksToCrossScreen">Speed of movement, in number of 60th of a second ticks to 
        /// cross the entire screen.</param>
        /// <param name="inMinDisplayX">minimum XPos value</param>
        /// <param name="inMaxDisplayX">maximum XPos value</param>
        /// <param name="inMinDisplayY">minimum YPos value</param>
        /// <param name="inMaxDisplayY">maximum YPos value</param>
        /// <param name="initialX">start XPos position for the bat</param>
        /// <param name="initialY">start YPos position for the bat</param>
        public MovingSprite(
            Texture2D inSpriteTexture,
            float widthFactor,
            float ticksToCrossScreen,
            float inMinDisplayX,
            float inMaxDisplayX,
            float inMinDisplayY,
            float inMaxDisplayY,
            float inInitialX,
            float inInitialY)
            : base(inSpriteTexture, Rectangle.Empty)
        {
            minDisplayX = inMinDisplayX;
            minDisplayY = inMinDisplayY;
            maxDisplayX = inMaxDisplayX;
            maxDisplayY = inMaxDisplayY;
            initialX = inInitialX;
            initialY = inInitialY;

            displayWidth = maxDisplayX - minDisplayX;
            displayHeight = maxDisplayY - minDisplayY;

            spriteRectangle.Width = (int)((displayWidth * widthFactor) + 0.5f);
            float aspectRatio =
                (float)spriteTexture.Width / spriteTexture.Height;
            spriteRectangle.Height =
                (int)((spriteRectangle.Width / aspectRatio) + 0.5f);
            xSpeed = displayWidth / ticksToCrossScreen;
            ySpeed = xSpeed;
        }

        public override void StartGame()
        {
            x = minDisplayX + (initialX * displayWidth);
            y = minDisplayY + (initialY * displayHeight);
            spriteRectangle.X = (int)x;
            spriteRectangle.Y = (int)y;
            base.StartGame();
        }
    }

    public class BatSprite : MovingSprite
    {
        private PlayerIndex player;

        /// <summary>
        /// Update the position of the bat using the gamepad to control it.
        /// </summary>
        public override void Update(ISpriteBasedGame game)
        {
            x = x + (xSpeed * game.GetGamePad(player).ThumbSticks.Left.X);
            y = y - (ySpeed * game.GetGamePad(player).ThumbSticks.Left.Y);
            spriteRectangle.X = (int)x;
            spriteRectangle.Y = (int)y;
        }

        public BatSprite(Texture2D inSpriteTexture,
            float widthFactor, float ticksToCrossScreen,
            float inMinDisplayX, float inMaxDisplayX,
            float inMinDisplayY, float inMaxDisplayY,
            float inInitialX, float inInitialY,
            PlayerIndex inPlayer)
            : base(inSpriteTexture, widthFactor, ticksToCrossScreen,
                inMinDisplayX, inMaxDisplayX,
                inMinDisplayY, inMaxDisplayY,
            inInitialX, inInitialY)
        {
            player = inPlayer;
        }

    }

    public class BallSprite : MovingSprite
    {
        private SoundEffect BatHitSound;

        private SoundEffect RepeatHitSound;

        private SoundEffect EdgeHitSound;

        private SoundEffect LoseLifeSound;

        private BatSprite player1;
        private BatSprite player2;

        private int player1Score;
        private int player2Score;

        /// <summary>
        /// Update the ball position. Handle collisions with the bat
        /// </summary>
        /// <param name="game">Game the cheese is part of</param>
        public override void Update(ISpriteBasedGame game)
        {
            x = x + xSpeed;
            y = y + ySpeed;

            spriteRectangle.X = (int)(x + 0.5f);
            spriteRectangle.Y = (int)(y + 0.5f);

            if (player1.CheckCollision(spriteRectangle))
            {
                // player1 has hit the ball.
                BatHitSound.Play();
                xSpeed = xSpeed * -1;
            }

            if (player2.CheckCollision(spriteRectangle))
            {
                // player2 has hit the ball.
                BatHitSound.Play();
                xSpeed = xSpeed * -1;
            }

            if (x + spriteRectangle.Width >= maxDisplayX)
            {
                // ball has hit the right hand side.
                // player 1 has scored
                LoseLifeSound.Play();
                player1Score++;
                xSpeed = Math.Abs(xSpeed) * -1;
            }

            if (x <= minDisplayX)
            {
                // ball has hit the left hand side.
                // player 2 has scored
                LoseLifeSound.Play();
                player2Score++;
                xSpeed = Math.Abs(xSpeed);
            }

            if (y + spriteRectangle.Height >= maxDisplayY)
            {
                // ball has hit the bottom.
                EdgeHitSound.Play();
                ySpeed = Math.Abs(ySpeed) * -1;
            }

            if (y <= minDisplayY)
            {
                // ball has hit the top of the screen.
                EdgeHitSound.Play();
                ySpeed = Math.Abs(ySpeed);
            }
            game.SetMessage(player1Score.ToString() + " : " + player2Score.ToString());
        }

        public override void StartGame()
        {
            player1Score = 0;
            player2Score = 0;
            base.StartGame();
        }

        public BallSprite(Texture2D inSpriteTexture,
            float widthFactor, float ticksToCrossScreen,
            float inMinDisplayX, float inMaxDisplayX,
            float inMinDisplayY, float inMaxDisplayY,
            float inInitialX, float inInitialY,
            BatSprite inPlayer1,
            BatSprite inPlayer2,
            SoundEffect inBatHitSound,
            SoundEffect inRepeatHitSound,
            SoundEffect inEdgeHitSound,
            SoundEffect inLoseLifeSound)
            : base(inSpriteTexture, widthFactor, ticksToCrossScreen,
                inMinDisplayX, inMaxDisplayX,
                inMinDisplayY, inMaxDisplayY,
            inInitialX, inInitialY)
        {
            player1 = inPlayer1;
            player2 = inPlayer2;
            BatHitSound = inBatHitSound;
            RepeatHitSound = inRepeatHitSound;
            EdgeHitSound = inEdgeHitSound;
            LoseLifeSound = inLoseLifeSound;
        }
    }

    #endregion


    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class PongGame : Microsoft.Xna.Framework.Game, ISpriteBasedGame
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Game World

        public BatSprite Player1;
        public BatSprite Player2;
        public BallSprite Cheese;
        public TitleSprite Title;
        public BaseSprite Background;

        public List<BaseSprite> GameSprites = new List<BaseSprite>();

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

        string displayMessage = "";

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
            Vector2 textVector = new Vector2(x - 16, y - 16);

            // Draw the shadow
            Color backColor = new Color(0, 0, 0, 10);
            for (layer = 0; layer < 10; layer++)
            {
                spriteBatch.DrawString(font, text, textVector, backColor);
                textVector.X--;
                textVector.Y++;
            }

            // Draw the solid part of the characters
            backColor = new Color(190, 190, 190);
            for (layer = 0; layer < 5; layer++)
            {
                spriteBatch.DrawString(font, text, textVector, backColor);
                textVector.X--;
                textVector.Y++;
            }

            // Draw the top of the characters
            spriteBatch.DrawString(font, text, textVector, textColor);
        }

        public void CenterBottomMessage(string displayMessage, Color color)
        {
            Vector2 messageSize = font.MeasureString(displayMessage);
            float y = displayHeight - messageSize.Y;
            float x = (displayWidth - messageSize.X) / 2;
            drawText(displayMessage, color, x, y);

        }

        public void SetMessage(string message)
        {
            displayMessage = message;
        }

        #endregion


        #region Game and Score Management

        public enum GameState
        {
            titleScreen,
            playingGame
        }

        GameState state = GameState.titleScreen;

        public void StartGame()
        {
            foreach (BaseSprite sprite in GameSprites)
            {
                sprite.StartGame();
            }

            state = GameState.playingGame;
        }

        public GamePadState GetGamePad(PlayerIndex player)
        {
            return GamePad.GetState(player);
        }

        public void EndGame()
        {
            foreach (BaseSprite sprite in GameSprites)
            {
                sprite.EndGame();
            }

            state = GameState.titleScreen;
        }

        public GameState GetState()
        {
            return state;
        }

        #endregion

        public PongGame()
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

            Background = new BaseSprite(
                Content.Load<Texture2D>("Images/Background"),
                new Rectangle(0, 0, displayWidth, displayHeight));

            GameSprites.Add(Background);

            Title = new TitleSprite(
                Content.Load<Texture2D>("Images/Title"),
                new Rectangle(0, 0, displayWidth, displayHeight));

            Player1 = new BatSprite(
                Content.Load<Texture2D>("Images/WhiteBread"),
                0.07f,   // a bread takes 0.166 of the screen width
                150,      // 150 ticks to cross the screen
                minDisplayX, maxDisplayX, minDisplayY, maxDisplayY,
                0.25f,    // start a quarter across the screen
                0.5f,     // start half way up the screen
                PlayerIndex.One);  // Controlled by player 1

            GameSprites.Add(Player1);

            Player2 = new BatSprite(
                Content.Load<Texture2D>("Images/BrownBread"),
                0.07f,   // a bread takes 0.166 of the screen width
                150,      // 150 ticks to cross the screen
                minDisplayX, maxDisplayX, minDisplayY, maxDisplayY,
                0.75f,    // start a quarter across the screen
                0.5f,     // start half way up the screen
                PlayerIndex.Two);  // Controlled by player 2

            GameSprites.Add(Player2);

            Cheese = new BallSprite(
                Content.Load<Texture2D>("Images/Cheese"),
                0.1f,   // a cheese takes 0.07 of the screen width
                200,     // cheese takes 200 ticks to cross the screen
                minDisplayX, maxDisplayX, minDisplayY, maxDisplayY,
                0.25f,   // a quarter across the screen
                0.25f,   // a quarter down the screen
                Player1, // player
                Player2, // other player
                Content.Load<SoundEffect>("Sounds/BreadHit"),
                Content.Load<SoundEffect>("Sounds/RepeatHit"),
                Content.Load<SoundEffect>("Sounds/EdgeHit"),
                Content.Load<SoundEffect>("Sounds/LoseLife"));

            GameSprites.Add(Cheese);

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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            switch (state)
            {
                case GameState.titleScreen:
                    Title.Update(this);
                    break;
                case GameState.playingGame:
                    foreach (BaseSprite sprite in GameSprites)
                    {
                        sprite.Update(this);
                    }
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
            spriteBatch.Begin();

            switch (state)
            {
                case GameState.titleScreen:
                    Title.Draw(spriteBatch);
                    break;
                case GameState.playingGame:
                    foreach (BaseSprite sprite in GameSprites)
                    {
                        sprite.Draw(spriteBatch);
                    }
                    break;
            }

            CenterBottomMessage(displayMessage, Color.Red);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
