﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Platformer
{
    class Player
    {
        Sprite sprite = new Sprite();
        float playerSpeed;

        public Player()
        {
            playerSpeed = 20f;
        }

        public void Load(ContentManager content)
        {
            sprite.Load(content, "hero");
        }

        public void Update(float deltaTime)
        {
            sprite.Update(deltaTime);

            if (Keyboard.GetState().IsKeyDown(Keys.D) == true)
            {
                sprite.position.X += playerSpeed;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.A) == true)
            {
                sprite.position.X -= playerSpeed;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch);
        }
    }
}