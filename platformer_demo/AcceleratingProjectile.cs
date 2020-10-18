using Microsoft.Xna.Framework;
using platformer_demo.Environment;
using System;
using System.Collections.Generic;
using System.Text;

namespace platformer_demo
{
    public class AcceleratingProjectile : Projectile
    {
        public AcceleratingProjectile(Map map) : base(map) 
        {
            AffectedByGravity = false;
            StartingSpeed = 100f;
            LifeLength = 2d;
            Color = Color.White; 
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Speed.Length() < 4000)
                Speed *= 1.4f;
        }
    }
}
