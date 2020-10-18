using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace platformer_demo
{
    public interface IEntity
    {
        public Vector2 Position { get; set; }

        public Point Size { get; set; }

        public Rectangle Rect { get; set; }

        public Point Center { get; set; }

        public void Initialize();

        public void Load(ContentManager content);

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime);

        public void OffsetDraw(SpriteBatch spriteBatch, GameTime gameTime, Vector2 offset);

        public void Update(GameTime gameTime);
    }
}
