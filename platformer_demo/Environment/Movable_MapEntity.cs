using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using platformer_demo.Character;
using platformer_demo.Environment;
using platformer_demo.Interfaces;
using platformer_demo.Enums;

using System;
using System.Collections.Generic;
using System.Text;

namespace platformer_demo.Environment
{
    public abstract class Movable_MapEntity : IEntity, IMovable
    {
        struct SpeedChangeParams
        {
            public Vector2 InitialSpeed { get; set; }
            public Vector2 TargetSpeed { get; set; }
            public int FrameCount { get; private set; }
            public Vector2 Step { get; private set; }
            public int FramesPassed { get; set; }
            public bool Completed { get; set; }

            public SpeedChangeParams(Vector2 initialSpeed, Vector2 targetSpeed, int frameCount)
            {
                InitialSpeed = initialSpeed;
                TargetSpeed = targetSpeed;
                FrameCount = frameCount;
                Step = (targetSpeed - initialSpeed) / frameCount;
                FramesPassed = 0;
                Completed = false;
            }
        }

        protected string _assetsDirectory { get; set; }
        public string ASSET_NAME_CHARA_TEXTURE { get; protected set; }

        public Texture2D Texture { get; protected set; }

        public Color Color { get; protected set; } = Color.White;

        private Vector2 _position;
        public Vector2 Position { get { return _position; } set { SetValues(value, _size, _speed); } }

        private Point _size;
        public Point Size { get { return _size; } set { SetValues(_position, value, _speed); } }

        private Rectangle _rect;
        public Rectangle Rect { get { return _rect; } set { SetValues(value.X, value.Y, value.Width, value.Height, _speed); } }

        private Vector2 _speed;
        public Vector2 Speed { get { return _speed; } set { SetValues(_position, _size, value); } }

        public Point Center 
        { 
            get { return new Point(Rect.X + Size.X / 2, Rect.Y + Size.Y / 2); } 
            set { SetValues(value.X - _size.X / 2, value.Y - _size.Y / 2, _size.X, _size.Y, _speed); } 
        }

        public ObjectState State { get; protected set; }

        public bool AffectedByGravity { get; set; } = true;

        public Map CurrentMap { get; protected set; }

        protected void SetValues(Vector2 position, Point size, Vector2 speed)
        {
            if (position != _position)
                _position = position;
            if (size != _size)
                _size = size;
            if (speed != _speed)
                _speed = speed;
            _rect = new Rectangle((int)Math.Round(position.X), (int)Math.Round(position.Y), size.X, size.Y);
        }

        protected void SetValues(float x, float y, int width, int height, Vector2 speed) =>
            SetValues(new Vector2(x, y), new Point(width, height), speed);

        public Movable_MapEntity(Map map) => RebindMap(map);

        public void RebindMap(Map map) => CurrentMap = map;

        public virtual void Move(GameTime gameTime)
        {
            Vector2 prevPos = Position;
            Move(gameTime, Speed);

            if (IsClipping())
            {
                Position = prevPos;

                Point topLeft = new Point(Rect.Left, Rect.Top);
                Point topRight = new Point(Rect.Right, Rect.Top);
                Point bottomLeft = new Point(Rect.Left, Rect.Bottom);
                Point bottomRight = new Point(Rect.Right, Rect.Bottom);

                float nearest = 0;
                if (Speed.X == 0)
                {
                    nearest = Speed.Y > 0 ?
                        Math.Min(GetNearestCollisionDistance(bottomLeft, Speed), GetNearestCollisionDistance(bottomRight, Speed)) :
                        Math.Min(GetNearestCollisionDistance(topLeft, Speed), GetNearestCollisionDistance(topRight, Speed));
                }

                else if (Speed.Y == 0)
                {
                    nearest = Speed.X > 0 ?
                        Math.Min(GetNearestCollisionDistance(topRight, Speed), GetNearestCollisionDistance(bottomRight, Speed)) :
                        Math.Min(GetNearestCollisionDistance(topLeft, Speed), GetNearestCollisionDistance(bottomLeft, Speed));
                }

                else if (Speed.X > 0)
                {
                    nearest = Math.Min(Math.Min(
                        GetNearestCollisionDistance(topRight, Speed),
                        GetNearestCollisionDistance(bottomRight, Speed)),
                        GetNearestCollisionDistance(Speed.Y > 0 ? bottomLeft : topLeft, Speed));
                }

                else if (Speed.X < 0)
                {
                    nearest = Math.Min(Math.Min(
                        GetNearestCollisionDistance(topLeft, Speed),
                        GetNearestCollisionDistance(bottomLeft, Speed)),
                        GetNearestCollisionDistance(Speed.Y > 0 ? bottomRight : topRight, Speed));
                }

                Position = Position + Speed * nearest;
            }
        }

        protected virtual void Move(GameTime gameTime, Vector2 speed) =>
            Position = new Vector2(
                (float)(Position.X + speed.X * GameParameters.GetElapsedGameTime(gameTime)),
                (float)(Position.Y + speed.Y * GameParameters.GetElapsedGameTime(gameTime)));

        protected float GetNearestCollisionDistance(Point startingPoint, Vector2 direction)
        {
            float nearest = float.MaxValue;
            float near, far;
            MonoGame.Extended.Ray2 ray = new MonoGame.Extended.Ray2(
                new MonoGame.Extended.Point2(startingPoint.X, startingPoint.Y),
                direction);

            foreach (IEntity entity in CurrentMap.Entities)
                if (entity != this)
                    if (ray.Crosses(entity.Rect, out near, out far) && near != 0 && near < nearest)
                        nearest = near;
            return nearest;
        }

        protected int _collisionRectGap = 0;
        public virtual Rectangle GetCollisionRect() => new Rectangle(
            Rect.X + _collisionRectGap,
            Rect.Y + _collisionRectGap,
            Size.X - _collisionRectGap * 2,
            Size.Y - _collisionRectGap * 2);

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime) => OffsetDraw(spriteBatch, gameTime, Vector2.Zero);

        public virtual void OffsetDraw(SpriteBatch spriteBatch, GameTime gameTime, Vector2 offset)
        {
            spriteBatch.OffsetDraw(
                Texture,
                Rect, offset,
                Color);
        }

        public abstract void Initialize();

        public virtual void Load()
        {
            Texture = SimplePlatformer.DefaultTexture;
        }

        public virtual void Load(ContentManager content)
        {
            Texture = content.Load<Texture2D>(@$"{_assetsDirectory}\{ASSET_NAME_CHARA_TEXTURE}");
        }

        public virtual void Update(GameTime gameTime)
        {
            State = IsClipping() ? ObjectState.Clipping : CanMoveDown() ? ObjectState.Falling : ObjectState.Standing;

            UpdateSpeed();

            switch (State)
            {
                case ObjectState.Falling:
                    if (AffectedByGravity)
                        Speed = new Vector2(Speed.X, (float)(Speed.Y + GameParameters.Gravity * GameParameters.GetElapsedGameTime(gameTime)));
                    break;
                case ObjectState.Clipping:
                    PushUntilNoCollisions();
                    break;
            }

            if (State == ObjectState.Standing || State == ObjectState.Falling)
            {
                if (!CanMoveLeft() && Speed.X < 0 || !CanMoveRight() && Speed.X > 0)
                    Speed = new Vector2(0, Speed.Y);
                if (!CanMoveUp() && Speed.Y < 0 || !CanMoveDown() && Speed.Y > 0)
                    Speed = new Vector2(Speed.X, 0);

                Move(gameTime);
            }
        }

        public bool CanMoveUp() => !ClipsWith(GetOuterRectSide(Direction.Up));
        public bool CanMoveDown() => !ClipsWith(GetOuterRectSide(Direction.Down));
        public bool CanMoveLeft() => !ClipsWith(GetOuterRectSide(Direction.Left));
        public bool CanMoveRight() => !ClipsWith(GetOuterRectSide(Direction.Right));
        public bool IsClipping() => ClipsWith(GetCollisionRect());

        protected virtual bool ClipsWith(Rectangle heroRect)
        {
            foreach (IEntity entity in CurrentMap.Entities)
                if (entity != this && !(entity is IPenetratable))
                    if (entity.Rect.Contains(heroRect) || entity.Rect.Intersects(heroRect))
                        return true;
            return false;
        }

        public Rectangle GetOuterRectSide(Direction direction, int thickness = 1)
        {
            switch (direction)
            {
                case Direction.Up:
                    return new Rectangle(Rect.X, Rect.Y - thickness, Size.X, thickness);
                case Direction.Down:
                    return new Rectangle(Rect.X, Rect.Y + Size.Y, Size.X, thickness);
                case Direction.Left:
                    return new Rectangle(Rect.X - thickness, Rect.Y, thickness, Size.Y);
                case Direction.Right:
                    return new Rectangle(Rect.X + Size.X, Rect.Y, thickness, Size.Y);
                default:
                    throw new Exception("Unknown direction");
            }
        }

        protected void PushUntilNoCollisions()
        {

        }

        SpeedChangeParams speedChangeTask;
        public void ChangeSpeedSmoothly(Vector2 targetSpeed, int frameCount)
        {
            speedChangeTask = new SpeedChangeParams(Speed, targetSpeed, frameCount);
        }

        public void UpdateSpeed()
        {
            if (speedChangeTask.Completed == false)
            {
                Speed += speedChangeTask.Step;
                speedChangeTask.FramesPassed++;
                if (speedChangeTask.FramesPassed == speedChangeTask.FrameCount)
                {
                    speedChangeTask.Completed = true;
                }
            }
        }
    }
}