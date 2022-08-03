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

namespace MessageBoard
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //Game World
        SpriteFont font;
        string messageString = "";

        // the keys that were pressed before – initially an empty array
        Keys[] oldKeys = new Keys[0];

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
            // TODO: Add your initialization logic here

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
            KeyboardState keyState = Keyboard.GetState();

            // Allows the game to exit by pressing the Esc key
            if (keyState.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            // the keys that are currently pressed
            Keys[] pressedKeys;
            pressedKeys = keyState.GetPressedKeys();

            // work through each key that is presently pressed
            for (int i = 0; i < pressedKeys.Length; i++)
            {
                // set a flag to indicate we have not found the key
                bool foundIt = false;

                // work through each key that was previously pressed
                for (int j = 0; j < oldKeys.Length; j++)
                {
                    if (pressedKeys[i] == oldKeys[j])
                    {
                        // we found the key in the previously pressed keys
                        foundIt = true;
                    }
                }
                if (foundIt == false)
                {
                    // if we get here we didn't find the key in the old keys, so
                    // add the key to the end of the message string
                    messageString = messageString + pressedKeys[i].ToString();
                }
            }

            // remember the currently pressed keys for next time
            oldKeys = pressedKeys;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Vector2 messageVector = new Vector2(50, 100);

            spriteBatch.Begin();
            spriteBatch.DrawString(font, messageString, messageVector, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
