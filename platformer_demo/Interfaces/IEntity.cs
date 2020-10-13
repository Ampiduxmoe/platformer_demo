using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace platformer_demo
{
    public interface IEntity
    {
        public Point Position { get; set; }

        public Point Size { get; set; }

        public Rectangle Rect { get; set; }

        public void Initialize();

        public void Load(ContentManager content);

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime);

        public void OffsetDraw(SpriteBatch spriteBatch, GameTime gameTime, Point offset);

        public void Update(GameTime gameTime);
    }
}
