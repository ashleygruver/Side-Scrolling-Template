using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Side_scrolling_game
{
    class Projectile
    {
        #region fields
        const int LIFETIME = 500;
        int elapsedLifetime;

        const float PROJECTILE_SPEED = 1.2f;

        Texture2D sprite;
        Rectangle drawRectangle;

        bool facingForward;

        bool friendly;

        #endregion

        #region properties

        public Rectangle rectangle
        {
            get { return drawRectangle; }
        }

        public int rectangleX
        {
            get { return drawRectangle.X; }
            set { drawRectangle.X = value; }
        }

        public bool playerOwned
        {
            get { return friendly; }
        }

        #endregion

        #region constructors

        public Projectile(Texture2D projectileSprite, bool aimedForward, bool playerCreated, Vector2 position)
        {
            sprite = projectileSprite;
            facingForward = aimedForward;
            friendly = playerCreated;
            drawRectangle.X = (int)(position.X);
            drawRectangle.Y = (int)(position.Y);
            drawRectangle.Width = sprite.Width;
            drawRectangle.Height = sprite.Height;
        }

        #endregion

        #region public methods

        public bool update(GameTime gameTime)
        {
            if (facingForward)
            {
                drawRectangle.X += (int)(PROJECTILE_SPEED * gameTime.ElapsedGameTime.Milliseconds);
            }
            else if (!facingForward)
            {
                drawRectangle.X -= (int)(PROJECTILE_SPEED * gameTime.ElapsedGameTime.Milliseconds);
            }
            elapsedLifetime += gameTime.ElapsedGameTime.Milliseconds;
            if (elapsedLifetime >= LIFETIME)
            {
                return true;
            }
            return false;
        }

        public void draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, drawRectangle, Color.White);
        }

        #endregion
    }
}
