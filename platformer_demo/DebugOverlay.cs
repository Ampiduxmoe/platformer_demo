using System;
using System.Collections.Generic;
using System.Text;

using platformer_demo.Enums;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace platformer_demo.Character
{
    class DebugOverlay : IEntity
    {
        private const string _asset_name_font = "simpleFont";
        private const string _assetsDirectory = "Content";
        private static SpriteFont _font;
        public static SpriteFont Font { get { return _font; } }
        private Color _defaultDebugColor = Color.DarkRed;

        private static Vector2 _overlayPosition;

        private Point _graphicsDebugPosition;
        private Color _graphicsDebugColor;
        private Vector2 _graphicsDebugMeasure;
        private Point _resolution;
        private string _resolutionSet;
        private string _resolutinRendered;
        private string _deviceResolution;
        private string _fps;
        private string _graphicsDebugText;

        private Point _gameInfoDebugPosition;
        private Color _gameInfoDebugColor;
        private Vector2 _gameInfoDebugMeasure;
        private string _updateFrequency;
        private string _gameTime;
        private string _gameSpeed;
        private string _gameInfoDebugText;

        private Point _heroDebugPosition;
        private Color _heroDebugColor;
        private Vector2 _heroDebugMeasure;
        private Hero _hero;
        private string _heroPosition;
        private string _heroSpeed;
        private string _heroSize;
        private string _state;
        private string _canMove;
        private string _heroDebugText;
        
        private Point _mapInfoDebugPosition;
        private Color _mapInfoDebugColor;
        private Vector2 _mapInfoDebugMeasure;
        private string _mapPosition;
        private string _mapEntityCount;
        private string _mapInfoDebugText;

        private Point _cameraInfoDebugPosition;
        private Color _cameraInfoDebugColor;
        private Vector2 _cameraInfoDebugMeasure;
        private string _cameraPosition;
        private string _cameraEntityCount;
        private string _cameraInfoDebugText;

        public Rectangle Rect { get { return new Rectangle((int)Math.Round(Position.X), (int)Math.Round(Position.Y), Size.X, Size.Y); } set { } }

        public Point Center 
        { 
            get { return new Point(Rect.X + Size.X / 2, Rect.Y + Size.Y / 2); } 
            set { Position = new Vector2(value.X - Size.X / 2, value.Y - Size.Y / 2); } 
        }

        public Vector2 Position { get { return _overlayPosition; } set { _overlayPosition = value; } }

        public Point Size 
        {
            get 
            {
                _graphicsDebugMeasure = _font.MeasureString(_graphicsDebugText);
                _gameInfoDebugMeasure = _font.MeasureString(_gameInfoDebugText);
                _heroDebugMeasure = _font.MeasureString(_heroDebugText);
                _mapInfoDebugMeasure = _font.MeasureString(_mapInfoDebugText);
                _cameraInfoDebugMeasure = _font.MeasureString(_cameraInfoDebugText);

                return new Point(
                    (int)Math.Round(_cameraInfoDebugMeasure.X + _cameraInfoDebugMeasure.X),
                    (int)Math.Max(Math.Max(Math.Max(Math.Max(
                        _graphicsDebugMeasure.Y, 
                        _gameInfoDebugMeasure.Y), 
                        _heroDebugMeasure.Y), 
                        _mapInfoDebugMeasure.Y),
                        _cameraInfoDebugMeasure.Y));
            }
            set { } 
        }

        public DebugOverlay() { }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime) => OffsetDraw(spriteBatch, gameTime, Vector2.Zero);

        public void OffsetDraw(SpriteBatch spriteBatch, GameTime gameTime, Vector2 offset)
        {
            spriteBatch.DrawString(
                _font,
                _graphicsDebugText,
                new Vector2(_overlayPosition.X + _graphicsDebugPosition.X + offset.X, _overlayPosition.Y + _graphicsDebugPosition.Y + offset.Y),
                _graphicsDebugColor);

            spriteBatch.DrawString(
                _font,
                _gameInfoDebugText,
                new Vector2(_overlayPosition.X + _gameInfoDebugPosition.X + offset.X, _overlayPosition.Y + _gameInfoDebugPosition.Y + offset.Y),
                _gameInfoDebugColor);

            spriteBatch.DrawString(
                _font,
                _heroDebugText,
                new Vector2(_overlayPosition.X + _heroDebugPosition.X + offset.X, _overlayPosition.Y + _heroDebugPosition.Y + offset.Y),
                _heroDebugColor);

            spriteBatch.DrawString(
                _font,
                _mapInfoDebugText,
                new Vector2(_overlayPosition.X + _mapInfoDebugPosition.X + offset.X, _overlayPosition.Y + _mapInfoDebugPosition.Y + offset.Y),
                _mapInfoDebugColor);

            spriteBatch.DrawString(
                _font,
                _cameraInfoDebugText,
                new Vector2(_overlayPosition.X + _cameraInfoDebugPosition.X + offset.X, _overlayPosition.Y + _cameraInfoDebugPosition.Y + offset.Y),
                _cameraInfoDebugColor);
        }

        public void Initialize()
        {
            _overlayPosition = new Vector2(10, 10);

            _graphicsDebugPosition = new Point(0, 0);
            _graphicsDebugColor = _defaultDebugColor;

            _gameInfoDebugPosition = new Point(250, 0);
            _gameInfoDebugColor = _defaultDebugColor;

            _heroDebugPosition = new Point(500, 0);
            _heroDebugColor = _defaultDebugColor;

            _mapInfoDebugPosition = new Point(750, 0);
            _mapInfoDebugColor = _defaultDebugColor;

            _cameraInfoDebugPosition = new Point(1000, 0);
            _cameraInfoDebugColor = _defaultDebugColor;
        }

        public void Load(ContentManager content)
        {
            _font = content.Load<SpriteFont>($@"{_asset_name_font}");
        }

        public void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public void Update(DebugInfo debugInfo)
        {
            _resolution = debugInfo.ResolutionSet == Resolution.Fullscreen ? 
                new Point(debugInfo.Display.Width, debugInfo.Display.Height) : GameParameters.Resolutions[debugInfo.ResolutionSet];
            _resolutionSet = $"Resolution: {debugInfo.ResolutionSet} ({_resolution.X}x{_resolution.Y})";
            _resolutinRendered = $"Actual Render: {debugInfo.Graphics.PreferredBackBufferWidth}x{debugInfo.Graphics.PreferredBackBufferHeight}";
            _deviceResolution = $"Device: {debugInfo.Display.Width}x{debugInfo.Display.Height}";
            _fps = debugInfo.FramesRendered == 0 ? "FPS: NaN" : $"FPS: {Math.Round(debugInfo.FramesRendered * 1000 / debugInfo.ElapsedMiliseconds, 2)}";
            _graphicsDebugText = $"<<GRAPHICS>> \n{_resolutionSet} \n{_resolutinRendered} \n{_deviceResolution} \n{_fps}";

            _updateFrequency = debugInfo.UpdatesProcessed == 0 ? 
                "Update Frequency: NaN" : $"Update Frequency: {Math.Round(debugInfo.UpdatesProcessed * 1000 / debugInfo.ElapsedMiliseconds, 2)} Hz";
            _gameTime = $"Time: {GameParameters.Clock.Hours}h {GameParameters.Clock.Minutes}m {GameParameters.Clock.Seconds}s {(GameParameters.Clock.Miliseconds / 10):f0}ms";
            _gameSpeed = $"Game Speed: {GameParameters.GameSpeed:f2}";
            _gameInfoDebugText = $"<<GAME>> \n{_updateFrequency} \n{_gameTime} \n{_gameSpeed}";

            _hero = debugInfo.CurrentHero;
            _heroPosition = $"Map: X = {_hero.Position.X:f2}, Y = {_hero.Position.Y:f2}";
            _heroSpeed = $"Speed: X = {(int)_hero.Speed.X}, Y = {(int)_hero.Speed.Y}";
            _heroSize = $"Size: X = {_hero.Size.X}, Y = {_hero.Size.Y}";
            _state = $"State: {_hero.State}";
            _canMove = $"Can Move: \n   Up({_hero.CanMoveUp().ToString()[0]}) Down({_hero.CanMoveDown().ToString()[0]}) \n   " +
                $"Left({_hero.CanMoveLeft().ToString()[0]}) Right({_hero.CanMoveRight().ToString()[0]})";
            _heroDebugText = $"<<HERO>> \n{_heroPosition} \n{_heroSpeed} \n{_heroSize} \n{_state} \n{_canMove}";

            _mapPosition = $"Position: X = {debugInfo.GameMap.Position.X}, Y = {debugInfo.GameMap.Position.Y}";
            _mapEntityCount = $"Entities: {debugInfo.GameMap.Entities.Count}";
            _mapInfoDebugText = $"<<MAP>> \n{_mapPosition} \n{_mapEntityCount}";

            _cameraPosition = $"Position: X = {debugInfo.GameMap.MapCamera.Position.X}, Y = {debugInfo.GameMap.MapCamera.Position.Y}";
            _cameraEntityCount = $"Entities: {debugInfo.GameMap.MapCamera.Entities.Count}";
            _cameraInfoDebugText = $"<<CAMERA>> \n{_cameraPosition} \n{_cameraEntityCount}";

        }
    }
}
