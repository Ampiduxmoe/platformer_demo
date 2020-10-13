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

        private Point _position;
        public  Point Position { get { return _position; } set { SetValues(value, _size); } }

        private Point _size;
        public  Point Size { get { return _size; } set { SetValues(_position, value); } }

        private Rectangle _rect;
        public  Rectangle Rect { get { return _rect; } set { SetValues(value.X, value.Y, value.Width, value.Height); } }

        private Texture2D _texture;
        public  Texture2D Texture { get { return _texture; } set { } }

        public void SetValues(Point position, Point size)
        {
            _position = position;
            _size = size;
            _rect = new Rectangle(position.X, position.Y, size.X, size.Y);
        }

        public void SetValues(int x, int y, int width, int height) => SetValues(new Point(x, y), new Point(width, height));

        public void Initialize()
        {
            _position = new Point(10, 600);
            _size = new Point(600, 100);
            _rect = new Rectangle(_position.X, _position.Y, _size.X, _size.Y);
        }

        public void Load(ContentManager content)
        {
            _texture = content.Load<Texture2D>(@$"{_assetsDirectory}\{_asset_name_obstacle_texture}");
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime) => OffsetDraw(spriteBatch, gameTime, Point.Zero);

        public void OffsetDraw(SpriteBatch spriteBatch, GameTime gameTime, Point offset)
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
