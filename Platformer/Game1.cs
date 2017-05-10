using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Maps.Tiled;
using MonoGame.Extended.ViewportAdapters;
using System;
using Microsoft.Xna.Framework.Media;            

namespace Platformer
{
    
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Player player = null;

        Camera2D camera = null;
        TiledMap map = null;
        TiledTileLayer collisionLayer;

        SpriteFont arialFont;

        Texture2D heart = null;

        Song gameMusic;

        int score = 0;
        int lives = 5;

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


        // all constant definitions
        public static int tile = 64;
        public static float meter = tile;
        public static float gravity = meter * 9.8f * 4.0f;
        public static Vector2 maxVelocity = new Vector2(meter * 10, meter * 15);
        public static float acceleration = maxVelocity.X * 2;
        public static float friction = maxVelocity.X * 6;
        public static float jumpImpulse = meter * 1500;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = 1280;
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

            arialFont = Content.Load<SpriteFont>("Arial");

            heart = Content.Load<Texture2D>("Heart");

            var viewportAdapter = new BoxingViewportAdapter(GraphicsDevice, ScreenWidth, ScreenHeight);
            camera = new Camera2D(viewportAdapter);
            camera.Position = new Vector2(0, ScreenHeight);

            map = Content.Load<TiledMap>("Test3");         
            foreach (TiledTileLayer layer in map.TileLayers)
            {
                if (layer.Name == "Collisions")
                    collisionLayer = layer;
            }

            // game music 
            gameMusic = Content.Load<Song>("SuperHero_violin_no_Intro");
            MediaPlayer.Play(gameMusic);
        }

        
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

              
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            player.Update(deltaTime);

            // updates the camera so it follows the player
            // by always drawing from the player position
            // minus (eg towards the top left) half of the screen size
            camera.Position = player.Position - new Vector2(ScreenWidth / 2, ScreenHeight / 2);

            base.Update(gameTime);
        }

        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            

            // TODO: Add your drawing code here
            var t = camera.GetViewMatrix();
            map.Draw(camera); 

            spriteBatch.Begin(transformMatrix: t);
            player.Draw(spriteBatch);
            spriteBatch.End();

            //          All GUI components below
            spriteBatch.Begin();
            spriteBatch.DrawString(arialFont, "Score : " + score.ToString(), new Vector2(1180, 30), Color.White);

            for (int i = 0; i < lives; i++)
            {
                spriteBatch.Draw(heart, new Vector2(20 + i * 35, 20), null, Color.White, 0f, new Vector2(0,0), 1.35f, SpriteEffects.None, 0f);
            }
            spriteBatch.End();
            
            base.Draw(gameTime);
        }

        public int CellAtTileCoord(int tx, int ty)
        {
            if (tx < 0 || tx >= map.Width || ty < 0)
                return 1;

            if (ty >= map.Height)
                return 0;

            TiledTile tile = collisionLayer.GetTile(tx, ty);
            return tile.Id;
        }

        public int PixelToTile(float pixelCoord)
        {
            return (int)Math.Floor(pixelCoord / tile);
        }

        public int TileToPixel(float tileCoord)
        {
            return (int)(tile * tileCoord);  //MAKE SURE THIS IS CORRECT, BOTH CASTING THIS AS A INT AND MAKING SURE IT IS USING THE CORRECT TILE VARIABLE (is it the one in the above function, or the value 64 at the top of this code)
        }

        public int CellAtPixelCoord(Vector2 pixelCoords)
        {
            if (pixelCoords.X < 0 || pixelCoords.X > map.WidthInPixels || pixelCoords.Y < 0)
                return 1;
            // lets the player drop to the bottom of the screen == death

            if (pixelCoords.Y > map.HeightInPixels)
                return 0;

            return CellAtTileCoord(PixelToTile(pixelCoords.X), PixelToTile(pixelCoords.Y));
        }

        
    }
}
