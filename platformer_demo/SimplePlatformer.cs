using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using platformer_demo.Character;
using platformer_demo.Environment;

using System;

namespace platformer_demo
{
    /* TODO: 
     * Move input checks to dedicated class
     * Camera Zoom, 
     * Move a lot of things from Recto to Hero, 
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

        KeyboardState keyboardState;
        MouseState mouseState;
        double elapsedMiliseconds;
        int frameCounter;
        int updateCounter;
        DebugInfo debugInfo;

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

            map.Load(Content);
            debugOverlay.Load(Content);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
            CaptureCharacterInput(gameTime);
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
            GraphicsDevice.Clear(Color.LightBlue);

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

        Vector2 speedChange;
        int kbcd = 0;
        int cd = 15;
        Point dragStart;
        Point dragEnd;
        Point oldCameraPos;
        bool mouseDrag = false;
        void CaptureCharacterInput(GameTime gameTime)
        {
            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();

            if (keyboardState.IsKeyDown(Keys.D))
            {
                if (recto.CanMoveRight())
                    speedChange = new Vector2((float)(GameParameters.HeroAcceleration * gameTime.ElapsedGameTime.TotalSeconds * GameParameters.GameSpeed), 0);
                else speedChange = new Vector2(0, 0);

                if (recto.Speed.X + speedChange.X > GameParameters.DefaultHeroMaxSpeed)
                    recto.Speed = new Vector2(GameParameters.DefaultHeroMaxSpeed, recto.Speed.Y);
                else
                    recto.Speed = new Vector2(recto.Speed.X + speedChange.X, recto.Speed.Y);
            }

            if (keyboardState.IsKeyDown(Keys.A))
            {
                if (recto.CanMoveLeft())
                    speedChange = new Vector2((float)(-GameParameters.HeroAcceleration * gameTime.ElapsedGameTime.TotalSeconds * GameParameters.GameSpeed), 0);
                else speedChange = new Vector2(0, 0);

                if (recto.Speed.X + speedChange.X < -GameParameters.DefaultHeroMaxSpeed)
                    recto.Speed = new Vector2(-GameParameters.DefaultHeroMaxSpeed, recto.Speed.Y);
                else
                    recto.Speed = new Vector2(recto.Speed.X + speedChange.X, recto.Speed.Y);
            }

            if (keyboardState.IsKeyDown(Keys.Space))
            {
                if (recto.State == HeroState.Standing && recto.CanMoveUp())
                    recto.Speed = new Vector2(recto.Speed.X, -700);
            }

            if (keyboardState.IsKeyDown(Keys.D) == keyboardState.IsKeyDown(Keys.A) && recto.Speed.X != 0)
            {
                speedChange = recto.Speed.X > 0 ?
                    new Vector2((float)(-GameParameters.HeroAcceleration * gameTime.ElapsedGameTime.TotalSeconds * GameParameters.GameSpeed), 0):
                    new Vector2((float)(GameParameters.HeroAcceleration * gameTime.ElapsedGameTime.TotalSeconds * GameParameters.GameSpeed), 0);

                if (Math.Abs(recto.Speed.X) - Math.Abs(speedChange.X) < 0)
                    recto.Speed = new Vector2(0, recto.Speed.Y);
                else
                    recto.Speed = new Vector2(recto.Speed.X + speedChange.X, recto.Speed.Y);
            }

            if (keyboardState.IsKeyDown(Keys.F1)) { if (kbcd >= cd) { recto.DrawHero = recto.DrawHero ? false : true; kbcd = 0; } }
            if (keyboardState.IsKeyDown(Keys.F2)) { if (kbcd >= cd) { recto.DrawCollisionRect = recto.DrawCollisionRect ? false : true; kbcd = 0; } }
            if (keyboardState.IsKeyDown(Keys.F3)) { if (kbcd >= cd) { recto.DrawOuterSides = recto.DrawOuterSides ? false : true; kbcd = 0; } }
            if (keyboardState.IsKeyDown(Keys.F4)) { if (kbcd >= cd) { recto.DrawRay = recto.DrawRay ? false : true; kbcd = 0; } }
            if (keyboardState.IsKeyDown(Keys.C)) { if (kbcd >= cd) { recto.RayIsControllable = recto.RayIsControllable ? false : true; kbcd = 0; } }
            if (keyboardState.IsKeyDown(Keys.R)) { if (kbcd >= cd) { recto.Initialize(); kbcd = 0; } }

            ++kbcd;

            if (mouseState.MiddleButton == ButtonState.Pressed && !mouseDrag)
            {
                oldCameraPos = map.MapCamera.Position;
                dragStart = mouseState.Position;
                mouseDrag = true;
            }

            if (mouseState.MiddleButton == ButtonState.Released) 
                mouseDrag = false;

            if (mouseDrag)
            {
                dragEnd = mouseState.Position;
                map.MapCamera.Position = oldCameraPos - (dragEnd - dragStart);
            }
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
