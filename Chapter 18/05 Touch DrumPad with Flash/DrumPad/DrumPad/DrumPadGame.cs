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

namespace DrumPad
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class DrumpadGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;


        class soundPad
        {
            public Texture2D padTexture;
            public SoundEffect padSound;
            public Rectangle padRectangle;

            int flashCount = 0;
            static Color flashColor = new Color(255, 255, 255, 128);
            static Color drawColor = Color.White;

            public soundPad(Texture2D inPadTexture, SoundEffect inPadSound, Rectangle inPadRectangle)
            {
                padTexture = inPadTexture;
                padSound = inPadSound;
                padRectangle = inPadRectangle;
            }

            public void Update(TouchCollection touches)
            {
                foreach (TouchLocation touch in touches)
                {
                    if (touch.State == TouchLocationState.Pressed)
                    {
                        if (padRectangle.Contains((int)touch.Position.X, (int)touch.Position.Y))
                        {
                            flashCount = 10;
                            padSound.Play();
                        }
                    }
                }
                if (flashCount > 0)
                {
                    flashCount = flashCount - 1;
                }
            }

            public void Draw(SpriteBatch spriteBatch)
            {
                if (flashCount > 0)
                {
                    spriteBatch.Draw(padTexture, padRectangle, flashColor);
                }
                else
                {
                    spriteBatch.Draw(padTexture, padRectangle, drawColor);
                }
            }
        }

        // Game World

        // Display Details
        int displayWidth;
        int displayHeight;

        // List of touch pads
        List<soundPad> pads = new List<soundPad>();

        public DrumpadGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 240;
            graphics.PreferredBackBufferHeight = 400;

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

            // Load our sound and textures and position them on the screen

            int halfWidth = displayWidth / 2;
            int halfHeight = displayHeight / 2;

            pads.Add(new soundPad(
                        Content.Load<Texture2D>("Images/cymbal"),
                        Content.Load<SoundEffect>("Sounds/cymbal"),
                        new Rectangle(0, 0, halfWidth, halfHeight))
                     );

            pads.Add(new soundPad(
                        Content.Load<Texture2D>("Images/top"),
                        Content.Load<SoundEffect>("Sounds/top"),
                        new Rectangle(halfWidth, 0, halfWidth, halfHeight))
                     );

            pads.Add(new soundPad(
                        Content.Load<Texture2D>("Images/snare"),
                        Content.Load<SoundEffect>("Sounds/snare"),
                        new Rectangle(0, halfHeight, halfWidth, halfHeight))
                     );

            pads.Add(new soundPad(
                        Content.Load<Texture2D>("Images/kick"),
                        Content.Load<SoundEffect>("Sounds/kick"),
                        new Rectangle(halfWidth, halfHeight, halfWidth, halfHeight))
                     );
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

            TouchCollection touches = TouchPanel.GetState();

            foreach (soundPad s in pads)
            {
                s.Update(touches);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();

            foreach (soundPad s in pads)
            {
                s.Draw(spriteBatch);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
