using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Side_scrolling_game
{
    class Player
    {
        #region fields

        int frameHeight;

        Rectangle drawRectangle;
        Texture2D frontSprite;
        Texture2D backSprite;

        bool alive;

        bool facingForward;

        Rectangle previousRectangle;

        int elapsedFallMilliseconds;

        bool jumping;
        bool freefall;

        const float PLAYER_SPEED = .6f;
        const int JUMP_SPEED = 10;

        #endregion

        #region properties

        public Rectangle rectangle
        {
            //returns the player's position
            get { return drawRectangle; }
            set { drawRectangle = value; }
        }

        public int previousRectangleX
        {
            get { return previousRectangle.X; }
            set { previousRectangle.X = value; }
        }

        public int fallMilliseconds
        {
            get { return elapsedFallMilliseconds; }
        }

        public bool isPlayerAlive
        {
            get { return alive; }
        }

        public bool facingRight
        {
            get { return facingForward; }
        }

        #endregion

        #region constructors

        public Player(Texture2D forwardSprite, Texture2D reverseSprite, Vector2 position, int frameHeight)
        {
            this.frameHeight = frameHeight;

            this.frontSprite = forwardSprite;
            backSprite = reverseSprite;

            drawRectangle = new Rectangle((int)(position.X), (int)(position.Y), forwardSprite.Width, forwardSprite.Height);

            alive = true;

            facingForward = true;

            previousRectangle = drawRectangle;

            jumping = false;

            elapsedFallMilliseconds = 0;
        }

        #endregion

        #region private methods

        private void jump(GameTime time, KeyboardState keyboard)
        {
            if ((keyboard.IsKeyDown(Keys.W) || keyboard.IsKeyDown(Keys.Up)) && elapsedFallMilliseconds == 0)
            {
                    jumping = true;
            }
            if (jumping && !freefall)
            {
                drawRectangle.Y -= JUMP_SPEED;
            }
        }

        #endregion

        #region public methods

        public void update(GameTime gameTime, KeyboardState keyboard)
        {
            if (keyboard.IsKeyDown(Keys.D) || keyboard.IsKeyDown(Keys.Right))
            {
                //move the character foreward
                drawRectangle.X += (int)(gameTime.ElapsedGameTime.Milliseconds * PLAYER_SPEED);
                
                //make the character face foreward
                facingForward = true;
            }
            else if (keyboard.IsKeyDown(Keys.A) || keyboard.IsKeyDown(Keys.Left))
            {
                // move the character backward
                drawRectangle.X -= (int)(gameTime.ElapsedGameTime.Milliseconds * PLAYER_SPEED);
                
                //make the character face backward
                facingForward = false;
            }

            jump(gameTime, keyboard);

             elapsedFallMilliseconds += gameTime.ElapsedGameTime.Milliseconds;

            //see if the player died
            if (drawRectangle.Bottom > frameHeight)
            {
                alive = false;
            }
        }

        public void draw(SpriteBatch spriteBatch)
        {
            if (facingForward)
            {
                spriteBatch.Draw(frontSprite, drawRectangle, Color.White);
            }
            else
            {
                spriteBatch.Draw(backSprite, drawRectangle, Color.White);
            }
        }

        public int resetX()
        {
            //makes the character centered
            int offset = drawRectangle.X - 300;
            drawRectangle.X = 300;
            return offset;
        }

        public void platformCollision(Rectangle platform)
        {
            if (previousRectangle.Left >= platform.Right)
            {
                drawRectangle.X = platform.Right;
            }
            else if (previousRectangle.Right <= platform.Left)
            {
                drawRectangle.X = platform.Left - drawRectangle.Width;
            }
            else if (previousRectangle.Top >= platform.Bottom)
            {
                drawRectangle.Y = platform.Bottom;
                freefall = true;
            }
            else if (previousRectangle.Bottom <= platform.Top)
            {
                drawRectangle.Y = platform.Top - drawRectangle.Height;
                elapsedFallMilliseconds = 0;
                jumping = false;
                freefall = false;
            }
        }

        public void resetPreviousRectangle()
        {
            previousRectangle = drawRectangle;
        }

        public void updateTime(GameTime gameTime)
        {
            elapsedFallMilliseconds += gameTime.ElapsedGameTime.Milliseconds;
        }

        public void kill()
        {
            alive = false;
        }

        #endregion
    }
}
