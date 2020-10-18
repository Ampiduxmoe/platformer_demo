using System;
using System.Collections.Generic;
using System.Text;

using platformer_demo.Enums;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace platformer_demo
{
    public class GameParameters
    {
        struct GameSpeedTranslationParams
        {
            public double InitialSpeed { get; set; }
            public double TargetSpeed { get; set; }
            public int FrameCount { get; private set; }
            public double Step { get; private set; }
            public int FramesPassed { get; set; }
            public bool Completed { get; set; }

            public GameSpeedTranslationParams(double targetSpeed, int frameCount)
            {
                InitialSpeed = GameSpeed;
                TargetSpeed = targetSpeed;
                FrameCount = frameCount;
                Step = (targetSpeed - InitialSpeed) / frameCount;
                FramesPassed = 0;
                Completed = false;
            }
        }

        public struct GameClock
        {
            public int Hours { get; private set; }
            public int Minutes { get; private set; }
            public int Seconds { get; private set; }
            public double Miliseconds { get; private set; }

            public GameClock(object o = null)
            {
                Hours = 0;
                Minutes = 0;
                Seconds = 0;
                Miliseconds = 0;
            }

            public void Update(GameTime gameTime)
            {
                Miliseconds += GetElapsedGameTime(gameTime) * 1000;
                if (Miliseconds > 1000)
                {
                    Seconds++;
                    Miliseconds -= 1000;
                }
                if (Seconds > 60)
                {
                    Minutes++;
                    Seconds -= 60;
                }
                if (Minutes > 60)
                {
                    Hours++;
                    Minutes -= 60;
                }
            }
        }

        public static Dictionary<Resolution, Point> Resolutions = new Dictionary<Resolution, Point>
        {
            { Resolution.Default, new Point(800, 600) },
            { Resolution.HD, new Point(1280, 720) },
            { Resolution.FullHD, new Point(1920, 1080) }
        };

        public static double GameSpeed = 1;

        public static double Gravity = 3000;

        public static float HeroAcceleration = 8000;

        public static float DefaultHeroMaxSpeed = 400;

        public static int HeroSideGapConst = 2;

        public static GameClock Clock;

        public static double GetElapsedGameTime(GameTime gameTime)
            => gameTime.ElapsedGameTime.TotalSeconds * GameSpeed;

        public static void Update(GameTime gameTime)
        {
            UpdateGameSpeed(gameTime);
            Clock.Update(gameTime);
        }

        public static void UpdateGameSpeed(GameTime gameTime)
        {
            if (gameSpeedTranslationTask.Completed == false)
            {
                GameSpeed += gameSpeedTranslationTask.Step;
                gameSpeedTranslationTask.FramesPassed++;
                if (gameSpeedTranslationTask.FramesPassed == gameSpeedTranslationTask.FrameCount)
                {
                    GameSpeed = gameSpeedTranslationTask.TargetSpeed;
                    gameSpeedTranslationTask.Completed = true;
                }
            }
        }

        private static GameSpeedTranslationParams gameSpeedTranslationTask;
        public static void TranslateGameSpeedSmoothly(double targetSpeed, int frameCount)
        {
            gameSpeedTranslationTask = new GameSpeedTranslationParams(targetSpeed, frameCount);
        }
    }
}
