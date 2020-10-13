using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using platformer_demo.Character;
using System;
using System.Collections.Generic;
using System.Text;

namespace platformer_demo
{
    class DebugInfo
    {
        public Hero CurrentHero { get; private set; }
        public Resolution ResolutionSet { get; private set; }
        public GraphicsDeviceManager Graphics { get; private set; }
        public DisplayMode Display { get; private set; }
        public int FramesRendered { get; private set; }
        public int UpdatesProcessed { get; private set; }
        public double ElapsedMiliseconds { get; private set; }
        public Map GameMap { get; private set; }

        public DebugInfo() { }

        public DebugInfo(Hero currentHero, Map map, Resolution resolutionSet, GraphicsDeviceManager graphics, DisplayMode displayMode) => 
            Update(currentHero, map, resolutionSet, graphics, displayMode);

        public void Update(Hero currentHero, Map map, Resolution resolutionSet, GraphicsDeviceManager graphics, DisplayMode displayMode)
        {
            CurrentHero = currentHero;
            ResolutionSet = resolutionSet;
            Graphics = graphics;
            Display = displayMode;
            GameMap = map;
        }

        public void UpdateRenderInfo(int framesRendered, int updatesProcessed, double elapsedMiliseconds)
        {
            FramesRendered = framesRendered;
            UpdatesProcessed = updatesProcessed;
            ElapsedMiliseconds = elapsedMiliseconds;
        }
    }
}
