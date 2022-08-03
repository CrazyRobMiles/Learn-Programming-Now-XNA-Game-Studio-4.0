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

namespace JakeZoom
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // The Game World

        Texture2D jakeTexture;
        Rectangle jakeRect;

        int getPercentage(int percentage, int inputValue)
        {
            return (inputValue * percentage) / 100;
        }


        public Game1()
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
            jakeRect = new Rectangle(
                0,    // X position of top left hand corner
                0,    // Y position of top left hand corner
                6000, // rectangle width
                4500); // rectangle height

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

            jakeTexture = this.Content.Load<Texture2D>("jake");

            int displayWidth = GraphicsDevice.Viewport.Width;
            int displayHeight = GraphicsDevice.Viewport.Height;

            int scaledWidth = jakeTexture.Width * 10;
            int scaledHeight = jakeTexture.Height * 10;

            jakeRect = new Rectangle(
                -(scaledWidth / 2) + (displayWidth / 2),
                -(scaledHeight / 2) + (displayHeight / 2),
                scaledWidth, scaledHeight);
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

            if (jakeRect.Width > graphics.GraphicsDevice.Viewport.Width)
            {
                int widthChange = getPercentage(1, jakeRect.Width);
                int heightChange = getPercentage(1, jakeRect.Height);
                jakeRect.Width = jakeRect.Width - widthChange;
                jakeRect.Height = jakeRect.Height - heightChange;
                jakeRect.X = jakeRect.X + (widthChange / 2);
                jakeRect.Y = jakeRect.Y + (heightChange / 2);
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
            spriteBatch.Draw(jakeTexture, jakeRect, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
