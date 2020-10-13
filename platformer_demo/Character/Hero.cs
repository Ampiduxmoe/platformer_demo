using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace platformer_demo.Character
{
    public abstract class Hero : IEntity, IMovable
    {
        protected const string _assetsDirectory = "Character";

        public abstract string ASSET_NAME_CHARA_TEXTURE { get; }

        public Map CurrentMap { get; protected set; }

        public abstract Point Position { get; set; }

        public abstract Point Center { get; }

        public abstract Point Size { get; set; }

        public abstract Rectangle Rect { get; set; }

        public abstract Vector2 Speed { get; set; }

        public abstract HeroState State { get; set; }

        public abstract Texture2D Texture { get; set; }

        public Hero(Map map) => RebindMap(map);

        public void RebindMap(Map map) => CurrentMap = map;

        public abstract Rectangle GetCollisionRect();

        public abstract bool CanMoveUp();
        public abstract bool CanMoveDown();
        public abstract bool CanMoveLeft();
        public abstract bool CanMoveRight();

        public abstract void PrimarySkill();

        public abstract void SecondarySkill();

        public abstract void DamageSkill();

        public abstract void MovementSkill();

        public abstract void Initialize();

        public abstract void Load(ContentManager content);

        public abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime);

        public abstract void OffsetDraw(SpriteBatch spriteBatch, GameTime gameTime, Point offset);

        public abstract void Update(GameTime gameTime);

        public abstract void Move(GameTime gameTime);
    }
}
