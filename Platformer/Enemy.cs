using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
namespace Platformer
{
    class Enemy
    {
        Sprite sprite = new Sprite();
        // keep reference for Game object to check for collisions on the map
        Game1 game = null;

        public Vector2 velocity = Vector2.Zero;
        Vector2 scale = Vector2.Zero;

        float pause = 0;
        public bool moveRight = true;

        static float enemyAcceleration = Game1.acceleration / 5.0f;
        static Vector2 enemyMaxVelocity = new Vector2(1f, 1f);
        
        public Vector2 Position
        {
            get
            {
                return sprite.position;
            }

            set
            {
                sprite.position = value;
            }
        }


        public Rectangle Bounds
        {
            get
            {
                return sprite.Bounds;
            }
        }

        public Enemy(Game1 game)
        {
            this.game = game;
            velocity = Vector2.Zero;
            scale = new Vector2(0.5f, 0.5f);
        }

        public void Load(ContentManager content)
        {
            AnimatedTexture animation = new AnimatedTexture(Vector2.Zero, 0, scale, 1);
            animation.Load(content, "enemy", 5, 5);

            sprite.scale = scale;       //setting the sprite scale (used for bounds) to the enemy scale 

            sprite.Add(animation, 0, 4);
        }

        public void Update(float deltaTime)
        {
            sprite.Update(deltaTime);

            if (pause > 0)
            {
                pause -= deltaTime;
            }
            else
            {
                float ddx = 0;  //acceleration

                int tx = game.PixelToTile(Position.X);
                int ty = game.PixelToTile(Position.Y);
                bool nx = (Position.X) % Game1.tile != 0;
                bool ny = (Position.Y) % Game1.tile != 0;

                bool cell = game.CellAtTileCoord(tx, ty, Game1.current.collisionLayer) != 0;
                bool cellright = game.CellAtTileCoord(tx + 1, ty, Game1.current.collisionLayer) != 0;
                bool celldown = game.CellAtTileCoord(tx, ty + 1, Game1.current.collisionLayer) != 0;
                bool celldiag = game.CellAtTileCoord(tx + 1, ty + 1, Game1.current.collisionLayer) != 0;

                if (moveRight)
                {
                    if (celldiag && !cellright)
                    {
                        ddx = ddx + enemyAcceleration;
                    }
                    else
                    {
                        this.velocity.X = 0;
                        this.moveRight = false;
                        this.pause = 0.3f;
                        sprite.SetFlipped(true);
                    }
                }

                if (!moveRight)
                {
                    if (celldown && !cell)
                    {
                        ddx = ddx - enemyAcceleration;
                    }
                    else
                    {
                        this.velocity.X = 0;
                        this.moveRight = true;
                        this.pause = 0.3f;
                        sprite.SetFlipped(false);
                    }
                }

                Position = new Vector2((float)Math.Floor(Position.X + (velocity.X)), Position.Y);

                velocity.X = MathHelper.Clamp(velocity.X + (deltaTime * ddx), -enemyMaxVelocity.X, enemyMaxVelocity.X);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch);
        }


    }
}
