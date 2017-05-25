using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Maps.Tiled;
using MonoGame.Extended.ViewportAdapters;
using System;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using System.Collections;
using ParticleEffects;
namespace Platformer
{
    
    public class Game1 : Game
    {
        public static Game1 current;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //player
        Player player = null;
        int score = 0;
        Emitter fireEmitter = null;
        Texture2D fireTexture = null;

        //game
        Camera2D camera = null;
        TiledMap map = null;
        public TiledTileLayer collisionLayer;
        public TiledTileLayer killLayer;
        Song gameMusic;

        //enemies
        List<Enemy> enemies = new List<Enemy>();

        //items
        List<Coin> coins = new List<Coin>();

        //HUD
        SpriteFont arialFont;
        Texture2D heart = null;

        

        

        // functions for screen width and height
        public int ScreenWidth
        {
            get
            {
                return graphics.GraphicsDevice.Viewport.Width;
            }
        }
        public int ScreenHeight
        {
            get
            {
                return graphics.GraphicsDevice.Viewport.Height;
            }
        }

        int viewWidth = 350;
        float deltaTime;
        int viewHeight = 280;
        // all constant definitions
        public static int tile = 16;
        public static float meter = tile;
        public static float gravity = meter * 9.8f * 4.0f;
        public static Vector2 maxVelocity = new Vector2(meter * 10, meter * 15);
        public static float acceleration = maxVelocity.X * 2;
        public static float friction = maxVelocity.X * 6;
        public static float jumpImpulse = meter * 1500;


        public Game1()
        {
            current = this;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.IsFixedTimeStep = false;
        }

        
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = 1080;
            graphics.PreferredBackBufferHeight = 720;
            graphics.ApplyChanges();

            // creates a new player
            // and passes it reference to this Game.1 object
            // so that the player object can access parts of the game object
            // so it can access those collision functions
            player = new Player(this);

            base.Initialize();
        }

        
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            player.Load(Content);

            LoadEmitter();           

            arialFont = Content.Load<SpriteFont>("Arial");

            heart = Content.Load<Texture2D>("Heart");

            var viewportAdapter = new BoxingViewportAdapter(GraphicsDevice, viewWidth, viewHeight);
            camera = new Camera2D(viewportAdapter);
            camera.Position = new Vector2(0, ScreenHeight/2);

            map = Content.Load<TiledMap>("PLS");         
            foreach (TiledTileLayer layer in map.TileLayers)
            {
                if (layer.Name == "Collisions")
                    collisionLayer = layer;
            }

            map = Content.Load<TiledMap>("PLS");
            foreach (TiledTileLayer layer in map.TileLayers)
            {
                if (layer.Name == "KillTiles")
                    killLayer = layer;
            }

            // game music 
            gameMusic = Content.Load<Song>("SuperHero_violin_no_Intro");
            MediaPlayer.Play(gameMusic);

            //loading enemies
            LoadAllEnemies();

            //loading items
            LoadAllCoins();
            

        }

        
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        private void LoadEmitter()
        {
            fireTexture = Content.Load<Texture2D>("spark");
            fireEmitter = new Emitter(fireTexture, player.Position);
            fireEmitter.gravity = 0f;
            fireEmitter.wind = 0f;
            fireEmitter.emissionSize = Vector2.Zero;
            fireEmitter.minSize = 5f;
            fireEmitter.maxSize = 0.2f;
            fireEmitter.minVelocity = Vector2.Zero;
            fireEmitter.maxVelocity = Vector2.Zero;
            fireEmitter.maxLife = 0.5f;
        }

        // loads all enemies
        private void LoadAllEnemies()
        {
            LoadEnemy(900, 208);
            LoadEnemy(1155, 144);
            LoadEnemy(1255, 80);
            LoadEnemy(1337, 80);
            LoadEnemy(1417, 80);
            LoadEnemy(1660, 176);
            LoadEnemy(1705, 176);
            LoadEnemy(1755, 176);
            LoadEnemy(1786, 176);
            LoadEnemy(1940, 144);
            LoadEnemy(140, 240);
            LoadEnemy(2180, 208);
        }

        // loads all coins
        private void LoadAllCoins()
        {
            LoadCoin(85, 256);
            LoadCoin(70, 256);
            LoadCoin(55, 256);
            LoadCoin(475, 240);
            LoadCoin(395, 240);
            LoadCoin(723, 32);
            LoadCoin(1185, 144);
            LoadCoin(1155, 32);
            LoadCoin(1265, 80);
            LoadCoin(1340, 80);
            LoadCoin(1420, 80);
            LoadCoin(1012, 80);
            LoadCoolCoin(140, 240);
            LoadCoolCoin(645, 32);
            LoadCoolCoin(661, 32);
            LoadCoolCoin(679, 32);
            LoadCoolCoin(1570, 32);
            LoadCoolCoin(1770, 176);
        }


        // loads one enemy
        private void LoadEnemy(int x, int y)
        {
            Enemy enemy = new Enemy(this);
            enemy.Load(Content);
            enemy.Position = new Vector2(x, y);
            enemies.Add(enemy);
        }

        // loads one standard coin
        private void LoadCoin(int x, int y)
        {
            Coin coin = new Platformer.Coin(this);
            coin.Load(Content);
            coin.Position = new Vector2(x, y);
            coins.Add(coin);
        }

        // loads one special coin
        private void LoadCoolCoin(int x, int y)
        {
            Coin coin = new Platformer.Coin(this);
            coin.Load(Content);
            coin.Position = new Vector2(x, y);
            coin.isLitFam = true;
            coins.Add(coin);
        }
        private void CheckEnemyCollisions()
        {
            foreach (Enemy e in enemies)
            {
                if (IsColliding(player.Bounds, e.Bounds) == true)
                {
                    if (player.IsJumping && player.Velocity.Y > 0)
                    {
                        player.JumpOnCollision();
                        enemies.Remove(e);
                        score += 30;
                        break;
                    }
                    else
                    {
                        // if player dies
                        // respawn player, reset score, remove all enemies, reload all enemies
                        player.Respawn();
                        score = 0;
                        enemies.Clear();
                        LoadAllEnemies();
                    }
                }
                
            }
        }

        private void CheckCoinCollisions()
        {
            foreach (Coin c in coins)
            {
                if (IsColliding(player.Bounds, c.Bounds) == true)
                {
                    if (c.isLitFam == true)
                    {
                        score += 50;
                        coins.Remove(c);
                        break;
                    }
                    else
                    {
                        score += 10;
                        coins.Remove(c);
                        break;
                    }
                }
            }
        }


        private bool IsColliding(Rectangle rect1, Rectangle rect2)
        {
            if (rect1.X + rect2.Width < rect2.X ||
                rect1.X> rect2.X + rect2.Width ||
                rect1.Y + rect1.Height < rect2.Y ||
                rect1.Y > rect2.Y + rect2.Height)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void UpdateEmitter(GameTime gameTime)
        {
            fireEmitter.position = new Vector2((player.Position.X + (player.Bounds.Width / 2)), (player.Position.Y + player.Bounds.Height + 1));
            
            fireEmitter.Update(gameTime);

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            player.Update(deltaTime);
            if (player.IsJumping == false && player.Velocity.X != 0)
            {
                UpdateEmitter(gameTime);
            }
            else
            {
                fireEmitter.Update(gameTime);
            }

            foreach (Enemy e in enemies)
            {
                e.Update(deltaTime);
            }
            foreach (Coin c in coins)
            {
                c.Update(deltaTime);
            } 

            // updates the camera so it follows the player
            // by always drawing from the player position
            // minus (eg towards the top left) half of the screen size
            camera.Position = player.Position - new Vector2(viewWidth / 2, viewHeight/2);

            //CheckEnemyCollisions();
            //CheckCoinCollisions();

            base.Update(gameTime);

        }

        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            

            // TODO: Add your drawing code here
            var t = camera.GetViewMatrix();
            map.Draw(camera); 
            
            spriteBatch.Begin(transformMatrix: t);
            

            //if (player.IsJumping == false && player.Velocity.X != 0)
            {
                fireEmitter.Draw(spriteBatch);
            }
            
            player.Draw(spriteBatch);
            foreach (Enemy e in enemies)
            {
                e.Draw(spriteBatch);
            }

            foreach (Coin c in coins)
            {
                c.Draw(spriteBatch);
            }

            spriteBatch.End();

            

            //          All GUI components below
            spriteBatch.Begin();
            spriteBatch.DrawString(arialFont, "Score : " + score.ToString(), new Vector2(30, 200), Color.White);

            for (int i = 0; i < player.lives; i++)
            {
                spriteBatch.Draw(heart, new Vector2(20 + i * 35, 20), null, Color.White, 0f, new Vector2(0,0), 1.35f, SpriteEffects.None, 0f);
            }
            spriteBatch.End();

            //All debugging outputs below
            //spriteBatch.Begin();
            //spriteBatch.DrawString(arialFont, "Position X = " + player.Position.X.ToString() + "Position Y = " + player.Position.Y.ToString(), new Vector2(50, 80), Color.Black);
            //spriteBatch.DrawString(arialFont, "Position X = " + player.Position.X.ToString() + "Position Y = " + player.Position.Y.ToString(), new Vector2(50, 120), Color.White);
            //spriteBatch.End();

            base.Draw(gameTime);
        }

        public int CellAtTileCoord(int tx, int ty, TiledTileLayer layer)
        {
            if (tx < 0 || tx >= map.Width || ty < 0)
                return 1;

            if (ty >= map.Height)
                return 0;

            TiledTile tile = layer.GetTile(tx, ty);
            return tile.Id;
        }

        public int PixelToTile(float pixelCoord)
        {
            return (int)Math.Floor(pixelCoord / tile);
        }

        public int TileToPixel(float tileCoord)
        {
            return (int)(tile * tileCoord);  
        }

        //public int CellAtPixelCoord(Vector2 pixelCoords)
        //{
        //    if (pixelCoords.X < 0 || pixelCoords.X > map.WidthInPixels || pixelCoords.Y < 0)
        //        return 1;
        //    // lets the player drop to the bottom of the screen == death

        //    if (pixelCoords.Y > map.HeightInPixels)
        //        return 0;

        //    return CellAtTileCoord(PixelToTile(pixelCoords.X), PixelToTile(pixelCoords.Y));
        //}

        
    }
}
