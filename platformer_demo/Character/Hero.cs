using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using platformer_demo.Environment;
using platformer_demo.Enums;
using platformer_demo.Interfaces;

using System;
using System.Collections.Generic;
using System.Text;

namespace platformer_demo.Character
{
    public abstract class Hero : Movable_MapEntity, IPenetratable
    {
        public Hero(Map map) : base(map)
        {
            _assetsDirectory = "Character";
            ASSET_NAME_CHARA_TEXTURE = "whiteRect";
            Color = Color.Green;
        }

        public override void Initialize()
        {
            Vector2 position = new Vector2(100, 200);
            Point size = new Point(40, 64);
            Vector2 speed = new Vector2(0, 0);
            SetValues(position, size, speed);
            State = ObjectState.Falling;
        }

        public override void Load(ContentManager content)
        {
            Texture = content.Load<Texture2D>(@$"{_assetsDirectory}\{ASSET_NAME_CHARA_TEXTURE}");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            UpdateCooldowns(gameTime);
            //CurrentMap.MapCamera.Center = Center;
        }

        public bool DrawHero = true;
        public bool DrawOuterSides = false;
        public bool DrawCollisionRect = false;
        public bool DrawRay = false;
        public override void OffsetDraw(SpriteBatch spriteBatch, GameTime gameTime, Vector2 offset)
        {
            if (DrawHero)
                spriteBatch.OffsetDraw(
                    Texture,
                    Rect, offset,
                    Color);

            if (DrawCollisionRect)
            {
                spriteBatch.OffsetDraw(
                    Texture,
                    GetCollisionRect(), offset,
                    Color.HotPink);
            }

            if (DrawOuterSides)
            {
                spriteBatch.OffsetDraw(
                    Texture,
                    GetOuterRectSide(Direction.Up), offset,
                    Color.Black);
                spriteBatch.OffsetDraw(
                    Texture,
                    GetOuterRectSide(Direction.Down), offset,
                    Color.Black);
                spriteBatch.OffsetDraw(
                    Texture,
                    GetOuterRectSide(Direction.Left), offset,
                    Color.Black);
                spriteBatch.OffsetDraw(
                    Texture,
                    GetOuterRectSide(Direction.Right), offset,
                    Color.Black);
            }

            if (DrawRay)
            {
                List<Rectangle> recList = GetIntersectionMarkers();
                foreach (Rectangle rect in recList)
                    spriteBatch.OffsetDraw(
                        Texture,
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

        MonoGame.Extended.Ray2 ray = new MonoGame.Extended.Ray2();
        public bool RayIsControllable = false;
        private List<Rectangle> GetIntersectionMarkers()
        {
            ray = new MonoGame.Extended.Ray2(Center, Speed);
            if (RayIsControllable)
            {
                const int rayLength = 500;
                MouseState mouseState = Mouse.GetState();
                Vector2 cursorPosition = Camera.ScreenToCameraPosition(CurrentMap.MapCamera, CurrentMap, new Vector2(mouseState.Position.X, mouseState.Position.Y));
                float rad;
                if (cursorPosition.X - ray.Position.X != 0)
                    rad = (float)Math.Atan((float)-(cursorPosition.Y - ray.Position.Y) / (cursorPosition.X - ray.Position.X)) + ((cursorPosition.X - ray.Position.X) > 0 ? 0 : (float)Math.PI);
                else rad = cursorPosition.Y - ray.Position.Y > 0 ? (float)-Math.PI / 2 : (float)Math.PI / 2;
                ray.Direction = GetHeroToMouseDirection() * rayLength;
            }
            List<Rectangle> output = new List<Rectangle>();
            foreach (IEntity e in CurrentMap.Entities)
                if (e != this)
                {
                    float near = 0, far = 0;
                    if (ray.Crosses(e.Rect, out near, out far))
                    {
                        const int recSize = 6;
                        Point rectLocation = new Point(
                            (int)Math.Round(ray.Position.X + ray.Direction.X * near) - recSize / 2, 
                            (int)Math.Round(ray.Position.Y + ray.Direction.Y * near) - recSize / 2);
                        Point rectSize = new Point(recSize, recSize);
                        output.Add(new Rectangle(rectLocation, rectSize));

                        rectLocation = new Point(
                            (int)Math.Round(ray.Position.X + ray.Direction.X * far) - recSize / 2, 
                            (int)Math.Round(ray.Position.Y + ray.Direction.Y * far) - recSize / 2);
                        output.Add(new Rectangle(rectLocation, rectSize));
                    }
                }
            return output;
        }

        public Vector2 GetHeroToMouseDirection()
        {
            Point mouseScreenPos = Mouse.GetState().Position;
            Vector2 mousePos = Camera.ScreenToCameraPosition(CurrentMap.MapCamera, CurrentMap, new Vector2(mouseScreenPos.X, mouseScreenPos.Y));
            float rad;
            if (mousePos.X - Center.X != 0)
                rad = (float)Math.Atan((float)-(mousePos.Y - Center.Y) / (mousePos.X - Center.X)) + ((mousePos.X - Center.X) > 0 ? 0 : (float)Math.PI);
            else rad = mousePos.Y - Center.Y > 0 ? (float)-Math.PI / 2 : (float)Math.PI / 2;
            return new Vector2((float)Math.Cos(rad), -(float)Math.Sin(rad));
        }

        public double PrimarySkillCooldown { get; set; } = 1;
        public double PrimarySkillRemainingCooldown { get; protected set; } = 0;
        public abstract void PrimarySkill();

        public double SecondarySkillCooldown { get; set; } = 1;
        public double SecondarySkillRemainingCooldown { get; protected set; } = 0;
        public abstract void SecondarySkill();

        public double DamageSkillCooldown { get; set; } = 1;
        public double DamageSkillRemainingCooldown { get; protected set; } = 0;
        public abstract void DamageSkill();

        public double MovementSkillCooldown { get; set; } = 1;
        public double MovementSkillRemainingCooldown { get; protected set; } = 0;
        public abstract void MovementSkill();

        public virtual void Jump()
        {
            const int jumpForce = 800;
            if (State == ObjectState.Standing && CanMoveUp())
                ChangeSpeedSmoothly(new Vector2(Speed.X, -jumpForce), 3);
        }

        protected void UpdateCooldowns(GameTime gameTime)
        {
            double elapsedTime = GameParameters.GetElapsedGameTime(gameTime);

            if (PrimarySkillRemainingCooldown < 0)
                PrimarySkillRemainingCooldown = 0;
            else if (PrimarySkillRemainingCooldown > 0)
                PrimarySkillRemainingCooldown -= elapsedTime;

            if (SecondarySkillRemainingCooldown < 0)
                SecondarySkillRemainingCooldown = 0;
            else if (SecondarySkillRemainingCooldown > 0)
                SecondarySkillRemainingCooldown -= elapsedTime;

            if (DamageSkillRemainingCooldown < 0)
                DamageSkillRemainingCooldown = 0;
            else if (DamageSkillRemainingCooldown > 0)
                DamageSkillRemainingCooldown -= elapsedTime;

            if (MovementSkillRemainingCooldown < 0)
                MovementSkillRemainingCooldown = 0;
            else if (MovementSkillRemainingCooldown > 0)
                MovementSkillRemainingCooldown -= elapsedTime;
        }
    }
}
