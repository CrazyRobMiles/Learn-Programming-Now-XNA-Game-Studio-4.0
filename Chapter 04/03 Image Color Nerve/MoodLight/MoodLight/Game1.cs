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

namespace MoodLight
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // The Game World - our color values
        byte redIntensity = 0;
        byte greenIntensity = 0;
        byte blueIntensity = 0;

        Texture2D jakeTexture;
        Rectangle jakeRect;

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

            jakeTexture = this.Content.Load<Texture2D>("jake");
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
            {
                GamePad.SetVibration(PlayerIndex.One, 0, 0);
                this.Exit();
            }

            GamePadState pad1 = GamePad.GetState(PlayerIndex.One);
            KeyboardState keys = Keyboard.GetState();

            if (keys.IsKeyDown(Keys.Escape)) Exit();

            if (pad1.Buttons.B == ButtonState.Pressed ||
                keys.IsKeyDown(Keys.R)) redIntensity++;

            if (pad1.Buttons.X == ButtonState.Pressed ||
                keys.IsKeyDown(Keys.B)) blueIntensity++;

            if (pad1.Buttons.A == ButtonState.Pressed ||
                keys.IsKeyDown(Keys.G)) greenIntensity++;

            if (pad1.Buttons.Y == ButtonState.Pressed ||
                keys.IsKeyDown(Keys.Y))
            {
                redIntensity++;
                greenIntensity++;
            }

            if (redIntensity > 220 ||
                 greenIntensity > 220 ||
                 blueIntensity > 220)
            {
                GamePad.SetVibration(PlayerIndex.One, 0, 1);
            }
            else
            {
                GamePad.SetVibration(PlayerIndex.One, 0, 0);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            Color backgroundColor;
            backgroundColor =
               new Color(redIntensity, greenIntensity, blueIntensity);

            spriteBatch.Begin();
            spriteBatch.Draw(jakeTexture, jakeRect, backgroundColor);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
