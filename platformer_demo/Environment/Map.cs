using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace platformer_demo
{
    public class Map : IEntity
    {
        //TODO: background, foreground, layout, other objects

        private Point _position;
        public Point Position { get { return _position; } set { } }

        private List<IEntity> _entities;
        public List<IEntity> Entities { get { return _entities; } set { } }

        public Point Size { get { return GetSize(); } set { } }

        public Rectangle Rect { get { return new Rectangle(Position, Size); } set { } }

        public Camera MapCamera { get; set; }

        public void AddEntity(IEntity entity)
        {
            if (!_entities.Contains(entity))
                _entities.Add(entity);
            else throw new Exception("Duplicates are not allowed");
        }

        public Point GetSize()
        {
            //TODO
            return new Point();
        }

        public void InitializeEntities()
        {
            if (Entities.Count > 0)
                foreach (IEntity entity in _entities)
                    entity.Initialize();
        }

        public void Initialize()
        {
            _position = new Point(0, 0);
            _entities = new List<IEntity>();
            MapCamera.Initialize();
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime) => OffsetDraw(spriteBatch, gameTime, Point.Zero);

        public void OffsetDraw(SpriteBatch spriteBatch, GameTime gameTime, Point offset)
        {
            MapCamera.OffsetDraw(spriteBatch, gameTime, Point.Zero);
        }

        public void Load(ContentManager content)
        {
            if (Entities.Count > 0)
                foreach (IEntity entity in _entities)
                    entity.Load(content);
        }

        public void Update(GameTime gameTime)
        {
            if (Entities.Count > 0)
                foreach (IEntity entity in _entities)
                    entity.Update(gameTime);
            MapCamera.Update(gameTime);
        }
    }
}
