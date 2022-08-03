#define debug

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

namespace ReactionTimer
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Game World

        // Display font
        SpriteFont font;

        // Game Timer
        int timer;

        // Game world sounds
        SoundEffect dingSound;

        // Gamepad 1
        GamePadState pad1;
        GamePadState oldpad1;
        int ascore1;
        int bscore1;
        int xscore1;
        int yscore1;

        Vector2 apos1 = new Vector2(150, 150);
        Vector2 bpos1 = new Vector2(200, 100);
        Vector2 xpos1 = new Vector2(100, 100);
        Vector2 ypos1 = new Vector2(150, 50);

        // Gamepad 2
        GamePadState pad2;
        GamePadState oldpad2;
        int ascore2;
        int bscore2;
        int xscore2;
        int yscore2;

        Vector2 apos2 = new Vector2(650, 150);
        Vector2 bpos2 = new Vector2(700, 100);
        Vector2 xpos2 = new Vector2(600, 100);
        Vector2 ypos2 = new Vector2(650, 50);

        // Gamepad 3
        GamePadState pad3;
        GamePadState oldpad3;
        int ascore3;
        int bscore3;
        int xscore3;
        int yscore3;

        Vector2 apos3 = new Vector2(150, 350);
        Vector2 bpos3 = new Vector2(200, 300);
        Vector2 xpos3 = new Vector2(100, 300);
        Vector2 ypos3 = new Vector2(150, 250);

        // Gamepad 4
        GamePadState pad4;
        GamePadState oldpad4;
        int ascore4;
        int bscore4;
        int xscore4;
        int yscore4;

        Vector2 apos4 = new Vector2(650, 350);
        Vector2 bpos4 = new Vector2(700, 300);
        Vector2 xpos4 = new Vector2(600, 300);
        Vector2 ypos4 = new Vector2(650, 250);

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

            dingSound = Content.Load<SoundEffect>("ding");
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
            pad1 = GamePad.GetState(PlayerIndex.One);

            if (pad1.Buttons.Back == ButtonState.Pressed)
            {
                this.Exit();
            }

            // start a new game
            if (pad1.Buttons.Start == ButtonState.Pressed)
            {
                ascore1 = 0;
                bscore1 = 0;
                xscore1 = 0;
                yscore1 = 0;
                ascore2 = 0;
                bscore2 = 0;
                xscore2 = 0;
                yscore2 = 0;
                ascore3 = 0;
                bscore3 = 0;
                xscore3 = 0;
                yscore3 = 0;
                ascore4 = 0;
                bscore4 = 0;
                xscore4 = 0;
                yscore4 = 0;
                timer = -120;
            }

            // update the timer
            timer++;

            // play the sound at the start of the game
            if (timer == 0)
            {
                dingSound.Play();
            }

            // if A is pressed copy the timer
            if (oldpad1.Buttons.A == ButtonState.Released &&
                 pad1.Buttons.A == ButtonState.Pressed && ascore1 == 0)
            {
                ascore1 = timer;
            }

            if (oldpad1.Buttons.B == ButtonState.Released &&
                 pad1.Buttons.B == ButtonState.Pressed && bscore1 == 0)
            {
                bscore1 = timer;
            }

            if (oldpad1.Buttons.X == ButtonState.Released &&
                 pad1.Buttons.X == ButtonState.Pressed && xscore1 == 0)
            {
                xscore1 = timer;
            }

            if (oldpad1.Buttons.Y == ButtonState.Released &&
                 pad1.Buttons.Y == ButtonState.Pressed && yscore1 == 0)
            {
                yscore1 = timer;
            }

            oldpad1 = pad1;

            pad2 = GamePad.GetState(PlayerIndex.Two);

#if (debug)
            pad2 = pad1;
#endif
            if (pad2.IsConnected)
            {

                if (oldpad2.Buttons.A == ButtonState.Released &&
                     pad2.Buttons.A == ButtonState.Pressed && ascore2 == 0)
                {
                    ascore2 = timer;
                }

                if (oldpad2.Buttons.B == ButtonState.Released &&
                     pad2.Buttons.B == ButtonState.Pressed && bscore2 == 0)
                {
                    bscore2 = timer;
                }

                if (oldpad2.Buttons.X == ButtonState.Released &&
                     pad2.Buttons.X == ButtonState.Pressed && xscore2 == 0)
                {
                    xscore2 = timer;
                }

                if (oldpad2.Buttons.Y == ButtonState.Released &&
                     pad2.Buttons.Y == ButtonState.Pressed && yscore2 == 0)
                {
                    yscore2 = timer;
                }

                oldpad2 = pad2;
            }

            pad3 = GamePad.GetState(PlayerIndex.Three);
#if (debug)
            pad3 = pad1;
#endif

            if (pad3.IsConnected)
            {

                if (oldpad3.Buttons.A == ButtonState.Released &&
                     pad3.Buttons.A == ButtonState.Pressed && ascore3 == 0)
                {
                    ascore3 = timer;
                }

                if (oldpad3.Buttons.B == ButtonState.Released &&
                     pad3.Buttons.B == ButtonState.Pressed && bscore3 == 0)
                {
                    bscore3 = timer;
                }

                if (oldpad3.Buttons.X == ButtonState.Released &&
                     pad3.Buttons.X == ButtonState.Pressed && xscore3 == 0)
                {
                    xscore3 = timer;
                }

                if (oldpad3.Buttons.Y == ButtonState.Released &&
                     pad3.Buttons.Y == ButtonState.Pressed && yscore3 == 0)
                {
                    yscore3 = timer;
                }

                oldpad3 = pad3;
            }

            pad4 = GamePad.GetState(PlayerIndex.Four);

#if (debug)
            pad4 = pad1;
#endif
            if (pad4.IsConnected)
            {

                if (oldpad4.Buttons.A == ButtonState.Released &&
                     pad4.Buttons.A == ButtonState.Pressed && ascore4 == 0)
                {
                    ascore4 = timer;
                }

                if (oldpad4.Buttons.B == ButtonState.Released &&
                     pad4.Buttons.B == ButtonState.Pressed && bscore4 == 0)
                {
                    bscore4 = timer;
                }

                if (oldpad4.Buttons.X == ButtonState.Released &&
                     pad4.Buttons.X == ButtonState.Pressed && xscore4 == 0)
                {
                    xscore4 = timer;
                }

                if (oldpad4.Buttons.Y == ButtonState.Released &&
                     pad4.Buttons.Y == ButtonState.Pressed && yscore4 == 0)
                {
                    yscore4 = timer;
                }

                oldpad4 = pad4;
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

            if (pad1.IsConnected)
            {
                spriteBatch.DrawString(font, ascore1.ToString(), apos1, Color.Green);
                spriteBatch.DrawString(font, bscore1.ToString(), bpos1, Color.Red);
                spriteBatch.DrawString(font, xscore1.ToString(), xpos1, Color.Blue);
                spriteBatch.DrawString(font, yscore1.ToString(), ypos1, Color.Yellow);
            }

            if (pad2.IsConnected)
            {
                spriteBatch.DrawString(font, ascore2.ToString(), apos2, Color.Green);
                spriteBatch.DrawString(font, bscore2.ToString(), bpos2, Color.Red);
                spriteBatch.DrawString(font, xscore2.ToString(), xpos2, Color.Blue);
                spriteBatch.DrawString(font, yscore2.ToString(), ypos2, Color.Yellow);
            }

            if (pad3.IsConnected)
            {
                spriteBatch.DrawString(font, ascore3.ToString(), apos3, Color.Green);
                spriteBatch.DrawString(font, bscore3.ToString(), bpos3, Color.Red);
                spriteBatch.DrawString(font, xscore3.ToString(), xpos3, Color.Blue);
                spriteBatch.DrawString(font, yscore3.ToString(), ypos3, Color.Yellow);
            }

            if (pad4.IsConnected)
            {
                spriteBatch.DrawString(font, ascore4.ToString(), apos4, Color.Green);
                spriteBatch.DrawString(font, bscore4.ToString(), bpos4, Color.Red);
                spriteBatch.DrawString(font, xscore4.ToString(), xpos4, Color.Blue);
                spriteBatch.DrawString(font, yscore4.ToString(), ypos4, Color.Yellow);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
