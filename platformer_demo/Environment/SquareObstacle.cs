using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace platformer_demo.Environment
{
    class SquareObstacle : IEntity
    {
        private const string _asset_name_obstacle_texture = "obstacleSquare";
        private const string _assetsDirectory = "Environment";

        private Vector2 _position;
        public Vector2 Position { get { return _position; } set { SetValues(value, _size); } }

        private Point _size;
        public  Point Size { get { return _size; } set { SetValues(_position, value); } }

        private Rectangle _rect;
        public  Rectangle Rect { get { return _rect; } set { SetValues(value.X, value.Y, value.Width, value.Height); } }

        private Texture2D _texture;
        public  Texture2D Texture { get { return _texture; } set { } }

        public Point Center
        {
            get { return new Point(Rect.X + Size.X / 2, Rect.Y + Size.Y / 2); }
            set { SetValues(value.X - _size.X / 2, value.Y - _size.Y / 2, _size.X, _size.Y); }
        }

        public void SetValues(Vector2 position, Point size)
        {
            _position = position;
            _size = size;
            _rect = new Rectangle((int)Math.Round(position.X), (int)Math.Round(position.Y), size.X, size.Y);
        }

        public void SetValues(float x, float y, int width, int height) => SetValues(new Vector2(x, y), new Point(width, height));

        public void Initialize()
        {
            _size = new Point(600, 100);
            Position = new Vector2(10, 600);
        }

        public void Load(ContentManager content)
        {
            _texture = content.Load<Texture2D>(@$"{_assetsDirectory}\{_asset_name_obstacle_texture}");
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime) => OffsetDraw(spriteBatch, gameTime, Vector2.Zero);

        public void OffsetDraw(SpriteBatch spriteBatch, GameTime gameTime, Vector2 offset)
        {
            spriteBatch.OffsetDraw(
                _texture,
                _rect, offset,
                Color.White);
        }

        public void Update(GameTime gameTime)
        {

        }
    }
}
