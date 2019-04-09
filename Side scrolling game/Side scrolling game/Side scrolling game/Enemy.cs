using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Side_scrolling_game
{
    class Enemy
    {
        #region fields
        int frameHeight;

        Texture2D sprite;
        Rectangle drawRectangle;

        Rectangle previousRectangle;

        bool alive;

        bool shooting;
        int shootTime;
        int currentShootTime;

        bool facingRight;
        float speed = .2f;

        int fallTime;

        #endregion

        #region properties

        public Rectangle rectangle
        {
            get { return drawRectangle; }
            set { drawRectangle = value; }
        }

        public int fallMilliseconds
        {
            get { return fallTime; }
        }

        public bool dead
        {
            get { return !alive; }
        }

        #endregion

        #region constructors

        public Enemy(Texture2D sprite, Vector2 position, bool facingForward, float movementSpeed, int frameHeight, bool canShoot = false, int timeBetweenShots = 1000)
        {
            this.frameHeight = frameHeight;

            this.sprite = sprite;
             drawRectangle = new Rectangle((int)(position.X), (int)(position.Y), sprite.Width, sprite.Height);

            alive = true;
            this.facingRight = facingForward;
            shootTime = timeBetweenShots;
            shooting = canShoot;
            speed = movementSpeed;
        }

        #endregion

        

        #region public methods

        public Projectile update(GameTime gameTime, Texture2D projectileSprite)
        {
            if (alive)
            {
                //moves enemy
                if (facingRight)
                {
                    drawRectangle.X += (int)(gameTime.ElapsedGameTime.Milliseconds * speed);
                }
                else
                {
                    drawRectangle.X -= (int)(gameTime.ElapsedGameTime.Milliseconds * speed);
                }
                //adds fall time
                fallTime += gameTime.ElapsedGameTime.Milliseconds;

                //Kills if below blast line.
                if (drawRectangle.Bottom >= frameHeight)
                {
                    alive = false;
                }
                if (currentShootTime >= shootTime)
                {
                    currentShootTime = 0;
                    if (facingRight)
                    {
                        return new Projectile(projectileSprite, facingRight, false, new Vector2(drawRectangle.Right, drawRectangle.Center.Y));
                    }
                    else
                    {
                        return new Projectile(projectileSprite, facingRight, false, new Vector2(drawRectangle.Left, drawRectangle.Center.Y));
                    }
                }
                else
                {
                    currentShootTime += gameTime.ElapsedGameTime.Milliseconds;
                }
            }
            return null;
        }

        public void draw(SpriteBatch spriteBatch)
        {
            if (alive)
            {
                spriteBatch.Draw(sprite, drawRectangle, Color.White);
            }
        }

        public void resetX(int offset)
        {
            drawRectangle.X += offset;
        }

        public void platformCollide(Rectangle platform)
        {
            if (previousRectangle.Bottom <= platform.Top)
            {
                drawRectangle.Y -= drawRectangle.Bottom - platform.Top;
                fallTime = 0;
            }

            if (previousRectangle.Left > platform.Right || previousRectangle.Right < platform.Left)
            {
                facingRight = !facingRight;
            }
        }

        public void resetPreviousRectangle()
        {
            previousRectangle = drawRectangle;
        }

        #endregion
    }
}
