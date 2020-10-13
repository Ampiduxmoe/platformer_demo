using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace platformer_demo
{
    public class GameParameters
    {
        public static Dictionary<Resolution, Point> Resolutions = new Dictionary<Resolution, Point>
        {
            { Resolution.Default, new Point(800, 600) },
            { Resolution.HD, new Point(1280, 720) },
            { Resolution.FullHD, new Point(1920, 1080) }
        };

        public static double GameSpeed = 1;

        public static double Gravity = 3000;

        public static float HeroAcceleration = 6000;

        public static float DefaultHeroMaxSpeed = 400;

        public static int HeroSideGapConst = 2;
    }
}
