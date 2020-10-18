using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using platformer_demo.Character;
using platformer_demo.Environment;
using platformer_demo.Enums;

using System;

namespace platformer_demo
{
    /* TODO: 
     * Move input checks to dedicated class => DONE
     * Camera Zoom, 
     * Move a lot of things from Recto to Hero => DONE
     * Recto abilities, 
     * Adjust everything to Gamespeed, 
     * Map init through file, 
     * Map editor.
    */

    public class SimplePlatformer : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Resolution resolution = Resolution.HD;

        Map map;
        SquareObstacle obst1;
        SquareObstacle obst2;
        SquareObstacle obst3;
        SquareObstacle obst4;
        Recto recto;
        DebugOverlay debugOverlay;

        InputManager inputManager;
        double elapsedMiliseconds;
        int frameCounter;
        int updateCounter;
        DebugInfo debugInfo;

        public static Texture2D DefaultTexture;

        #region Default_Functions

        public SimplePlatformer()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true; 
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            SetResolution(resolution);

            map = new Map() { MapCamera = new Camera(GameParameters.Resolutions[resolution]) };
            map.MapCamera.BoundMap = map;
            obst1 = new SquareObstacle();
            obst2 = new SquareObstacle();
            obst3 = new SquareObstacle();
            obst4 = new SquareObstacle();
            recto = new Recto(map);
            debugOverlay = new DebugOverlay();

            inputManager = new InputManager();
            inputManager.BoundHero = recto;
            elapsedMiliseconds = 0;
            frameCounter = 0;
            updateCounter = 0;
            debugInfo = new DebugInfo();

            map.Initialize();
            AddObjectsToMap();
            map.InitializeEntities();
            obst2.SetValues(660, 520, 150, 180);
            obst3.SetValues(810, 650, 50, 50);
            obst4.SetValues(860, 600, 100, 100);
            debugOverlay.Initialize();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            DefaultTexture = Content.Load<Texture2D>("whiteSquare");

            map.Load(Content);
            debugOverlay.Load(Content);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
            inputManager.CaptureInput(gameTime);
            GameParameters.Update(gameTime);
            map.Update(gameTime);
            debugInfo.Update(recto, map, resolution, _graphics, GraphicsDevice.DisplayMode);
            elapsedMiliseconds += gameTime.ElapsedGameTime.TotalMilliseconds;
            debugOverlay.Update(debugInfo);

            if (elapsedMiliseconds > 1000)
                RefreshFPSVariables();

            ++updateCounter;        
        }
        protected override void Draw(GameTime gameTime)
        {
            const double maxTint = 0.5d;
            float tintCoeff = (float)((1 - maxTint) + GameParameters.GameSpeed * maxTint);
            Color bgColor = Color.LightBlue;
            GraphicsDevice.Clear(new Color(bgColor.R / 255f, bgColor.G * tintCoeff / 255, bgColor.B * tintCoeff / 255));

            // TODO: Add your drawing code here

            base.Draw(gameTime);

            _spriteBatch.Begin();
            map.Draw(_spriteBatch, gameTime);
            debugOverlay.Draw(_spriteBatch, gameTime);
            _spriteBatch.End();

            ++frameCounter;
        }

        #endregion Default_Functions

        #region My_Functions

        void SetResolution(Point resolution)
        {
            _graphics.PreferredBackBufferWidth = resolution.X;
            _graphics.PreferredBackBufferHeight = resolution.Y;
            _graphics.ApplyChanges();
        }

        void SetResolution(Resolution resolution)
        {
            if (resolution == Resolution.Fullscreen)
            { 
                _graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
                _graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
                _graphics.IsFullScreen = true;
                _graphics.ApplyChanges();
            }
            else
            {
                SetResolution(GameParameters.Resolutions[resolution]);
            }
        }

        void RefreshFPSVariables()
        {
            debugInfo.UpdateRenderInfo(frameCounter, updateCounter, elapsedMiliseconds);
            elapsedMiliseconds -= 1000;
            frameCounter = 0;
            updateCounter = 0;
        }

        void AddObjectsToMap()
        {
            map.AddEntity(obst1);
            map.AddEntity(obst2);
            map.AddEntity(obst3);
            map.AddEntity(obst4);

            map.AddEntity(recto);
        }

        #endregion My_Functions
    }
}
