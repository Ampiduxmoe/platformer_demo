using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using platformer_demo.Character;
using platformer_demo.Enums;

using System;
using System.Collections.Generic;
using System.Text;

namespace platformer_demo
{
    public class InputManager
    {
        public Hero BoundHero { get; set; }

        public InputManager() { }

        Vector2 speedChange;
        int kbcd = 0;
        int cd = 15;
        Vector2 dragStart;
        Vector2 dragEnd;
        Vector2 oldCameraPos;
        bool mouseDrag = false;

        bool movementSkill = false;
        public void CaptureInput(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();

            if (keyboardState.IsKeyDown(Keys.D))
            {
                if (BoundHero.CanMoveRight())
                    speedChange = new Vector2((float)(GameParameters.HeroAcceleration * gameTime.ElapsedGameTime.TotalSeconds * GameParameters.GameSpeed), 0);
                else speedChange = new Vector2(0, 0);

                if (BoundHero.Speed.X < GameParameters.DefaultHeroMaxSpeed)
                {
                    BoundHero.Speed += speedChange;
                    if (BoundHero.Speed.X > GameParameters.DefaultHeroMaxSpeed)
                        BoundHero.Speed = new Vector2(GameParameters.DefaultHeroMaxSpeed, BoundHero.Speed.Y);
                }
                else if (BoundHero.State == ObjectState.Standing)
                    BoundHero.Speed -= speedChange;
            }

            if (keyboardState.IsKeyDown(Keys.A))
            {
                if (BoundHero.CanMoveLeft())
                    speedChange = new Vector2((float)(-GameParameters.HeroAcceleration * gameTime.ElapsedGameTime.TotalSeconds * GameParameters.GameSpeed), 0);
                else speedChange = new Vector2(0, 0);

                if (BoundHero.Speed.X > -GameParameters.DefaultHeroMaxSpeed)
                {
                    BoundHero.Speed += speedChange;
                    if (BoundHero.Speed.X < -GameParameters.DefaultHeroMaxSpeed)
                        BoundHero.Speed = new Vector2(-GameParameters.DefaultHeroMaxSpeed, BoundHero.Speed.Y);
                }
                else if (BoundHero.State == ObjectState.Standing)
                    BoundHero.Speed -= speedChange;
            }

            if (keyboardState.IsKeyDown(Keys.D) == keyboardState.IsKeyDown(Keys.A) && BoundHero.Speed.X != 0)
            {
                speedChange = BoundHero.Speed.X > 0 ?
                    new Vector2((float)(-GameParameters.HeroAcceleration * gameTime.ElapsedGameTime.TotalSeconds * GameParameters.GameSpeed), 0) :
                    new Vector2((float)(GameParameters.HeroAcceleration * gameTime.ElapsedGameTime.TotalSeconds * GameParameters.GameSpeed), 0);

                if (BoundHero.State == ObjectState.Falling)
                    speedChange /= 8;

                if (Math.Abs(BoundHero.Speed.X) - Math.Abs(speedChange.X) < 0)
                    BoundHero.Speed = new Vector2(0, BoundHero.Speed.Y);
                else
                    BoundHero.Speed = new Vector2(BoundHero.Speed.X + speedChange.X, BoundHero.Speed.Y);
            }

            if (keyboardState.IsKeyDown(Keys.Space))
            {
                BoundHero.Jump();
            }

            if (keyboardState.IsKeyDown(Keys.F1)) { if (kbcd >= cd) { BoundHero.DrawHero = BoundHero.DrawHero ? false : true; kbcd = 0; } }
            if (keyboardState.IsKeyDown(Keys.F2)) { if (kbcd >= cd) { BoundHero.DrawCollisionRect = BoundHero.DrawCollisionRect ? false : true; kbcd = 0; } }
            if (keyboardState.IsKeyDown(Keys.F3)) { if (kbcd >= cd) { BoundHero.DrawOuterSides = BoundHero.DrawOuterSides ? false : true; kbcd = 0; } }
            if (keyboardState.IsKeyDown(Keys.F4)) { if (kbcd >= cd) { BoundHero.DrawRay = BoundHero.DrawRay ? false : true; kbcd = 0; } }
            if (keyboardState.IsKeyDown(Keys.C)) { if (kbcd >= cd) { BoundHero.RayIsControllable = BoundHero.RayIsControllable ? false : true; kbcd = 0; } }
            if (keyboardState.IsKeyDown(Keys.R)) { if (kbcd >= cd) { BoundHero.Initialize(); kbcd = 0; } }

            if (mouseState.LeftButton == ButtonState.Pressed)
                BoundHero.PrimarySkill();
            if (keyboardState.IsKeyDown(Keys.LeftShift))
            {
                if (!movementSkill)
                {
                    GameParameters.TranslateGameSpeedSmoothly(0.33d, 15);
                    movementSkill = true;
                }
            }
            else if (movementSkill)
            {
                GameParameters.TranslateGameSpeedSmoothly(1d, 5);
                movementSkill = false;
                BoundHero.MovementSkill();
            }

            ++kbcd;

            if (mouseState.MiddleButton == ButtonState.Pressed && !mouseDrag)
            {
                oldCameraPos = BoundHero.CurrentMap.MapCamera.Position;
                dragStart = new Vector2(mouseState.Position.X, mouseState.Position.Y);
                mouseDrag = true;
            }

            if (mouseState.MiddleButton == ButtonState.Released)
                mouseDrag = false;

            if (mouseDrag)
            {
                dragEnd = new Vector2(mouseState.Position.X, mouseState.Position.Y);
                BoundHero.CurrentMap.MapCamera.Position = oldCameraPos - (dragEnd - dragStart);
            }
        }
    }
}
