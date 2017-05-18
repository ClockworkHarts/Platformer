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
    class Coin
    {
        Sprite sprite = new Sprite();
        // keep reference for Game object to check for collisions on the map
        Game1 game = null;

        public Color colour;
        public bool isLitFam = false;
        public Vector2 scale;

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



        public Coin(Game1 game)
        {
            this.game = game;
            colour = new Color(58, 231, 5, 255);
            scale = new Vector2(0.5f, 0.5f);
        }

        public void Load(ContentManager content)
        {
            AnimatedTexture animation = new AnimatedTexture(Vector2.Zero, 0, scale, 1);
            animation.Load(content, "coin", 12, 5);

            sprite.scale = scale;
             
            sprite.Add(animation, 0, 5);
        }

        public void Update(float deltaTime)
        {
            sprite.Update(deltaTime);

            if (isLitFam == true)
            {
                sprite.colour = colour;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch);
        }
    }
}
