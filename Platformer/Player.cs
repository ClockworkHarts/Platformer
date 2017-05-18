using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace Platformer
{
    class Player
    {
        Sprite sprite = new Sprite();

        // keep reference to the game object
        // so we can check for collisions on the map
        Game1 game = null;

        bool isFalling = true;
        bool isJumping = false;
        bool autoJump = true;
        public int lives = 5;

        Vector2 velocity = Vector2.Zero;
        //Vector2 position = Vector2.Zero;  do we even need this 
        Vector2 scale = Vector2.Zero;

        // sound related stuff
        SoundEffect jumpSound;
        SoundEffectInstance jumpSoundInstance;

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

        public Vector2 Velocity
        {
            get
            {
                return velocity;
            }

            set
            {
                velocity = value;
            }
        }

        public Rectangle Bounds
        {
            get
            {
                return sprite.Bounds;
            }
        }

        public bool IsJumping
        {
            get
            {
                return isJumping;
            }
        }

        public void JumpOnCollision()
        {
            autoJump = true;
        }


        public Player(Game1 game)
        {
            sprite.colour = new Color(255, 255, 255, 255);

            this.game = game;
            isFalling = true;
            isJumping = false;
            velocity = Vector2.Zero;
            //position = Vector2.Zero;  is this even needed??
            scale = new Vector2(0.3f, 0.3f);
            
        }

        public void Load(ContentManager content)
        {
            AnimatedTexture animation = new AnimatedTexture(Vector2.Zero, 0,  scale, 1);
            animation.Load(content, "DudeWalking", 9, 13);

            sprite.scale = scale;       //setting the sprite scale(used for bounds) to the player scale
            sprite.Add(animation, 0, 1);
            sprite.Pause();

            jumpSound = content.Load<SoundEffect>("Jump");
            jumpSoundInstance = jumpSound.CreateInstance();
        }

        public void Respawn()
        {
            lives--;
            velocity = new Vector2(0,0);
            Position = new Vector2(0, 0);
        }

        private void UpdateInput(float deltaTime)
        {
            bool wasMovingLeft = velocity.X < 0;
            bool wasMovingRight = velocity.X > 0;
            bool falling = isFalling;

            Vector2 acceleration = new Vector2(0, Game1.gravity);

            if (Keyboard.GetState().IsKeyDown(Keys.A) == true)
            {
                acceleration.X -= Game1.acceleration;
                sprite.SetFlipped(true);
                sprite.Play();
            }
            else if (wasMovingLeft == true)
            {
                acceleration.X += Game1.friction;
            }


            if (Keyboard.GetState().IsKeyDown(Keys.D) == true)
            {
                acceleration.X += Game1.acceleration;
                sprite.SetFlipped(false);
                sprite.Play();
            }
            else if (wasMovingRight == true)
            {
                acceleration.X -= Game1.friction;
            }


            if ((Keyboard.GetState().IsKeyDown(Keys.W) == true && this.isJumping == false && falling == false) || autoJump == true)
            {
                autoJump = false;
                acceleration.Y -= Game1.jumpImpulse;
                this.isJumping = true;
                jumpSoundInstance.Play();
            }

            // calculates velocity every frame based on new acceleration input
            velocity += acceleration * deltaTime;


            // clamps velocity so that the player doesn't continuously accelerate if a key is held down
            velocity.X = MathHelper.Clamp(velocity.X, -Game1.maxVelocity.X, Game1.maxVelocity.X);
            velocity.Y = MathHelper.Clamp(velocity.Y, -Game1.maxVelocity.Y, Game1.maxVelocity.Y);


            // actually updates the sprite's position every frame
            sprite.position += velocity * deltaTime;

            // to stop the player from jiggling as a result
            // of the friction force used to slower the player down
            // not being precisely equal to their forwards force
            // we will clamp the player's horizontal velocity to zero if
            // we detect that the player's direction has just changed
            // essentially, if the player was just moving left and now their velocity is positive (in the right direction)
            // make their horizontal velocity == to 0 for a frame, 
            // before their velocity gets updated again
            if ((wasMovingLeft && (velocity.X > 0)) || (wasMovingRight && (velocity.X < 0)))
            {
                velocity.X = 0;
                sprite.Pause();
            }

            if (velocity.X == 0 || isFalling == true || isJumping == true)
            {
                sprite.Pause();
            }



            //BELOW IS ALL COLLISION LOGIC\\

            int tx = game.PixelToTile(sprite.position.X);
            int ty = game.PixelToTile(sprite.position.Y);

            // x % y divides x by y then looks at the remainder
            // because tile = 64, when the sprite's position (a single pixel)
            // is equal to 64, the remainder is zero,
            // therefore the sprite is only on one tile
            // in any other case, it is overlapping two or more tiles
            // therefore the bools are true
            // nx checks left/right overlapping
            // ny checks up/down overlapping 
            bool nx = (sprite.position.X) % Game1.tile != 0;
            bool ny = (sprite.position.Y) % Game1.tile != 0;

            bool cell = game.CellAtTileCoord(tx, ty) != 0;
            bool cellright = game.CellAtTileCoord(tx + 1, ty) != 0;
            bool celldown = game.CellAtTileCoord(tx, ty + 1) != 0;
            bool celldiag = game.CellAtTileCoord(tx + 1, ty + 1) != 0;


            //if player has vertical velocity
            //check to see if they have collided with a platform above or below
            //if they have clamp their vertical velocity
            if (this.velocity.Y > 0)
            {
                if ((celldown && !cell) || (celldiag && !cellright && nx))
                {
                    sprite.position.Y = game.TileToPixel(ty);
                    this.velocity.Y = 0;
                    this.isFalling = false;
                    this.isJumping = false;
                    ny = false;
                }
            }
            else if (this.velocity.Y < 0)
            {
                if ((cell && !celldown) || (cellright && !celldiag && nx))
                {
                    sprite.position.Y = game.TileToPixel(ty + 1);
                    this.velocity.Y = 0;
                    cell = celldown;
                    cellright = celldiag;
                    ny = false;

                }
            }


            //same as above but for horizontal velocity
            if (this.velocity.X > 0)
            {
                if ((cellright && !cell) || (celldiag && !celldown && ny))
                {
                    sprite.position.X = game.TileToPixel(tx);
                    this.velocity.X = 0;
                    sprite.Pause();
                }
            }
            else if (this.velocity.X < 0)
            {
                if ((cell && !cellright) || (celldown && !celldiag && ny))
                {
                    sprite.position.X = game.TileToPixel(tx + 1);
                    this.velocity.X = 0;
                    sprite.Pause();
                }
            }


            // then we need to detect if the player is falling or not
            // we do this by seeing if there is a cell below them
            this.isFalling = !(celldown || (nx && celldiag));
        

        }

        public void Update(float deltaTime)
        {
            sprite.Update(deltaTime);
            UpdateInput(deltaTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch);
        }
    }
}
