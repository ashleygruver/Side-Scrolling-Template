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

namespace Side_scrolling_game
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont text;

        bool victory;

        Vector2 textPos = new Vector2(0, 120);
        const string GAME_OVER = "Game Over";
        const string WIN = "You Win.";

        Texture2D blackRectangle;
        List<Rectangle> platforms = new List<Rectangle>();

        Texture2D goldRectangle;
        Rectangle goal;

        List<Enemy> enemies = new List<Enemy>();
        Texture2D gruntSprite;
        Texture2D bossSprite;

        List<Projectile> projectiles = new List<Projectile>();
        Texture2D projectileSprite;
        int projectileLaunchTimer;


        Player player;

        const float GRAVITY_ACCELERATION = .00007f;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferHeight = 400;
            graphics.PreferredBackBufferWidth = 600;

        }

        private Rectangle gravity(GameTime time, Rectangle rectangle, int elapsedFallMilliseconds)
        {
            rectangle.Y += (int)(GRAVITY_ACCELERATION * Math.Pow((double)(elapsedFallMilliseconds), 2));
            return rectangle;
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

            text = Content.Load<SpriteFont>("SpriteFont1");

            // initilizes a platform
            blackRectangle = new Texture2D(graphics.GraphicsDevice, 1, 1);
            goldRectangle = new Texture2D(graphics.GraphicsDevice, 1, 1);

            blackRectangle.SetData(new[] {Color.White});
            goldRectangle.SetData(new[] { Color.White });

            projectileSprite = Content.Load<Texture2D>("Projectile");
            gruntSprite = Content.Load<Texture2D>("Whitespace");
            bossSprite = Content.Load<Texture2D>("Boss");


            //initilizes the goal
            //goal = new Rectangle(1100, 110, 20, 100);

            //initilizes platform locations
            platforms.Add(new Rectangle(100, 100, 100, 50));
            platforms.Add(new Rectangle(700, 200, 100, 10));
            platforms.Add(new Rectangle(0, 350, 1000, 15));
            platforms.Add(new Rectangle(700, 250, 15, 300));
            platforms.Add(new Rectangle(1100, 200, 5000, 92));
            platforms.Add(new Rectangle(1100, 180, 15, 20));
            platforms.Add(new Rectangle(6100, 180, 15, 20));

            //add enemies
            enemies.Add(new Enemy(gruntSprite, new Vector2(30, 15), true, .2f, graphics.PreferredBackBufferHeight));
            enemies.Add(new Enemy(gruntSprite, new Vector2(500, -100), false, .2f,  graphics.PreferredBackBufferHeight));
            enemies.Add(new Enemy(gruntSprite, new Vector2(1500, 0), true, .2f, graphics.PreferredBackBufferHeight, true));
            enemies.Add(new Enemy(bossSprite, new Vector2(700, 0), false, .01f, graphics.PreferredBackBufferHeight, true, 500));

            //initilizes the player
            player = new Player(Content.Load<Texture2D>("Untitled"), Content.Load<Texture2D>("Untitled Flipped"), new Vector2(300, 300), graphics.PreferredBackBufferHeight);
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

            //updates the player
            player.rectangle = gravity(gameTime, player.rectangle, player.fallMilliseconds);
            if (player.isPlayerAlive && !victory)
            {
                player.update(gameTime, Keyboard.GetState());
            }
            else if (!player.isPlayerAlive)
            {
                player.updateTime(gameTime);
            }

            //updates enemies
            for(int i = 0; i < enemies.Count; i++)
            {
                enemies[i].rectangle = gravity(gameTime, enemies[i].rectangle, enemies[i].fallMilliseconds);
                Projectile newProjectile = enemies[i].update(gameTime, projectileSprite);
                if (newProjectile != null)
                {
                    projectiles.Add(newProjectile);
                }
                if (enemies[i].dead)
                {
                    enemies.RemoveAt(i);
                    i--;
                }
            }

            //updates projectiles
            for (int i = 0; i < projectiles.Count; i++)
            {
                if(projectiles[i].update(gameTime))
                {
                    projectiles.RemoveAt(i);
                    i--;
                }
            }

            //Shoot projectiles
            if (Keyboard.GetState().IsKeyDown(Keys.Space) && player.isPlayerAlive && projectileLaunchTimer >= 100)
            {
                if (player.facingRight)
                {
                    projectiles.Add(new Projectile(projectileSprite, player.facingRight, true, new Vector2(player.rectangle.Right, player.rectangle.Center.Y)));
                }
                else
                {
                    projectiles.Add(new Projectile(projectileSprite, player.facingRight, true, new Vector2(player.rectangle.Left, player.rectangle.Center.Y)));
                }
                projectileLaunchTimer = 0;
            }
            else
            {
                projectileLaunchTimer += gameTime.ElapsedGameTime.Milliseconds;
            }

            //player platform collisions
            for (int i = 0; i < platforms.Count; i++)
            {
                if (player.rectangle.Intersects(platforms[i]))
                {
                    player.platformCollision(platforms[i]);
                }
            }

            //enemy platform collisions
            for(int i = 0; i < enemies.Count; i++)
            {
                for (int j = 0; j < platforms.Count; j++)
                {
                    if(enemies[i].rectangle.Intersects(platforms[j]))
                    {
                       enemies[i].platformCollide(platforms[j]);
                    }
                }
            }

            //player enemy collisions
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].rectangle.Intersects(player.rectangle))
                {
                    player.kill();
                }
            }

            //player goal collisions
            if(player.rectangle.Intersects(goal))
            {
                victory = true;
            }

            //projectile platform collisions
            for (int i = 0; i < platforms.Count; i++)
            {
                for (int j = 0; j < projectiles.Count; j++)
                {
                    if (platforms[i].Intersects(projectiles[j].rectangle))
                    {
                        projectiles.RemoveAt(j);
                        j--;
                    }
                }
            }

            //enemy projectile collisions
            for (int i = 0; i < projectiles.Count; i++)
            {
                for (int j = 0; j < enemies.Count; j++)
                {
                    if (enemies[j].rectangle.Intersects(projectiles[i].rectangle) && projectiles[i].playerOwned)
                    {
                        enemies.RemoveAt(j);
                        projectiles.RemoveAt(i);
                    }
                }
            }

            //player projectile collisions
            for (int i = 0; i < projectiles.Count; i++)
            {
                if (projectiles[i].rectangle.Intersects(player.rectangle) && !projectiles[i].playerOwned)
                {
                    player.kill();
                }
            }

            player.resetPreviousRectangle();

            if (player.rectangle.X != 300)
            {
                //moves everything by the amount moved to simulate screen motion
                //moves platforms
                for (int i = 0; i < platforms.Count; i++)
                {
                    Rectangle platformsI = platforms[i];
                    platformsI.X -= player.rectangle.X - 300;
                    platforms[i] = platformsI;
                }

                //moves enemies with the player
                for (int i = 0; i < enemies.Count; i++)
                {
                    enemies[i].resetX(300 - player.rectangle.X);
                    enemies[i].resetPreviousRectangle();
                }

                //moves projectiles
                for (int i = 0; i < projectiles.Count; i++)
                {
                    projectiles[i].rectangleX -= player.rectangle.X - 300;
                }

                //moves goal
                goal.X -= player.rectangle.X - 300;

                //moves previous rectangle
                player.previousRectangleX += 300 - player.rectangle.X;


                //resets the player to the center
                player.resetX();
            }
            else
            {
                for (int i = 0; i < enemies.Count; i++)
                {
                    enemies[i].resetPreviousRectangle();
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

            //draws goal
            spriteBatch.Draw(goldRectangle, goal, Color.Gold);

            // draws platforms
            for (int i = 0; i < platforms.Count; i++)
            {
                spriteBatch.Draw(blackRectangle, platforms[i], Color.Black);
            }
            //draws enemies
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].draw(spriteBatch);
            }

            //draws projectiles
            for (int i = 0; i < projectiles.Count; i++)
            {
                projectiles[i].draw(spriteBatch);
            }

            //draws player
            player.draw(spriteBatch);

            //draw endgame text
            if (victory)
            {
                spriteBatch.DrawString(text, WIN, textPos, Color.Firebrick);
            }
            else if (!player.isPlayerAlive)
            {
                spriteBatch.DrawString(text, GAME_OVER, textPos, Color.Magenta);
            }

             spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
