using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace platformer_demo.Character
{
    class Recto : Hero
    {
        private const string _asset_name_chara_texture = "whiteRect";
        public override string ASSET_NAME_CHARA_TEXTURE => _asset_name_chara_texture;

        private Point _position;
        public override Point Position { get { return _position; } set { SetValues(value, _size, _speed); } }

        public override Point Center { get { return new Point(_position.X + _size.X / 2, _position.Y + _size.Y / 2); } }

        private Point _size;
        public override Point Size { get { return _size; } set { SetValues(_position, value, _speed); } }

        private Rectangle _rect;
        public override Rectangle Rect { get { return _rect; } set { SetValues(value.X, value.Y, value.Width, value.Height, _speed); } }

        private Vector2 _speed;
        public override Vector2 Speed { get { return _speed; } set { SetValues(_position, _size, value); } }

        private HeroState _state;
        public override HeroState State { get { return _state; } set { } }

        private Texture2D _texture;
        public override Texture2D Texture { get { return _texture; } set { } }

        private Color _heroColor = Color.Green;
        public Color HeroColor { get { return _heroColor; } set { } }

        private void SetValues(Point position, Point size, Vector2 speed)
        {
            _position = position;
            _size = size;
            _rect = new Rectangle(position, size);
            _speed = speed;
        }

        private void SetValues(int x, int y, int width, int height, Vector2 speed) => SetValues(new Point(x, y), new Point(width, height), speed);

        public Recto(Map map) : base(map) { }

        public override void Initialize()
        {
            _position = new Point(100, 200);
            _size = new Point(40, 64);
            _rect = new Rectangle(_position, _size);
            _speed = new Vector2(0, 0);
            _state = HeroState.Falling;
        }

        public override void Load(ContentManager content)
        {
            _texture = content.Load<Texture2D>(@$"{_assetsDirectory}\{_asset_name_chara_texture}");
        }

        public bool DrawHero = true;
        public bool DrawOuterSides = false;
        public bool DrawCollisionRect = false;
        public bool DrawRay = false;
        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime) => OffsetDraw(spriteBatch, gameTime, Point.Zero);

        public override void OffsetDraw(SpriteBatch spriteBatch, GameTime gameTime, Point offset)
        {
            if (DrawHero)
                spriteBatch.OffsetDraw(
                    _texture, 
                     _rect, offset, 
                    _heroColor);

            if (DrawCollisionRect)
            {
                spriteBatch.OffsetDraw(
                    _texture,
                    GetCollisionRect(), offset,
                    Color.HotPink);
            }

            if (DrawOuterSides)
            {
                spriteBatch.OffsetDraw(
                    _texture,
                    GetOuterRectSide(Direction.Up), offset,
                    Color.Black);
                spriteBatch.OffsetDraw(
                    _texture,
                    GetOuterRectSide(Direction.Down), offset,
                    Color.Black);
                spriteBatch.OffsetDraw(
                    _texture,
                    GetOuterRectSide(Direction.Left), offset,
                    Color.Black);
                spriteBatch.OffsetDraw(
                    _texture,
                    GetOuterRectSide(Direction.Right), offset,
                    Color.Black);
            }

            if (DrawRay)
            {
                List<Rectangle> recList = GetIntersectionMarkers();
                foreach (Rectangle rect in recList)
                    spriteBatch.OffsetDraw(
                        _texture,
                        rect, offset,
                        Color.DarkRed);

                spriteBatch.OffsetDrawLine(
                ray.Position.X,
                ray.Position.Y,
                ray.Position.X + ray.Direction.X / 5,
                ray.Position.Y + ray.Direction.Y / 5,
                offset,
                Color.Black);

                spriteBatch.OffsetDrawString(
                    DebugOverlay.Font,
                    $"ray.Crosses(): {recList.Count > 0} " +
                    $"\nray.Position: {ray.Position} " +
                    $"\nray.Direction: (X = {Math.Round(ray.Direction.X, 2)} Y = {Math.Round(ray.Direction.Y, 2)}) " +
                    $"\nIntersections: {recList.Count}", new Vector2(Rect.X, Rect.Y - 80),
                    offset,
                    Color.Black);
            }
        }

        public override void Update(GameTime gameTime)
        {
            _state = IsClipping() ? HeroState.Clipping : CanMoveDown() ? HeroState.Falling : HeroState.Standing;

            if (_state == HeroState.Standing || _state == HeroState.Falling)
            {
                if (!CanMoveLeft() && _speed.X < 0 || !CanMoveRight() && _speed.X > 0)
                    _speed = new Vector2(0, _speed.Y);
                if (!CanMoveUp() && _speed.Y < 0 || !CanMoveDown() && _speed.Y > 0)
                    _speed = new Vector2(_speed.X, 0);

                Move(gameTime);
            }

            switch (_state)
            {
                case HeroState.Falling:
                    _speed = new Vector2(_speed.X, (float)(_speed.Y + GameParameters.Gravity * gameTime.ElapsedGameTime.TotalSeconds));
                    break;
                case HeroState.Clipping:
                    PushUntilNoCollisions();
                    break;
            }
        }

        public override void Move(GameTime gameTime)
        {
            Point prevPos = _position;
            Move(gameTime, _speed);

            if (IsClipping())
            {
                Position = prevPos;

                Point topLeft = new Point(_rect.Left, _rect.Top);
                Point topRight = new Point(_rect.Right, _rect.Top);
                Point bottomLeft = new Point(_rect.Left, _rect.Bottom);
                Point bottomRight = new Point(_rect.Right, _rect.Bottom);

                float nearest = 0;
                if (_speed.X == 0)
                {
                    nearest = _speed.Y > 0 ?
                        Math.Min(GetNearestCollisionDistance(bottomLeft, _speed), GetNearestCollisionDistance(bottomRight, _speed)) :
                        Math.Min(GetNearestCollisionDistance(topLeft, _speed), GetNearestCollisionDistance(topRight, _speed));
                }

                else if (_speed.Y == 0)
                {
                    nearest = _speed.X > 0 ?
                        Math.Min(GetNearestCollisionDistance(topRight, _speed), GetNearestCollisionDistance(bottomRight, _speed)) :
                        Math.Min(GetNearestCollisionDistance(topLeft, _speed), GetNearestCollisionDistance(bottomLeft, _speed));
                }

                else if (_speed.X > 0)
                {
                    nearest = Math.Min(Math.Min(
                        GetNearestCollisionDistance(topRight, _speed),
                        GetNearestCollisionDistance(bottomRight, _speed)),
                        GetNearestCollisionDistance(_speed.Y > 0 ? bottomLeft : topLeft, _speed));
                }

                else if (_speed.X < 0)
                {
                    nearest = Math.Min(Math.Min(
                        GetNearestCollisionDistance(topLeft, _speed),
                        GetNearestCollisionDistance(bottomLeft, _speed)),
                        GetNearestCollisionDistance(_speed.Y > 0 ? bottomRight : topRight, _speed));
                }

                Position = new Point((int)(_position.X + _speed.X * nearest), (int)(_position.Y + _speed.Y * nearest));
            }
        }

        private void Move(GameTime gameTime, Vector2 speed) =>
            Position = new Point(
                (int)(_position.X + speed.X * gameTime.ElapsedGameTime.TotalSeconds * GameParameters.GameSpeed), 
                (int)(_position.Y + speed.Y * gameTime.ElapsedGameTime.TotalSeconds * GameParameters.GameSpeed));

        public override bool CanMoveUp() => !ClipsWith(GetOuterRectSide(Direction.Up));
        public override bool CanMoveDown() => !ClipsWith(GetOuterRectSide(Direction.Down));
        public override bool CanMoveLeft() => !ClipsWith(GetOuterRectSide(Direction.Left));
        public override bool CanMoveRight() => !ClipsWith(GetOuterRectSide(Direction.Right));
        public bool IsClipping() => ClipsWith(GetCollisionRect());

        private bool ClipsWith(Rectangle heroRect)
        {
            foreach (IEntity entity in CurrentMap.Entities)
                if (entity != this)
                    if (entity.Rect.Contains(heroRect) || entity.Rect.Intersects(heroRect))
                        return true;
            return false;
        }

        private const int _collisionRectGap = 0;
        public override Rectangle GetCollisionRect() => new Rectangle(
            _position.X + _collisionRectGap, 
            _position.Y + _collisionRectGap, 
            _size.X - _collisionRectGap * 2, 
            _size.Y - _collisionRectGap * 2);


        public Rectangle GetOuterRectSide(Direction direction, int thickness = 1)
        {
            switch (direction)
            {
                case Direction.Up:
                        return new Rectangle(_position.X, _position.Y - thickness, _size.X, thickness);
                case Direction.Down:
                        return new Rectangle(_position.X, _position.Y + _size.Y, _size.X, thickness);
                case Direction.Left:
                        return new Rectangle(_position.X - thickness, _position.Y, thickness, _size.Y);
                case Direction.Right:
                        return new Rectangle(_position.X + _size.X, _position.Y, thickness, _size.Y);
                default:
                    throw new Exception("Unknown direction");
            }
        }

        private void PushUntilNoCollisions()
        {
            
        }

        private float GetNearestCollisionDistance(Point startingPoint, Vector2 direction)
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

        MonoGame.Extended.Ray2 ray = new MonoGame.Extended.Ray2();
        public bool RayIsControllable = false;
        private List<Rectangle> GetIntersectionMarkers()
        {

            ray = new MonoGame.Extended.Ray2(Center, _speed);
            if (RayIsControllable)
            {
                const int rayLength = 500;
                MouseState mouseState = Mouse.GetState();
                Point cursorPosition = Camera.ScreenToCameraPosition(CurrentMap.MapCamera, CurrentMap, mouseState.Position);
                float rad;
                if (cursorPosition.X - ray.Position.X != 0)
                    rad = (float)Math.Atan((float)-(cursorPosition.Y - ray.Position.Y) / (cursorPosition.X - ray.Position.X)) + ((cursorPosition.X - ray.Position.X) > 0 ? 0 : (float)Math.PI);
                else rad = cursorPosition.Y - ray.Position.Y > 0 ? (float)-Math.PI / 2 : (float)Math.PI / 2;
                ray.Direction = new Vector2(rayLength * (float)Math.Cos(rad), rayLength * -(float)Math.Sin(rad));
            }
            List<Rectangle> output = new List<Rectangle>();
            foreach (IEntity e in CurrentMap.Entities)
                if (e != this)
                {
                    float near = 0, far = 0;
                    MonoGame.Extended.Point2 center = new MonoGame.Extended.Point2(e.Rect.X + e.Rect.Width / 2, e.Rect.Y + e.Rect.Height / 2);
                    MonoGame.Extended.Size2 size = new MonoGame.Extended.Size2(e.Rect.Width / 2, e.Rect.Height / 2);
                    MonoGame.Extended.BoundingRectangle boundingRectangle = new MonoGame.Extended.BoundingRectangle(center, size);
                    if (ray.Crosses(boundingRectangle, out near, out far))
                    {
                        const int recSize = 6;
                        Point rectLocation = new Point((int)(ray.Position.X + ray.Direction.X * near) - recSize / 2, (int)(ray.Position.Y + ray.Direction.Y * near) - recSize / 2);
                        Point rectSize = new Point(recSize, recSize);
                        output.Add(new Rectangle(rectLocation, rectSize));

                        rectLocation = new Point((int)(ray.Position.X + ray.Direction.X * far) - recSize / 2, (int)(ray.Position.Y + ray.Direction.Y * far) - recSize / 2);
                        output.Add(new Rectangle(rectLocation, rectSize));
                    }
                }
            return output;
        }

        //private Point FindIntersection(Point a1, Point a2, Point b1, Point b2)
        //{
        //    int denominator = (a1.X - a2.X) * (b1.Y - b2.Y) - (a1.Y - a2.Y) * (b1.X - b2.X);
        //    return new Point(
        //        ((a1.X * a2.Y - a1.Y * a2.X) * (b1.X - b2.X) - (a1.X - a2.X) * (b1.X * b2.Y - b1.Y * b2.X)) / denominator,
        //        ((a1.X * a2.Y - a1.Y * a2.X) * (b1.Y - b2.Y) - (a1.Y - a2.Y) * (b1.X * b2.Y - b1.Y * b2.X)) / denominator);
        //}

        //private bool AreParallel(Point a1, Point a2, Point b1, Point b2)
        //    => (a1.X - a2.X) * (b1.Y - b2.Y) - (a1.Y - a2.Y) * (b1.X - b2.X) == 0;

        public override void DamageSkill()
        {

        }

        public override void MovementSkill()
        {

        }

        public override void PrimarySkill()
        {

        }

        public override void SecondarySkill()
        {
            
        }
    }
}
