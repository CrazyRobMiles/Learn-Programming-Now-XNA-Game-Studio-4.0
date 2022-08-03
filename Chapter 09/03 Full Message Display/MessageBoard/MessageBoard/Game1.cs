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

        // Game World

        // font for message display
        SpriteFont font;

        // background texture and rectangle
        Texture2D gameTexture;
        Rectangle spriteRect;

        // message to be displayed
        // Initially an empty string
        string messageString = "";

        // the keys that were pressed before – initially an empty array
        Keys[] oldKeys = new Keys[0];

        // Color animation values
        byte redIntensity = 0;
        bool redCountingUp = true;
        byte redUpdateLimit = 3;
        byte redUpdateCount = 0;

        byte greenIntensity = 0;
        bool greenCountingUp = true;
        byte greenUpdateLimit = 5;
        byte greenUpdateCount = 0;

        byte blueIntensity = 0;
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
            spriteRect = new Rectangle(
                0,    // X position of top left hand corner
                0,    // Y position of top left hand corner
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
            gameTexture = Content.Load<Texture2D>("MoodSquares");
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

                // work through each key in the old keys
                for (int j = 0; j < oldKeys.Length; j++)
                {
                    if (pressedKeys[i] == oldKeys[j])
                    {
                        // we found the key in the old keys
                        foundIt = true;
                    }
                }
                if (foundIt == false)
                {
                    // if we get here we didn't find the key in the old keys
                    // now decode the key value for use in the message
                    string keyString = ""; // initially this is an empty string
                    switch (pressedKeys[i])
                    {
                        case Keys.D0:
                            keyString = "0";
                            break;
                        case Keys.D1:
                            keyString = "1";
                            break;
                        case Keys.D2:
                            keyString = "2";
                            break;
                        case Keys.D3:
                            keyString = "3";
                            break;
                        case Keys.D4:
                            keyString = "4";
                            break;
                        case Keys.D5:
                            keyString = "5";
                            break;
                        case Keys.D6:
                            keyString = "6";
                            break;
                        case Keys.D7:
                            keyString = "7";
                            break;
                        case Keys.D8:
                            keyString = "8";
                            break;
                        case Keys.D9:
                            keyString = "9";
                            break;
                        case Keys.A:
                            keyString = "A";
                            break;
                        case Keys.B:
                            keyString = "B";
                            break;
                        case Keys.C:
                            keyString = "C";
                            break;
                        case Keys.D:
                            keyString = "D";
                            break;
                        case Keys.E:
                            keyString = "E";
                            break;
                        case Keys.F:
                            keyString = "F";
                            break;
                        case Keys.G:
                            keyString = "G";
                            break;
                        case Keys.H:
                            keyString = "H";
                            break;
                        case Keys.I:
                            keyString = "I";
                            break;
                        case Keys.J:
                            keyString = "J";
                            break;
                        case Keys.K:
                            keyString = "K";
                            break;
                        case Keys.L:
                            keyString = "L";
                            break;
                        case Keys.M:
                            keyString = "M";
                            break;
                        case Keys.N:
                            keyString = "N";
                            break;
                        case Keys.O:
                            keyString = "O";
                            break;
                        case Keys.P:
                            keyString = "P";
                            break;
                        case Keys.Q:
                            keyString = "Q";
                            break;
                        case Keys.R:
                            keyString = "R";
                            break;
                        case Keys.S:
                            keyString = "S";
                            break;
                        case Keys.T:
                            keyString = "T";
                            break;
                        case Keys.U:
                            keyString = "U";
                            break;
                        case Keys.W:
                            keyString = "W";
                            break;
                        case Keys.V:
                            keyString = "V";
                            break;
                        case Keys.X:
                            keyString = "X";
                            break;
                        case Keys.Y:
                            keyString = "Y";
                            break;
                        case Keys.Z:
                            keyString = "Z";
                            break;
                        case Keys.Space:
                            keyString = " ";
                            break;
                        case Keys.OemPeriod:
                            keyString = ".";
                            break;
                        case Keys.Enter:
                            keyString = "\n";
                            break;

                    }

                    if (keyState.IsKeyUp(Keys.LeftShift) && keyState.IsKeyUp(Keys.RightShift))
                    {
                        keyString = keyString.ToLower();
                    }

                    if (pressedKeys[i] == Keys.Back)
                    {
                        if (messageString.Length > 0)
                        {
                            messageString = messageString.Remove(messageString.Length - 1, 1);
                        }
                    }

                    messageString = messageString + keyString;
                }
            }

            // remember the keys for next time
            oldKeys = pressedKeys;

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
            Color backgroundColor;
            backgroundColor = new Color(redIntensity, greenIntensity, blueIntensity);

            string displayString = messageString + "\n" + DateTime.Now.ToLongTimeString();
            Vector2 nowVector = new Vector2(50, 50);
            int layer;

            spriteBatch.Begin();

            spriteBatch.Draw(gameTexture, spriteRect, backgroundColor);

            // Draw the shadow
            Color nowColor = new Color(0, 0, 0, 20);
            for (layer = 0; layer < 10; layer++)
            {
                spriteBatch.DrawString(font, displayString, nowVector, nowColor);
                nowVector.X++;
                nowVector.Y++;
            }

            // Draw the solid part of the characters
            nowColor = new Color(190, 190, 190);
            for (layer = 0; layer < 5; layer++)
            {
                spriteBatch.DrawString(font, displayString, nowVector, nowColor);
                nowVector.X++;
                nowVector.Y++;
            }

            // Draw the top of the characters
            spriteBatch.DrawString(font, displayString, nowVector, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
