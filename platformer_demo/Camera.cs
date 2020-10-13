using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace platformer_demo
{
    public class Camera : IEntity
    {
        public Point Position { get; set; }
        public Point Size { get; set; }
        public Rectangle Rect { get { return new Rectangle(Position, Size); } set { } }

        private List<IEntity> _entities;
        public List<IEntity> Entities { get { return _entities; } }

        public Map BoundMap { get; set; }

        public Camera(Point cameraSize)
        {
            Size = cameraSize;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime) => OffsetDraw(spriteBatch, gameTime, Point.Zero);

        public void OffsetDraw(SpriteBatch spriteBatch, GameTime gameTime, Point offset)
        {
            if (_entities.Count > 0)
                foreach (IEntity entity in _entities)
                    entity.OffsetDraw(spriteBatch, gameTime, BoundMap.Position - Position + offset);
        }

        public void Initialize()
        {
            Position = new Point(0, 0);
            _entities = new List<IEntity>();
        }

        public void Load(ContentManager content)
        {
            
        }

        public void Update(GameTime gameTime)
        {
            _entities.Clear();
            if (BoundMap.Entities.Count > 0)
                foreach (IEntity entity in BoundMap.Entities)
                    if (entity.Rect.Bottom > Position.Y && entity.Rect.Top < Position.Y + Size.Y &&
                        entity.Rect.Right > Position.X && entity.Rect.Left < Position.X + Size.X)
                        _entities.Add(entity);
        }

        public static Point ScreenToCameraPosition(Camera camera, Map map, Point position)
            => position + map.Position + camera.Position;
    }
}
