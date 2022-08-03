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

namespace PanicButton
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class PanicButtonGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Game Data

        int displayWidth;
        int displayHeight;

        Texture2D buttonTexture;
        Rectangle buttonRectangle;
        SoundEffect panicSound;

        public PanicButtonGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.SupportedOrientations =
                DisplayOrientation.Default |
                DisplayOrientation.LandscapeLeft |
                DisplayOrientation.LandscapeRight |
                DisplayOrientation.Portrait;

            Window.OrientationChanged += new EventHandler<EventArgs>(Window_OrientationChanged);

            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);
        }

        void Window_OrientationChanged(object sender, EventArgs e)
        {
            positionButton();
        }

        private void positionButton()
        {
            displayWidth = GraphicsDevice.Viewport.Width;
            displayHeight = GraphicsDevice.Viewport.Height;

            if (displayWidth > displayHeight)
            {
                // landscape mode
                buttonRectangle = new Rectangle(
                    (displayWidth - displayHeight) / 2, 0,
                    displayHeight, displayHeight);
            }
            else
            {
                // portrait mode
                buttonRectangle = new Rectangle(
                    0, (displayHeight - displayWidth) / 2,
                    displayWidth, displayWidth);
            }

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
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

            buttonTexture = Content.Load<Texture2D>("PanicButton");
            panicSound = Content.Load<SoundEffect>("PanicSound");

            positionButton();

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

            if (touches.Count() > 0)
            {
                TouchLocation touch = touches[0];
                if (touch.State == TouchLocationState.Pressed)
                {
                    if (buttonRectangle.Contains((int)touch.Position.X, (int)touch.Position.Y))
                    {
                        panicSound.Play();
                    }
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            spriteBatch.Draw(buttonTexture, buttonRectangle, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
