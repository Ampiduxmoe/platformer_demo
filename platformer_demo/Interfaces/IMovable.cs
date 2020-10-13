using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace platformer_demo.Character
{
    interface IMovable
    {
        public Vector2 Speed { get; set; }

        public abstract void Move(GameTime gameTime);
    }
}
