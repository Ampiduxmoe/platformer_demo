using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using platformer_demo.Environment;

using System;
using System.Collections.Generic;
using System.Text;

namespace platformer_demo
{
    public class Camera : IEntity
    {
        struct OffsetChangeParams
        {
            public Vector2 InitialPosition { get; set; }
            public Vector2 TargetPosition { get; set; }
            public int FrameCount { get; private set; }
            public Vector2 Step { get; private set; }
            public int FramesPassed { get; set; }
            public bool Completed { get; set; }

            public OffsetChangeParams(Vector2 initialPosition, Vector2 targetPosition, int frameCount)
            {
                InitialPosition = initialPosition;
                TargetPosition = targetPosition;
                FrameCount = frameCount;
                Step = (targetPosition - initialPosition) / frameCount;
                FramesPassed = 0;
                Completed = false;
            }
        }
        struct ShakeParams
        {
            public int Power { get; private set; }
            public int FlicksCount { get; private set; }
            public int FlickDuration { get; private set; }
            public int StepsPassed { get; set; }
            public bool Completed { get; set; }
            public bool Initialized { get; private set; }

            public ShakeParams(int power, int flicksCount, int flickDuration)
            {
                Power = power;
                FlicksCount = flicksCount;
                FlickDuration = flickDuration;
                StepsPassed = 0;
                Completed = false;
                Initialized = true;
            }
        }

        private Vector2 _internalOffset = Vector2.Zero;
        private float _offscreenRenderDistance = 0;
        public Vector2 Position { get; set; }
        public Point Size { get; set; }
        public Rectangle Rect { get { return new Rectangle((int)Math.Round(Position.X), (int)Math.Round(Position.Y), Size.X, Size.Y); } set { } }
        public Point Center 
        { 
            get { return new Point(Rect.X + Size.X / 2, Rect.Y + Size.Y / 2); } 
            set { Position = new Vector2(value.X - Size.X / 2, value.Y - Size.Y / 2); } 
        }

        public List<IEntity> Entities { get; private set; }

        public Map BoundMap { get; set; }

        public Camera(Point cameraSize)
        {
            Size = cameraSize;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime) => OffsetDraw(spriteBatch, gameTime, Vector2.Zero);

        public void OffsetDraw(SpriteBatch spriteBatch, GameTime gameTime, Vector2 offset)
        {
            if (Entities.Count > 0)
                foreach (IEntity entity in Entities)
                    entity.OffsetDraw(spriteBatch, gameTime, BoundMap.Position - Position + offset + _internalOffset);
        }

        public void Initialize()
        {
            Position = new Vector2(0, 0);
            Entities = new List<IEntity>();
        }

        public void Load(ContentManager content)
        {
            
        }

        public void Update(GameTime gameTime)
        {
            Entities.Clear();
            if (BoundMap.Entities.Count > 0)
                foreach (IEntity entity in BoundMap.Entities)
                    if (entity.Rect.Bottom > Position.Y && entity.Rect.Top < Position.Y + Size.Y &&
                        entity.Rect.Right > Position.X && entity.Rect.Left < Position.X + Size.X)
                        Entities.Add(entity);

            if (shakeTask.Initialized)
                UpdateOffset();
        }

        public static Vector2 ScreenToCameraPosition(Camera camera, Map map, Vector2 position)
            => position + map.Position + camera.Position;

        private ShakeParams shakeTask;
        private OffsetChangeParams offsetChangeTask;
        public void GenerateShake(int power, int flickCoint, int flickDuration)
        {
            if (shakeTask.Initialized && !shakeTask.Completed)
                shakeTask = new ShakeParams(shakeTask.Power + power, flickCoint, flickDuration);
            else
            {
                offsetChangeTask = new OffsetChangeParams(_internalOffset, GenerateOffset(power), flickDuration);
                shakeTask = new ShakeParams(power, flickCoint, flickDuration);
            }
        }

        Random rnd = new Random();
        private void UpdateOffset()
        {
            if (!offsetChangeTask.Completed)
            {
                _internalOffset += offsetChangeTask.Step;
                offsetChangeTask.FramesPassed++;
                if (offsetChangeTask.FramesPassed >= offsetChangeTask.FrameCount)
                {
                    offsetChangeTask.Completed = true;
                }
            }

            if (!shakeTask.Completed)
            {
                if (offsetChangeTask.Completed)
                    shakeTask.StepsPassed++;
                if (shakeTask.StepsPassed >= shakeTask.FlicksCount)
                {
                    shakeTask.Completed = true;
                    offsetChangeTask = new OffsetChangeParams(_internalOffset, Vector2.Zero, shakeTask.FlickDuration);
                }
                if (offsetChangeTask.Completed && !shakeTask.Completed)
                {
                    Vector2 newOffset;

                    do
                    {
                        newOffset = GenerateOffset(shakeTask.Power);
                    } while ((newOffset - _internalOffset).Length() < shakeTask.Power);

                    offsetChangeTask = new OffsetChangeParams(
                        _internalOffset,
                        GenerateOffset(shakeTask.Power),
                        shakeTask.FlickDuration);
                }
            }
        }

        private Vector2 GenerateOffset(int distanceFromZero)
        {
            return new Vector2(rnd.Next(distanceFromZero * 2) - distanceFromZero, rnd.Next(distanceFromZero * 2) - distanceFromZero);
        }
    }
}
