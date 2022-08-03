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

namespace ShuffleBoard
{
    public class Region
    {
        public Texture2D Texture;
        public Rectangle DrawPosition;
        public Color TextureColor;

        public SpriteFont Font;
        public string Message;

        public int Score;

        public Region(Texture2D inTexture,
            Rectangle inDRawPosition,
            Color inTextureColor,
            string inMessage,
            SpriteFont inFont,
            int inScore)
        {
            Texture = inTexture;
            TextureColor = inTextureColor;
            DrawPosition = inDRawPosition;
            Message = inMessage;
            Font = inFont;
            Score = inScore;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, DrawPosition, TextureColor);

            Vector2 textSize = Font.MeasureString(Message);
            Vector2 textPos = new Vector2(
                DrawPosition.X + ((DrawPosition.Width - textSize.X) / 2),
                DrawPosition.Y + ((DrawPosition.Height - textSize.Y) / 2)
                );

            spriteBatch.DrawString(Font, Message, textPos, Color.White);
        }

    }

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
                    Vector2 vectorToPuck = PuckPosition - touch.Position;
                    if (vectorToPuck.Length() < PuckRadius)
                    {
                        lastTouch = touch;
                        PuckPosition = touch.Position;
                        state = PuckState.playerDragging;
                    }
                }
            }
        }

        private void updateWhenDragging()
        {
            foreach (TouchLocation touch in game.Touches)
            {
                if (touch.Id == lastTouch.Id)
                {
                    if (touch.State == TouchLocationState.Moved)
                    {
                        PuckVelocity = touch.Position - lastTouch.Position;
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

            PuckVelocity = PuckVelocity * Friction;
            if (PuckVelocity.Length() < 0.05f)
            {
                PuckVelocity.X = 0;
                PuckVelocity.Y = 0;
                game.PuckStopped();
                state = PuckState.stopped;
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

        // Play area regions
        List<Region> regions = new List<Region>();

        Region startRegion;

        PuckSprite playerSprite;

        // Start position for the puck
        Vector2 puckStart;

        int GameScore;

        public ShuffleBoardGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 480;
            graphics.PreferredBackBufferHeight = 800;

            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);
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

            font = Content.Load<SpriteFont>("RegionFont");
            scorePos = new Vector2(0, DisplayHeight - font.LineSpacing);

            int regionHeight = DisplayHeight / 8;

            regions.Add(new Region(
                regionTexture,
                new Rectangle(0, 0, DisplayWidth, regionHeight),
                Color.Red,
                "Lose",
                font,
                -1)
                );

            regions.Add(new Region(
                regionTexture,
                new Rectangle(0, regionHeight, DisplayWidth, regionHeight),
                Color.Green,
                "100",
                font,
                100)
                );

            regions.Add(new Region(
                regionTexture,
                new Rectangle(0, 2 * regionHeight, DisplayWidth, regionHeight),
                Color.Yellow,
                "50",
                font,
                50)
                );

            regions.Add(new Region(
                regionTexture,
                new Rectangle(0, 3 * regionHeight, DisplayWidth, regionHeight),
                Color.Orange,
                "20",
                font,
                20)
                );

            regions.Add(new Region(
                regionTexture,
                new Rectangle(0, 4 * regionHeight, DisplayWidth, regionHeight),
                Color.Purple,
                "10",
                font,
                10)
                );

            startRegion = new Region(
                regionTexture,
                new Rectangle(0, 5 * regionHeight, DisplayWidth, 3 * regionHeight),
                Color.White,
                "START",
                font,
                0);

            regions.Add(startRegion);

            Texture2D puckTexture = Content.Load<Texture2D>("Images/YellowPuck");

            Rectangle puckRectangle = new Rectangle(
                0, 0,
                DisplayWidth / 8, DisplayWidth / 8);

            puckStart = new Vector2(DisplayWidth / 2, 7 * regionHeight + regionHeight / 2);

            playerSprite = new PuckSprite(puckTexture, puckRectangle, puckStart, this);

        }

        public void PuckStopped()
        {
            foreach (Region r in regions)
            {
                if (r.DrawPosition.Contains(
                    (int)playerSprite.PuckPosition.X,
                    (int)playerSprite.PuckPosition.Y))
                {
                    // This is the region containing the puck
                    if (r.Score < 0)
                    {
                        GameScore = 0;
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

            // Update the touches value for use by the sprites
            Touches = TouchPanel.GetState();

            if (playerSprite.State == PuckState.stopped)
            {
                // If the puck is stopped, see if we need to resart the game
                if (playerSprite.PuckRectangle.Top < startRegion.DrawPosition.Top)
                {
                    // puck is not in the start region
                    // See if we are resetting the game
                    if (Touches.Count > 0)
                    {
                        if (Touches[0].State == TouchLocationState.Pressed)
                        {
                            if (startRegion.DrawPosition.Contains(
                                (int)Touches[0].Position.X,
                                (int)Touches[0].Position.Y))
                            {
                                playerSprite.PuckPosition = puckStart;
                            }
                        }
                    }
                }
            }

            playerSprite.Update();

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

            foreach (Region r in regions)
            {
                r.Draw(spriteBatch);
            }

            playerSprite.Draw(spriteBatch);

            spriteBatch.DrawString(font, GameScore.ToString(), scorePos, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
