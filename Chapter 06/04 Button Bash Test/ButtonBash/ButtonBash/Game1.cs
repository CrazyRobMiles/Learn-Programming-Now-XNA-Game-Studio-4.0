#define test

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

namespace ButtonBash
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

        // Gamepad 1
        GamePadState pad1;
        GamePadState oldpad1;
        int acount1;
        int bcount1;
        int xcount1;
        int ycount1;

        Vector2 apos1 = new Vector2(150, 150);
        Vector2 bpos1 = new Vector2(200, 100);
        Vector2 xpos1 = new Vector2(100, 100);
        Vector2 ypos1 = new Vector2(150, 50);

        // Gamepad 2
        GamePadState pad2;
        GamePadState oldpad2;
        int acount2;
        int bcount2;
        int xcount2;
        int ycount2;

        Vector2 apos2 = new Vector2(650, 150);
        Vector2 bpos2 = new Vector2(700, 100);
        Vector2 xpos2 = new Vector2(600, 100);
        Vector2 ypos2 = new Vector2(650, 50);

        // Gamepad 3
        GamePadState pad3;
        GamePadState oldpad3;
        int acount3;
        int bcount3;
        int xcount3;
        int ycount3;

        Vector2 apos3 = new Vector2(150, 350);
        Vector2 bpos3 = new Vector2(200, 300);
        Vector2 xpos3 = new Vector2(100, 300);
        Vector2 ypos3 = new Vector2(150, 250);

        // Gamepad 4
        GamePadState pad4;
        GamePadState oldpad4;
        int acount4;
        int bcount4;
        int xcount4;
        int ycount4;

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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            pad1 = GamePad.GetState(PlayerIndex.One);

            if (pad1.IsConnected)
            {

                if (pad1.Buttons.Start == ButtonState.Pressed)
                {
                    acount1 = 0;
                    bcount1 = 0;
                    xcount1 = 0;
                    ycount1 = 0;
                }

                if (oldpad1.Buttons.A == ButtonState.Released &&
                     pad1.Buttons.A == ButtonState.Pressed)
                {
                    acount1++;
                }

                if (oldpad1.Buttons.B == ButtonState.Released &&
                     pad1.Buttons.B == ButtonState.Pressed)
                {
                    bcount1++;
                }

                if (oldpad1.Buttons.X == ButtonState.Released &&
                     pad1.Buttons.X == ButtonState.Pressed)
                {
                    xcount1++;
                }

                if (oldpad1.Buttons.Y == ButtonState.Released &&
                     pad1.Buttons.Y == ButtonState.Pressed)
                {
                    ycount1++;
                }

                oldpad1 = pad1;
            }

            pad2 = GamePad.GetState(PlayerIndex.Two);

#if test
            // test code - copy the value of pad1 into pad2
            pad2 = pad1;
#endif

            if (pad2.IsConnected)
            {

                if (oldpad2.Buttons.A == ButtonState.Released &&
                     pad2.Buttons.A == ButtonState.Pressed)
                {
                    acount2++;
                }

                if (oldpad2.Buttons.B == ButtonState.Released &&
                     pad2.Buttons.B == ButtonState.Pressed)
                {
                    bcount2++;
                }

                if (oldpad2.Buttons.X == ButtonState.Released &&
                     pad2.Buttons.X == ButtonState.Pressed)
                {
                    xcount2++;
                }

                if (oldpad2.Buttons.Y == ButtonState.Released &&
                     pad2.Buttons.Y == ButtonState.Pressed)
                {
                    ycount2++;
                }

                oldpad2 = pad2;
            }

            pad3 = GamePad.GetState(PlayerIndex.Three);

#if test
            // test code - copy the value of pad1 into pad3
            pad3 = pad1;
#endif

            if (pad3.IsConnected)
            {

                if (oldpad3.Buttons.A == ButtonState.Released &&
                     pad3.Buttons.A == ButtonState.Pressed)
                {
                    acount3++;
                }

                if (oldpad3.Buttons.B == ButtonState.Released &&
                     pad3.Buttons.B == ButtonState.Pressed)
                {
                    bcount3++;
                }

                if (oldpad3.Buttons.X == ButtonState.Released &&
                     pad3.Buttons.X == ButtonState.Pressed)
                {
                    xcount3++;
                }

                if (oldpad3.Buttons.Y == ButtonState.Released &&
                     pad3.Buttons.Y == ButtonState.Pressed)
                {
                    ycount3++;
                }

                oldpad3 = pad3;
            }

            pad4 = GamePad.GetState(PlayerIndex.Four);

#if test
            // test code - copy the value of pad1 into pad4
            pad4 = pad1;
#endif

            if (pad4.IsConnected)
            {

                if (oldpad4.Buttons.A == ButtonState.Released &&
                     pad4.Buttons.A == ButtonState.Pressed)
                {
                    acount4++;
                }

                if (oldpad4.Buttons.B == ButtonState.Released &&
                     pad4.Buttons.B == ButtonState.Pressed)
                {
                    bcount4++;
                }

                if (oldpad4.Buttons.X == ButtonState.Released &&
                     pad4.Buttons.X == ButtonState.Pressed)
                {
                    xcount4++;
                }

                if (oldpad4.Buttons.Y == ButtonState.Released &&
                     pad4.Buttons.Y == ButtonState.Pressed)
                {
                    ycount4++;
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
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            if (pad1.IsConnected)
            {
                spriteBatch.DrawString(font, acount1.ToString(), apos1, Color.Green);
                spriteBatch.DrawString(font, bcount1.ToString(), bpos1, Color.Red);
                spriteBatch.DrawString(font, xcount1.ToString(), xpos1, Color.Blue);
                spriteBatch.DrawString(font, ycount1.ToString(), ypos1, Color.Yellow);
            }

            if (pad2.IsConnected)
            {
                spriteBatch.DrawString(font, acount2.ToString(), apos2, Color.Green);
                spriteBatch.DrawString(font, bcount2.ToString(), bpos2, Color.Red);
                spriteBatch.DrawString(font, xcount2.ToString(), xpos2, Color.Blue);
                spriteBatch.DrawString(font, ycount2.ToString(), ypos2, Color.Yellow);
            }

            if (pad3.IsConnected)
            {
                spriteBatch.DrawString(font, acount3.ToString(), apos3, Color.Green);
                spriteBatch.DrawString(font, bcount3.ToString(), bpos3, Color.Red);
                spriteBatch.DrawString(font, xcount3.ToString(), xpos3, Color.Blue);
                spriteBatch.DrawString(font, ycount3.ToString(), ypos3, Color.Yellow);
            }

            if (pad4.IsConnected)
            {
                spriteBatch.DrawString(font, acount4.ToString(), apos4, Color.Green);
                spriteBatch.DrawString(font, bcount4.ToString(), bpos4, Color.Red);
                spriteBatch.DrawString(font, xcount4.ToString(), xpos4, Color.Blue);
                spriteBatch.DrawString(font, ycount4.ToString(), ypos4, Color.Yellow);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
