
// Debug is set to true to enable the output of 
// the touch events at the end of every move

//#define Debug

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

using Microsoft.Phone.Shell;

using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;

namespace ShuffleBoard
{
    #region Screen Regions

    public class Region
    {
        public Texture2D Texture;
        public Rectangle DrawPosition;
        public Color TextureColor;

        public SpriteFont Font;
        public string[] Messages;

        public Region(Texture2D inTexture,
            Rectangle inDRawPosition,
            Color inTextureColor,
            string[] inMessages,
            SpriteFont inFont)
        {
            Texture = inTexture;
            TextureColor = inTextureColor;
            DrawPosition = inDRawPosition;
            Messages = inMessages;
            Font = inFont;
        }

        public Region(Texture2D inTexture,
            Rectangle inDRawPosition,
            Color inTextureColor,
            string inMessage,
            SpriteFont inFont)
        {
            Texture = inTexture;
            TextureColor = inTextureColor;
            DrawPosition = inDRawPosition;
            Messages = new string[] { inMessage };
            Font = inFont;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, DrawPosition, TextureColor);

            float lineCount = 0.5f;

            foreach (string line in Messages)
            {
                Vector2 textSize = Font.MeasureString(line);
                // Put the text in the middle of the region
                Vector2 textPos = new Vector2(
                    DrawPosition.X + ((DrawPosition.Width - textSize.X) / 2),
                    DrawPosition.Y + ((DrawPosition.Height - (Font.LineSpacing * (Messages.Count() + 1))) / 2) + (Font.LineSpacing * lineCount)
                );
                spriteBatch.DrawString(Font, line, textPos, Color.White);
                lineCount = lineCount + 1;
            }
        }
    }

    public class ScoreRegion : Region
    {

        public int Score;

        public ScoreRegion(Texture2D inTexture,
            Rectangle inDRawPosition,
            Color inTextureColor,
            string inMessage,
            SpriteFont inFont,
            int inScore)
            : base(inTexture, inDRawPosition, inTextureColor, inMessage, inFont)
        {
            Score = inScore;
        }

        public ScoreRegion(Texture2D inTexture,
            Rectangle inDRawPosition,
            Color inTextureColor,
            string[] inMessages,
            SpriteFont inFont,
            int inScore)
            : base(inTexture, inDRawPosition, inTextureColor, inMessages, inFont)
        {
            Score = inScore;
        }
    }

    public class ButtonRegion : Region
    {
        public ButtonRegion(Texture2D inTexture,
            Rectangle inDRawPosition,
            Color inTextureColor,
            string inMessage,
            SpriteFont inFont)
            : base(inTexture, inDRawPosition, inTextureColor, inMessage, inFont)
        {
        }

        public ButtonRegion(Texture2D inTexture,
            Rectangle inDRawPosition,
            Color inTextureColor,
            string[] inMessages,
            SpriteFont inFont)
            : base(inTexture, inDRawPosition, inTextureColor, inMessages, inFont)
        {
        }

        public bool Pressed(TouchCollection locations)
        {
            foreach (TouchLocation location in locations)
            {
                if (location.State == TouchLocationState.Pressed)
                {
                    if (DrawPosition.Contains(
                            (int)location.Position.X,
                            (int)location.Position.Y))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

    #endregion


    #region Puck

    public enum PuckState
    {
        playerDragging,
        moving,
        stopped
    }

    public class PuckSprite
    {
        public Texture2D PuckTexture;
        public Rectangle PuckRectangle;
        public Vector2 PuckPosition;
        public Vector2 PuckVelocity;
        public static float Friction = 0.9f;
        public float PuckRadius;

        private PuckState state = PuckState.stopped;

        public PuckState State
        {
            get
            {
                return state;
            }
        }

        TouchLocation lastTouch;

        ShuffleBoardGame game;

#if Debug
        List<TouchLocation> touchSequence = new List<TouchLocation>();
#endif

        public PuckSprite(Texture2D inPadTexture, Rectangle inPadRectangle, Vector2 inPosition, ShuffleBoardGame inGame)
        {
            PuckTexture = inPadTexture;
            PuckRectangle = inPadRectangle;
            PuckPosition = inPosition;
            PuckRadius = PuckRectangle.Width / 2.0f;
            game = inGame;
        }

        private void updateWhenStopped()
        {
            foreach (TouchLocation touch in game.Touches)
            {
                if (touch.State == TouchLocationState.Pressed)
                {
                    Vector2 vectorToPuck = Vector2.Subtract(PuckPosition, touch.Position);
                    if (vectorToPuck.Length() < PuckRadius)
                    {
                        if (PuckRectangle.Top > game.StartRegion.DrawPosition.Top)
                        {
                            // Only drag the puck if it is in the start region
#if Debug
                            touchSequence.Clear();
                            touchSequence.Add(touch);
#endif
                            lastTouch = touch;
                            PuckPosition = touch.Position;
                            state = PuckState.playerDragging;
                        }
                    }
                }
            }
        }

        int sameValueCount = 1;

        private void updateWhenDragging()
        {
            foreach (TouchLocation touch in game.Touches)
            {
                if (touch.Id == lastTouch.Id)
                {
#if Debug
                    touchSequence.Add(touch);
#endif
                    if (touch.State == TouchLocationState.Moved)
                    {
                        Vector2 newVelocity;
                        newVelocity = Vector2.Subtract(touch.Position, lastTouch.Position);
                        if (newVelocity.Length() == 0)
                        {
                            sameValueCount++;
                        }
                        else
                        {
                            PuckVelocity = Vector2.Divide(newVelocity, sameValueCount);
                            sameValueCount = 1;
                        }
                    }

                    lastTouch = touch;
                    PuckPosition = touch.Position;

                    if (touch.State == TouchLocationState.Released)
                    {
                        state = PuckState.moving;
                    }
                }
            }
        }

        private void updateWhenMoving()
        {
            // If we are not being touched update according to physics
            PuckPosition += PuckVelocity;

            if (PuckRectangle.Right > game.DisplayWidth)
            {
                PuckVelocity.X = -(float)Math.Abs(PuckVelocity.X);
            }

            if (PuckRectangle.Left < 0)
            {
                PuckVelocity.X = (float)Math.Abs(PuckVelocity.X);
            }

            if (PuckRectangle.Bottom > game.DisplayHeight)
            {
                PuckVelocity.Y = -(float)Math.Abs(PuckVelocity.Y);
            }

            if (PuckRectangle.Top < 0)
            {
                PuckVelocity.Y = (float)Math.Abs(PuckVelocity.Y);
            }

            PuckVelocity = Vector2.Multiply(PuckVelocity, Friction);
            if (PuckVelocity.Length() < 0.05f)
            {
                PuckVelocity.X = 0;
                PuckVelocity.Y = 0;
                game.PuckStopped();
                state = PuckState.stopped;
#if Debug
                foreach (TouchLocation t in touchSequence)
                {
                    System.Diagnostics.Debug.WriteLine ("X: " + t.Position.X.ToString() + "Y: " + t.Position.Y.ToString() + " " + t.State);
                }
#endif
            }
        }

        public void Update()
        {
            switch (state)
            {
                case PuckState.stopped:
                    updateWhenStopped();
                    break;
                case PuckState.playerDragging:
                    updateWhenDragging();
                    break;
                case PuckState.moving:
                    updateWhenMoving();
                    break;
            }

            // Position the drawing rectangle around the centre position

            PuckRectangle.X = (int)(PuckPosition.X - PuckRadius + 0.5f);
            PuckRectangle.Y = (int)(PuckPosition.Y - PuckRadius + 0.5f);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(PuckTexture, PuckRectangle, Color.White);
        }
    }

    #endregion


    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class ShuffleBoardGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Game World

        SpriteFont font;
        Vector2 scorePos;

        // Display Details
        public int DisplayWidth;
        public int DisplayHeight;

        // Touch information for use by the disk
        public TouchCollection Touches;

        // Grey texture to overlay the display
        Texture2D greyOverlay;

        // Pause Menu Regions
        Region pauseMessage;
        ButtonRegion gamePausedMenuButton;
        ButtonRegion gamePausedResumeButton;

        // Game Main Menu Display Regions
        Region mainMenuMessage;
        ButtonRegion mainMenuNewgameButton;

        // Game Over Menu Regions
        Region gameOverMessage;
        ButtonRegion gameOverStartGameButton;
        ButtonRegion gameOverMainMenuButton;

        // Play area regions
        List<ScoreRegion> boardRegions = new List<ScoreRegion>();

        // Region for the start position
        public ButtonRegion StartRegion;

        PuckSprite puck;

        // Start position for the puck
        Vector2 puckStart;

        // Score for this game
        int GameScore;

        // Current high score
        int HighScore;

        // Game States
        enum GameState
        {
            GameMenu,
            PlayingGame,
            GamePaused,
            GameOver
        }

        // Game state variable
        GameState state = GameState.GameMenu;

        Rectangle completeScreen;

        public ShuffleBoardGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 480;
            graphics.PreferredBackBufferHeight = 800;

            this.Exiting += new EventHandler<EventArgs>(ShuffleBoardGame_Exiting);

            this.Activated += new EventHandler<EventArgs>(ShuffleBoardGame_Activated);

            this.Deactivated += new EventHandler<EventArgs>(ShuffleBoardGame_Deactivated);

            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);
        }

        void ShuffleBoardGame_Exiting(object sender, EventArgs e)
        {
            saveGame();
        }

        void ShuffleBoardGame_Deactivated(object sender, EventArgs e)
        {
            //handleIncomingCall();
        }

        void ShuffleBoardGame_Activated(object sender, EventArgs e)
        {
            //int i=1;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            DisplayWidth = GraphicsDevice.Viewport.Width;
            DisplayHeight = GraphicsDevice.Viewport.Height;

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

            Texture2D regionTexture = Content.Load<Texture2D>("Images/GreyRegion");
            greyOverlay = Content.Load<Texture2D>("Images/GreyOverlay");

            completeScreen = new Rectangle(0, 0, DisplayWidth, DisplayHeight);

            font = Content.Load<SpriteFont>("RegionFont");
            scorePos = new Vector2(0, DisplayHeight - font.LineSpacing);

            int regionHeight = DisplayHeight / 8;

            Color menuBackground = new Color(128, 128, 128);

            #region Create Playfield

            boardRegions.Add(new ScoreRegion(
                regionTexture,
                new Rectangle(0, 0, DisplayWidth, regionHeight),
                Color.Red,
                "Game Over",
                font,
                -1)
                );

            boardRegions.Add(new ScoreRegion(
                regionTexture,
                new Rectangle(0, regionHeight, DisplayWidth, regionHeight),
                Color.Green,
                "100",
                font,
                100)
                );

            boardRegions.Add(new ScoreRegion(
                regionTexture,
                new Rectangle(0, 2 * regionHeight, DisplayWidth, regionHeight),
                Color.Yellow,
                "50",
                font,
                50)
                );

            boardRegions.Add(new ScoreRegion(
                regionTexture,
                new Rectangle(0, 3 * regionHeight, DisplayWidth, regionHeight),
                Color.Orange,
                "20",
                font,
                20)
                );

            boardRegions.Add(new ScoreRegion(
                regionTexture,
                new Rectangle(0, 4 * regionHeight, DisplayWidth, regionHeight),
                Color.Purple,
                "10",
                font,
                10)
                );

            StartRegion = new ButtonRegion(
                regionTexture,
                new Rectangle(0, 5 * regionHeight, DisplayWidth, 3 * regionHeight),
                Color.White,
                new string[] { "Start", "Score: " },
                font
                );

            #endregion

            #region Create Pause Menu

            // Pause Menu Regions
            pauseMessage = new Region(
                regionTexture,
                new Rectangle(0, 0, DisplayWidth, 6 * regionHeight),
                menuBackground,
                new string[] { "Shuffle Fun", "", "Game", "Paused" },
                font);

            gamePausedMenuButton = new ButtonRegion(
                regionTexture,
                new Rectangle(0, 6 * regionHeight, DisplayWidth, regionHeight),
                Color.LightPink,
                "Main menu",
                font);

            gamePausedResumeButton = new ButtonRegion(
                regionTexture,
                new Rectangle(0, 7 * regionHeight, DisplayWidth, regionHeight),
                Color.LightGreen,
                "Resume",
                font);

            #endregion

            #region Create Main Menu

            mainMenuMessage = new Region(
                regionTexture,
                new Rectangle(0, 0, DisplayWidth, 6 * regionHeight),
                menuBackground,
                new string[] { "Shuffle Fun", "", "High: 0", "Main Menu" },
                font);

            mainMenuNewgameButton = new ButtonRegion(
                regionTexture,
                new Rectangle(0, 7 * regionHeight, DisplayWidth, regionHeight),
                Color.LightGreen,
                "New game",
                font);

            #endregion

            #region Create Game Over Menu

            gameOverMessage = new Region(
                regionTexture,
                new Rectangle(0, 0, DisplayWidth, 6 * regionHeight),
                menuBackground,
                new string[] { "Shuffle Fun", "", "Game Over", "", "Score:", "High:" },
                font);

            gameOverMainMenuButton = new ButtonRegion(
                regionTexture,
                new Rectangle(0, 6 * regionHeight, DisplayWidth, regionHeight),
                Color.LightPink,
                "Main menu",
                font);

            gameOverStartGameButton = new ButtonRegion(
                regionTexture,
                new Rectangle(0, 7 * regionHeight, DisplayWidth, regionHeight),
                Color.LightGreen,
                "New game",
                font);

            #endregion

            #region Create Puck

            Texture2D puckTexture = Content.Load<Texture2D>("Images/YellowPuck");

            Rectangle puckRectangle = new Rectangle(
                0, 0,
                DisplayWidth / 8, DisplayWidth / 8);

            puckStart = new Vector2(DisplayWidth / 2, 7 * regionHeight + regionHeight / 2);

            puck = new PuckSprite(puckTexture, puckRectangle, puckStart, this);

            #endregion

            // Load the game
            loadGame();
        }

        public void PuckStopped()
        {
            foreach (ScoreRegion r in boardRegions)
            {
                if (r.DrawPosition.Contains(
                    (int)puck.PuckPosition.X,
                    (int)puck.PuckPosition.Y))
                {
                    // This is the region containing the puck
                    if (r.Score < 0)
                    {
                        endGame();
                    }
                    else
                    {
                        GameScore += r.Score;
                    }
                }
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        private string filename = "GameStatus.";

        private void saveGame()
        {

            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream rawStream = isf.CreateFile(filename))
                {
                    StreamWriter writer = new StreamWriter(rawStream);
                    int stateInt = (int)state;
                    writer.WriteLine(stateInt.ToString());
                    writer.WriteLine(puck.PuckPosition.X);
                    writer.WriteLine(puck.PuckPosition.Y);
                    writer.WriteLine(GameScore);
                    writer.WriteLine(HighScore);
                    writer.Close();
                }
            }
        }

        private void loadGame()
        {
            using (IsolatedStorageFile isf =
                IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (isf.FileExists(filename))
                {
                    try
                    {
                        using (IsolatedStorageFileStream rawStream =
                            isf.OpenFile(filename, System.IO.FileMode.Open))
                        {
                            StreamReader reader = new StreamReader(rawStream);
                            int stateInt = int.Parse(reader.ReadLine());
                            state = (GameState)stateInt;
                            puck.PuckPosition.X = float.Parse(reader.ReadLine());
                            puck.PuckPosition.Y = float.Parse(reader.ReadLine());
                            GameScore = int.Parse(reader.ReadLine());
                            HighScore = int.Parse(reader.ReadLine());
                            reader.Close();
                        }
                    }
                    catch
                    {
                        // Select the main menu if the load fails
                        selectGameMenu();
                    }

                    // Remove the file so that it isn't used next time
                    isf.DeleteFile(filename);
                }
                else
                {
                    // Select the main menu if there is no file to load from
                    selectGameMenu();
                }
            }
        }

        private void selectGameMenu()
        {
            state = GameState.GameMenu;
        }

        private void startGame()
        {
            GameScore = 0;
            puck.PuckPosition = puckStart;
            state = GameState.PlayingGame;
        }

        private void endGame()
        {
            if (GameScore > HighScore)
            {
                HighScore = GameScore;
            }

            state = GameState.GameOver;
        }

        private void pauseGame()
        {
            state = GameState.GamePaused;
        }

        private void resumeGame()
        {
            state = GameState.PlayingGame;
        }

        private void exitGame()
        {
            Exit();
        }

        private void updatePausedGame()
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                gamePausedResumeButton.Pressed(Touches))
            {
                resumeGame();
            }

            if (gamePausedMenuButton.Pressed(Touches))
            {
                selectGameMenu();
            }
        }

        private void updatePlayingGame()
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                pauseGame();
            }

            StartRegion.Messages[1] = "Score: " + GameScore.ToString();

            if (puck.State == PuckState.stopped)
            {
                // If the puck is stopped, see if we need to resart the game
                if (puck.PuckRectangle.Top < StartRegion.DrawPosition.Top)
                {
                    // puck is not in the start region
                    // See if we are resetting the game
                    if (StartRegion.Pressed(Touches))
                    {
                        puck.PuckPosition = puckStart;
                    }
                }
            }
            puck.Update();
        }

        private void updateGameMenu()
        {
            mainMenuMessage.Messages[2] = "High: " + HighScore.ToString();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                exitGame();
            }

            if (mainMenuNewgameButton.Pressed(Touches))
            {
                startGame();
            }
        }

        private void updateGameOver()
        {
            gameOverMessage.Messages[4] = "Score: " + GameScore.ToString();
            gameOverMessage.Messages[5] = "High: " + HighScore.ToString();

            if (gameOverMainMenuButton.Pressed(Touches))
            {
                selectGameMenu();
            }

            if (gameOverStartGameButton.Pressed(Touches))
            {
                startGame();
            }
        }


        private void handleIncomingCall()
        {
            if (state == GameState.PlayingGame)
            {
                pauseGame();
            }
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            Touches = TouchPanel.GetState();

            switch (state)
            {
                case GameState.GameMenu:
                    updateGameMenu();
                    break;

                case GameState.PlayingGame:
                    updatePlayingGame();
                    break;

                case GameState.GamePaused:
                    updatePausedGame();
                    break;

                case GameState.GameOver:
                    updateGameOver();
                    break;
            }

            base.Update(gameTime);
        }

        private void boardDraw()
        {
            foreach (Region r in boardRegions)
            {
                r.Draw(spriteBatch);
            }
            StartRegion.Draw(spriteBatch);
        }

        private void puckDraw()
        {
            puck.Draw(spriteBatch);
        }


        private void drawGamePaused()
        {
            boardDraw();
            spriteBatch.Draw(greyOverlay, completeScreen, Color.White);
            gamePausedMenuButton.Draw(spriteBatch);
            gamePausedResumeButton.Draw(spriteBatch);
        }

        private void drawPlayingGame()
        {
            boardDraw();
            puckDraw();
        }

        private void drawGameMenu()
        {
            boardDraw();
            spriteBatch.Draw(greyOverlay, completeScreen, Color.White);
            mainMenuMessage.Draw(spriteBatch);
            mainMenuNewgameButton.Draw(spriteBatch);
        }


        private void drawGameOver()
        {
            boardDraw();
            spriteBatch.Draw(greyOverlay, completeScreen, Color.White);

            gameOverMessage.Draw(spriteBatch);

            gameOverStartGameButton.Draw(spriteBatch);
            gameOverMainMenuButton.Draw(spriteBatch);
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
                case GameState.GameMenu:
                    drawGameMenu();
                    break;

                case GameState.PlayingGame:
                    drawPlayingGame();
                    break;

                case GameState.GamePaused:
                    drawGamePaused();
                    break;

                case GameState.GameOver:
                    drawGameOver();
                    break;

            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
