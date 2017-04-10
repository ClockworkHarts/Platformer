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
    class Sprite
    {
        public Vector2 position;
        public Vector2 offset;

        Texture2D texture;

        public Sprite()
        {
            position = Vector2.Zero;
            offset = Vector2.Zero;
        }

        public void Load(ContentManager content, string asset)
        {
            texture = content.Load<Texture2D>(asset);

        }

        public void Update(float deltaTime)
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
           
            spriteBatch.Draw(texture, position + offset, Color.White);
        }


    }
}
