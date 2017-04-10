using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Maps.Tiled;
using MonoGame.Extended.ViewportAdapters;

namespace Platformer
{
    
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Player player = new Player();

        Camera2D camera = null;
        TiledMap map = null;

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

            base.Initialize();
        }

        
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            player.Load(Content);

            var viewportAdapter = new BoxingViewportAdapter(GraphicsDevice, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height);

            camera = new Camera2D(viewportAdapter);
            camera.Position = new Vector2(0, 0/*graphics.GraphicsDevice.Viewport.Height*/);

            map = Content.Load<TiledMap>("Test3");
        }

        
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        public void UpdateCamera(float deltaTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Up) == true)
            {
                camera.Move(new Vector2(0, -200) * deltaTime);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Down) == true)
            {
                camera.Move(new Vector2(0, 200) * deltaTime);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Left) == true)
            {
                camera.Move(new Vector2(-200, 0) * deltaTime);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Right) == true)
            {
                camera.Move(new Vector2(200, 0) * deltaTime);
            }
        }

        
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            player.Update(deltaTime);
            UpdateCamera(deltaTime);

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
            
            base.Draw(gameTime);
        }
    }
}
