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

using Microsoft.Devices.Sensors;

namespace CheeseLander
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class CheeseLanderGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        int displayWidth;
        int displayHeight;

        // Cheese 
        Texture2D cheeseTexture;
        Rectangle cheeseRectangle;
        int cheeseWidth;
        float cheeseWidthFactor = 8;

        float cheeseX;
        float cheeseY;

        // Bread
        Texture2D breadTexture;
        Rectangle breadRectangle;
        int breadWidth;
        float breadWidthFactor = 3;

        // Backgrounds and messages
        Texture2D wonTexture;
        Texture2D lostTexture;
        Texture2D backgroundTexture;
        Rectangle fullScreenDrawPos;

        // Accelleration values

        Accelerometer accel;
        Vector3 accelReading;
        float acceleratorPower = 8;

        enum gameState
        {
            playing,
            won,
            lost
        }

        private gameState state = gameState.playing;

        SoundEffect wonSound;
        SoundEffect lostSound;

        Random rand = new Random();

        public CheeseLanderGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 240;
            graphics.PreferredBackBufferHeight = 400;

            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);
        }


        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            accel = new Accelerometer();

            accel.ReadingChanged += new EventHandler<AccelerometerReadingEventArgs>(accel_ReadingChanged);

            accel.Start();

            base.Initialize();
        }

        void accel_ReadingChanged(object sender, AccelerometerReadingEventArgs e)
        {
            accelReading.X = (float)e.X;
            accelReading.Y = (float)e.Y;
            accelReading.Z = (float)e.Z;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            displayWidth = GraphicsDevice.Viewport.Width;
            displayHeight = GraphicsDevice.Viewport.Height;

            cheeseWidth = (int)(displayWidth / cheeseWidthFactor);
            breadWidth = (int)(displayWidth / breadWidthFactor);

            // Create the draw rectangle 
            // Cheese should be a 12th the size of the screen
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load the cheese and set the size of the draw rectangle
            cheeseTexture = Content.Load<Texture2D>("cheese");
            cheeseRectangle.Width = cheeseWidth;
            float aspectRatio = (float)cheeseTexture.Width / cheeseTexture.Height;
            cheeseRectangle.Height = (int)((cheeseWidth / aspectRatio) + 0.5f);

            // Load the bread and set the size of the draw rectangle
            breadTexture = Content.Load<Texture2D>("bread");
            breadRectangle.Width = breadWidth;
            aspectRatio = (float)breadTexture.Width / breadTexture.Height;
            breadRectangle.Height = (int)((breadWidth / aspectRatio) + 0.5f);

            // Load the background
            backgroundTexture = Content.Load<Texture2D>("background");

            // Load the win and lose textures
            wonTexture = Content.Load<Texture2D>("wonScreen");
            lostTexture = Content.Load<Texture2D>("lostScreen");

            fullScreenDrawPos = new Rectangle(
                0, 0,
                displayWidth, displayHeight);

            // Load the sound effects
            wonSound = Content.Load<SoundEffect>("wonSound");
            lostSound = Content.Load<SoundEffect>("lostSound");

            newGame();

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        private void updateGame()
        {
            // Update the X and Y positions
            cheeseX = cheeseX + (accelReading.X * acceleratorPower);
            cheeseY = cheeseY - (accelReading.Y * acceleratorPower);

            // Set the cheese Draw rectangle to cheeseX and cheeseY
            cheeseRectangle.X = (int)(cheeseX + 0.5f);
            cheeseRectangle.Y = (int)(cheeseY + 0.5f);

            // Limit the sides

            if (cheeseX < 0)
            {
                cheeseX = 0; ;
            }

            if (cheeseY < 0)
            {
                cheeseY = 0;
            }

            if (cheeseRectangle.Right > displayWidth)
            {
                cheeseX = displayWidth - cheeseWidth;
            }

            if (cheeseRectangle.Bottom > GraphicsDevice.Viewport.Height)
            {
                cheeseY = displayHeight - cheeseRectangle.Width;
            }

            if (cheeseRectangle.Intersects(breadRectangle))
            {
                // May have won 
                if (cheeseRectangle.Top < breadRectangle.Top &&
                    cheeseRectangle.Left >= breadRectangle.Left &&
                    cheeseRectangle.Right <= breadRectangle.Right)
                {
                    // cheese is properly landed – player wins
                    gameWon();
                }
                else
                {
                    // cheese is badly landed – player loses
                    gameLost();
                }
            }
        }

        private void gameWon()
        {
            wonSound.Play();
            state = gameState.won;
        }

        private void gameLost()
        {
            lostSound.Play();
            state = gameState.lost;
        }

        private void newGame()
        {
            // Put the cheese at a random position on the top of the scren
            cheeseX = rand.Next(displayWidth - cheeseWidth);
            cheeseY = 0;

            // Move the bread to a new position across the screen
            breadRectangle.X = rand.Next(displayWidth - breadWidth);
            breadRectangle.Y = displayHeight - breadRectangle.Height;

            // Start playing the game
            state = gameState.playing;
        }

        private void checkNewGame()
        {
            // Get the Touch Screen

            TouchCollection touches = TouchPanel.GetState();

            foreach (TouchLocation location in touches)
            {
                if (location.State == TouchLocationState.Pressed)
                {
                    newGame();
                    break;
                }
            }
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

            switch (state)
            {
                case gameState.playing:
                    updateGame();
                    break;
                case gameState.lost:
                    checkNewGame();
                    break;
                case gameState.won:
                    checkNewGame();
                    break;
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

            switch (state)
            {
                case gameState.playing:
                    spriteBatch.Draw(backgroundTexture, fullScreenDrawPos, Color.White);
                    spriteBatch.Draw(breadTexture, breadRectangle, Color.White);
                    spriteBatch.Draw(cheeseTexture, cheeseRectangle, Color.White);
                    break;

                case gameState.won:
                    spriteBatch.Draw(wonTexture, fullScreenDrawPos, Color.White);
                    break;

                case gameState.lost:
                    spriteBatch.Draw(lostTexture, fullScreenDrawPos, Color.White);
                    break;
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
