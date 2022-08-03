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

        int[] scores = new int[4];

        // Matching button names
        string[] names = new string[] {
            "Gamepad 1 A",
            "Gamepad 1 B",
            "Gamepad 1 X",
            "Gamepad 1 Y"
        };

        string winnerName = "";
        Vector2 winnerPos = new Vector2(150, 400);

        // Game world sounds
        SoundEffect dingSound;

        // Gamepad 1
        GamePadState pad1;
        GamePadState oldpad1;
        Vector2 apos1 = new Vector2(150, 150);
        Vector2 bpos1 = new Vector2(200, 100);
        Vector2 xpos1 = new Vector2(100, 100);
        Vector2 ypos1 = new Vector2(150, 50);

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
                for (int i = 0; i < 4; i++)
                {
                    scores[i] = 0;
                }
                winnerName = "";
                timer = -120;
            }

            // update the timer
            timer++;

            // play the sound at the start of the game
            if (timer == 0)
            {
                dingSound.Play();
            }


            // if A is pressed and scores[0] is 0 copy the timer
            if (oldpad1.Buttons.A == ButtonState.Released &&
                 pad1.Buttons.A == ButtonState.Pressed && scores[0] == 0)
            {
                scores[0] = timer;
            }

            if (oldpad1.Buttons.B == ButtonState.Released &&
                 pad1.Buttons.B == ButtonState.Pressed && scores[1] == 0)
            {
                scores[1] = timer;
            }

            if (oldpad1.Buttons.X == ButtonState.Released &&
                 pad1.Buttons.X == ButtonState.Pressed && scores[2] == 0)
            {
                scores[2] = timer;
            }

            if (oldpad1.Buttons.Y == ButtonState.Released &&
                 pad1.Buttons.Y == ButtonState.Pressed && scores[3] == 0)
            {
                scores[3] = timer;
            }

            oldpad1 = pad1;

            if (timer == 120)
            {
                int winningValue = 120;
                int winnerSubscript = 0;
                for (int i = 0; i < 4; i++)
                {
                    if (scores[i] > 0)
                    {
                        if (scores[i] < winningValue)
                        {
                            winningValue = scores[i];
                            winnerSubscript = i;
                        }
                    }
                }

                if (winningValue != 120)
                {
                    winnerName = names[winnerSubscript];
                }
                else
                {
                    winnerName = "**NO WINNER**";
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
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            if (pad1.IsConnected)
            {
                spriteBatch.DrawString(font, scores[0].ToString(), apos1, Color.Green);
                spriteBatch.DrawString(font, scores[1].ToString(), bpos1, Color.Red);
                spriteBatch.DrawString(font, scores[2].ToString(), xpos1, Color.Blue);
                spriteBatch.DrawString(font, scores[3].ToString(), ypos1, Color.Yellow);
            }

            spriteBatch.DrawString(font, winnerName, winnerPos, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
