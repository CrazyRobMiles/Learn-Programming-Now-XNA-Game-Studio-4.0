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
            set
            {
                x = value;
                spriteRectangle.X = (int)(x + 0.5f);
            }
        }

        public float YPos
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
                spriteRectangle.Y = (int)(y + 0.5f);
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
        /// <param name="initialX">start XPos position for the bread</param>
        /// <param name="initialY">start YPos position for the bread</param>
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
        /// Update the position of the ball using the gamepad to control it.
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
        /// Update the ball position. Handle collisions with the bat.
        /// </summary>
        /// <param name="game">Game the ball is part of</param>
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
            SoundEffect inBreadHitSound,
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
            BatHitSound = inBreadHitSound;
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

        public BatSprite Player1Bat;
        public BatSprite Player2Bat;
        public BallSprite CheeseBall;
        public Texture2D TitleTexture;
        public Texture2D NotSignedInTexture;
        public Texture2D SelectingRoleTexture;
        public Texture2D WaitingForHostTexture;
        public Texture2D WaitingForPlayerTexture;
        public Rectangle ScreenRectangle;
        public BaseSprite Background;

        public List<BaseSprite> GameSprites = new List<BaseSprite>();

        NetworkSession session = null;

        PacketWriter writer = new PacketWriter();
        PacketReader reader = new PacketReader();


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
            NotSignedIn,
            SelectingRole,
            WaitingAsHost,
            WaitingAsPlayer,
            PlayingAsPlayer,
            PlayingAsHost
        }

        GameState state = GameState.titleScreen;

        public void StartGame()
        {
            foreach (BaseSprite sprite in GameSprites)
            {
                sprite.StartGame();
            }
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
            displayMessage = "";
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

            this.Components.Add(new GamerServicesComponent(this));

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

            TitleTexture = Content.Load<Texture2D>("Images/Title");
            NotSignedInTexture = Content.Load<Texture2D>("Images/Waiting For Sign In");
            SelectingRoleTexture = Content.Load<Texture2D>("Images/Selecting Role");
            WaitingForHostTexture = Content.Load<Texture2D>("Images/Waiting for Host");
            WaitingForPlayerTexture = Content.Load<Texture2D>("Images/Waiting for Player");
            ScreenRectangle = new Rectangle(0, 0, displayWidth, displayHeight);

            Player1Bat = new BatSprite(
                Content.Load<Texture2D>("Images/WhiteBread"),
                0.07f,   // a bread takes 0.166 of the screen width
                150,      // 150 ticks to cross the screen
                minDisplayX, maxDisplayX, minDisplayY, maxDisplayY,
                0.25f,    // start a quarter across the screen
                0.5f,     // start half way up the screen
                PlayerIndex.One);  // Controlled by player 1

            GameSprites.Add(Player1Bat);

            Player2Bat = new BatSprite(
                Content.Load<Texture2D>("Images/BrownBread"),
                0.07f,   // a bread takes 0.166 of the screen width
                150,      // 150 ticks to cross the screen
                minDisplayX, maxDisplayX, minDisplayY, maxDisplayY,
                0.75f,    // start a quarter across the screen
                0.5f,     // start half way up the screen
                PlayerIndex.One);  // Controlled by player 1

            GameSprites.Add(Player2Bat);

            CheeseBall = new BallSprite(
                Content.Load<Texture2D>("Images/Cheese"),
                0.1f,   // a cheese takes 0.07 of the screen width
                200,     // cheese takes 200 ticks to cross the screen
                minDisplayX, maxDisplayX, minDisplayY, maxDisplayY,
                0.25f,   // a quarter across the screen
                0.25f,   // a quarter down the screen
                Player1Bat, // player
                Player2Bat, // other player
                Content.Load<SoundEffect>("Sounds/BreadHit"),
                Content.Load<SoundEffect>("Sounds/RepeatHit"),
                Content.Load<SoundEffect>("Sounds/EdgeHit"),
                Content.Load<SoundEffect>("Sounds/LoseLife"));

            GameSprites.Add(CheeseBall);

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

            GamePadState gamePad1 = GamePad.GetState(PlayerIndex.One);

            if (this.IsActive)
            {
                switch (state)
                {
                    case GameState.titleScreen:
                        if (gamePad1.Buttons.A == ButtonState.Pressed)
                        {
                            state = GameState.NotSignedIn;
                        }
                        break;

                    case GameState.NotSignedIn:
#if ZUNE
                    // Special handling for the Zune device

                    // This state will never be reached 
                    // because the zune is always signed 
                    // in with the device profile. 
                    // So we Move on to the selecting role state 
                    // if we ever do end up here for 
                    // any reason

                    state = GameState.SelectingRole;

#else
                        if (Gamer.SignedInGamers.Count == 0)
                        {
                            if (!Guide.IsVisible)
                            {

                                Guide.ShowSignIn(1, false);
                            }
                        }
                        else
                        {
                            state = GameState.SelectingRole;
                        }
#endif
                        break;

                    case GameState.SelectingRole:

                        if (gamePad1.DPad.Left == ButtonState.Pressed)
                        {
                            // Selected Host role
                            // Create the session
                            state = GameState.WaitingAsHost;
                            session = NetworkSession.Create(
                                NetworkSessionType.SystemLink,
                                1, // only 1 local gamer
                                2  // no more than 2 players
                                );

                            session.GamerJoined += new EventHandler<GamerJoinedEventArgs>(hostSession_GamerJoined);
                            session.GamerLeft += new EventHandler<GamerLeftEventArgs>(hostSession_GamerLeft);
                        }

                        if (gamePad1.DPad.Right == ButtonState.Pressed)
                        {
                            state = GameState.WaitingAsPlayer;
                            // Selected Player role
                        }

                        break;

                    case GameState.WaitingAsHost:
                        displayMessage = "";
                        foreach (Gamer g in session.AllGamers)
                        {
                            displayMessage += g.Gamertag + "\n";
                        }
                        session.Update();
                        break;

                    case GameState.WaitingAsPlayer:
                        AvailableNetworkSessionCollection sessions =
                            NetworkSession.Find(NetworkSessionType.SystemLink, 1, null);

                        if (sessions.Count > 0)
                        {
                            AvailableNetworkSession mySession = sessions[0];
                            session = NetworkSession.Join(mySession);
                            session.GamerLeft += new EventHandler<GamerLeftEventArgs>(playerSession_GamerLeft);
                            StartGame();
                            state = GameState.PlayingAsPlayer;
                        }
                        break;

                    case GameState.PlayingAsPlayer:
                        Player2Bat.Update(this);
                        writer.Write('P');
                        writer.Write(Player2Bat.XPos);
                        writer.Write(Player2Bat.YPos);

                        LocalNetworkGamer localPlayer = session.LocalGamers[0];
                        localPlayer.SendData(writer, SendDataOptions.ReliableInOrder);

                        while (localPlayer.IsDataAvailable)
                        {
                            NetworkGamer sender;
                            localPlayer.ReceiveData(reader, out sender);
                            char messageType = reader.ReadChar();
                            if (messageType == 'H')
                            {
                                CheeseBall.XPos = reader.ReadSingle();
                                CheeseBall.YPos = reader.ReadSingle();
                                Player1Bat.XPos = reader.ReadSingle();
                                Player1Bat.YPos = reader.ReadSingle();
                                displayMessage = reader.ReadString();
                            }
                        }

                        session.Update();
                        break;

                    case GameState.PlayingAsHost:
                        CheeseBall.Update(this);
                        Player1Bat.Update(this);

                        writer.Write('H');

                        writer.Write(CheeseBall.XPos);
                        writer.Write(CheeseBall.YPos);

                        writer.Write(Player1Bat.XPos);
                        writer.Write(Player1Bat.YPos);

                        writer.Write(displayMessage);


                        LocalNetworkGamer localHost = session.LocalGamers[0];
                        localHost.SendData(writer, SendDataOptions.ReliableInOrder);

                        if (gameTime.TotalGameTime.Milliseconds == 0)
                        {
                            Console.WriteLine(gameTime.ElapsedGameTime.TotalSeconds.ToString());
                            foreach (NetworkGamer g in session.AllGamers)
                            {
                                Console.WriteLine(g.Gamertag.ToString());
                            }
                            Console.WriteLine();
                        }

                        while (localHost.IsDataAvailable)
                        {
                            NetworkGamer sender;
                            localHost.ReceiveData(reader, out sender);
                            char messageType = reader.ReadChar();
                            if (messageType == 'P')
                            {
                                Player2Bat.XPos = reader.ReadSingle();
                                Player2Bat.YPos = reader.ReadSingle();
                            }
                        }
                        session.Update();
                        break;
                }
            }

            base.Update(gameTime);
        }

        void hostSession_GamerLeft(object sender, GamerLeftEventArgs e)
        {
            session.Dispose();
            EndGame();
        }

        void hostSession_GamerJoined(object sender, GamerJoinedEventArgs e)
        {
            if (session.RemoteGamers.Count == 1)
            {
                StartGame();
                state = GameState.PlayingAsHost;
            }
        }

        void playerSession_GamerLeft(object sender, GamerLeftEventArgs e)
        {
            session.Dispose();
            EndGame();
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
                    spriteBatch.Draw(TitleTexture, ScreenRectangle, Color.White);
                    break;

                case GameState.NotSignedIn:
                    spriteBatch.Draw(NotSignedInTexture, ScreenRectangle, Color.White);
                    break;

                case GameState.SelectingRole:
                    spriteBatch.Draw(SelectingRoleTexture, ScreenRectangle, Color.White);
                    break;

                case GameState.WaitingAsHost:
                    spriteBatch.Draw(WaitingForPlayerTexture, ScreenRectangle, Color.White);
                    break;

                case GameState.WaitingAsPlayer:
                    spriteBatch.Draw(WaitingForHostTexture, ScreenRectangle, Color.White);
                    break;

                case GameState.PlayingAsHost:
                    foreach (BaseSprite sprite in GameSprites)
                    {
                        sprite.Draw(spriteBatch);
                    }
                    break;

                case GameState.PlayingAsPlayer:
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
