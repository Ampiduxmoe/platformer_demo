using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace platformer_demo.Environment
{
    public class Map : IEntity
    {
        //TODO: background, foreground, layout, other objects

        private Vector2 _position;
        public Vector2 Position { get { return _position; } set { } }

        private List<IEntity> _entities;
        public List<IEntity> Entities { get { return _entities; } set { } }

        public Point Size { get { return GetSize(); } set { } }

        public Rectangle Rect { get { return new Rectangle((int)Math.Round(Position.X), (int)Math.Round(Position.Y), Size.X, Size.Y); } set { } }

        public Point Center
        {
            get { return new Point(Rect.X + Size.X / 2, Rect.Y + Size.Y / 2); }
            set { Position = new Vector2(value.X - Size.X / 2, value.Y - Size.Y / 2); }
        }

        public Camera MapCamera { get; set; }

        private Queue<IEntity> _deletionQueue;

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
            _position = new Vector2(0, 0);
            _entities = new List<IEntity>();
            _deletionQueue = new Queue<IEntity>();
            MapCamera.Initialize();
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime) => OffsetDraw(spriteBatch, gameTime, Vector2.Zero);

        public void OffsetDraw(SpriteBatch spriteBatch, GameTime gameTime, Vector2 offset)
        {
            MapCamera.OffsetDraw(spriteBatch, gameTime, offset);
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
                {
                    entity.Update(gameTime);
                    if (entity is Projectile)
                    {
                        Projectile projectile = entity as Projectile;
                        if (projectile.TimeElapsed > projectile.LifeLength)
                            _deletionQueue.Enqueue(entity);
                    }
                }

            while (_deletionQueue.Count > 0)
                Entities.Remove(_deletionQueue.Dequeue());

            MapCamera.Update(gameTime);
        }
    }
}
