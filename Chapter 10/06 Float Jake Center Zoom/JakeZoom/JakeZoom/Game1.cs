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

        float displayWidth;
        float displayHeight;
        float rectWidth;
        float rectHeight;
        float rectX;
        float rectY;

        float getPercentage(float percentage, float inputValue)
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

            displayWidth = GraphicsDevice.Viewport.Width;
            displayHeight = GraphicsDevice.Viewport.Height;

            displayWidth = GraphicsDevice.Viewport.Width;
            displayHeight = GraphicsDevice.Viewport.Height;

            rectWidth = jakeTexture.Width * 20;
            rectHeight = jakeTexture.Height * 20;
            rectX = ((rectWidth / 2) - (displayWidth / 2)) * -1;
            rectY = ((rectHeight / 2) - (displayHeight / 2)) * -1;
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

            if (rectWidth > displayWidth)
            {
                float widthChange = getPercentage(1, rectWidth);
                rectWidth = rectWidth - widthChange;
                rectX = rectX + (widthChange / 2);

                float heightChange = getPercentage(1, rectHeight);
                rectHeight = rectHeight - heightChange;
                rectY = rectY + (heightChange / 2);

                jakeRect.Width = (int)rectWidth;
                jakeRect.Height = (int)rectHeight;
                jakeRect.X = (int)rectX;
                jakeRect.Y = (int)rectY;
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
