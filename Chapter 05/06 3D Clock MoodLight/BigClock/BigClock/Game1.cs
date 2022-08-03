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

namespace BigClock
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Game World
        SpriteFont font;

        Texture2D imageTexture;
        Rectangle imageRect;

        byte redIntensity = 0;
        bool redCountingUp = true;
        byte redUpdateLimit = 3;
        byte redUpdateCount = 0;

        byte greenIntensity = 80;
        bool greenCountingUp = false;
        byte greenUpdateLimit = 5;
        byte greenUpdateCount = 0;

        byte blueIntensity = 160;
        bool blueCountingUp = true;
        byte blueUpdateLimit = 7;
        byte blueUpdateCount = 0;

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
            imageRect = new Rectangle(
                0,    // X position of top left corner
                0,    // Y position of top left corner
                GraphicsDevice.Viewport.Width,   // rectangle width
                GraphicsDevice.Viewport.Height); // rectangle height

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

            font = Content.Load<SpriteFont>("SpriteFont1");
            imageTexture = this.Content.Load<Texture2D>("MoodSquares");
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

            // Update each color in turn
            redUpdateCount++;
            if (redUpdateCount == redUpdateLimit)
            {
                if (redCountingUp) redIntensity++; else redIntensity--;
                if (redIntensity == 255) redCountingUp = false;
                if (redIntensity == 0) redCountingUp = true;
                redUpdateCount = 0;
            }

            greenUpdateCount++;
            if (greenUpdateCount == greenUpdateLimit)
            {
                if (greenCountingUp) greenIntensity++; else greenIntensity--;
                if (greenIntensity == 255) greenCountingUp = false;
                if (greenIntensity == 0) greenCountingUp = true;
                greenUpdateCount = 0;
            }

            blueUpdateCount++;
            if (blueUpdateCount == blueUpdateLimit)
            {
                if (blueCountingUp) blueIntensity++; else blueIntensity--;
                if (blueIntensity == 255) blueCountingUp = false;
                if (blueIntensity == 0) blueCountingUp = true;
                blueUpdateCount = 0;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {

            DateTime nowDateTime = DateTime.Now;
            string nowString = nowDateTime.ToLongTimeString();
            Vector2 nowVector = new Vector2(50, 320);
            int layer;

            spriteBatch.Begin();

            Color backgroundColor;
            backgroundColor = new Color(redIntensity, greenIntensity, blueIntensity);


            // Draw the image at the back
            spriteBatch.Draw(imageTexture, imageRect, backgroundColor);

            // Draw the shadow
            Color nowColor = new Color(0, 0, 0, 20);
            for (layer = 0; layer < 10; layer++)
            {
                spriteBatch.DrawString(font, nowString, nowVector, nowColor);
                nowVector.X++;
                nowVector.Y++;
            }

            // Draw the solid part of the characters
            nowColor = Color.Gray;
            for (layer = 0; layer < 5; layer++)
            {
                spriteBatch.DrawString(font, nowString, nowVector, nowColor);
                nowVector.X++;
                nowVector.Y++;
            }

            // Draw the top of the characters
            spriteBatch.DrawString(font, nowString, nowVector, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
