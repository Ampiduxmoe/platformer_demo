using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using platformer_demo.Character;
using platformer_demo.Environment;
using platformer_demo.Interfaces;
using platformer_demo.Enums;

using System;
using System.Collections.Generic;
using System.Text;

namespace platformer_demo
{
    public class Projectile : Movable_MapEntity, IPenetratable
    {
        public double LifeLength { get; set; } = 5d;
        public double TimeElapsed { get; private set; } = 0d;
        public float StartingSpeed { get; protected set; } = 1500f;

        public Projectile(Map map) : base(map)
        {
            _assetsDirectory = "Environment";
            ASSET_NAME_CHARA_TEXTURE = "defaultProjectile";
            Color = Color.Red;

            Vector2 position = new Vector2(100, 200);
            Point size = new Point(10, 10);
            Vector2 speed = new Vector2(0, 0);
            SetValues(position, size, speed);
        }

        public override void Initialize()
        {
            
        }

        public override void Load(ContentManager content)
        {
            Texture = content.Load<Texture2D>(@$"{_assetsDirectory}\{ASSET_NAME_CHARA_TEXTURE}");
        }

        public float BounceEnergyLoss = 0.33f;
        public override void Update(GameTime gameTime)
        {
            State = IsClipping() ? ObjectState.Clipping : ObjectState.Falling;

            switch (State)
            {
                case ObjectState.Falling:
                    if (AffectedByGravity)
                        Speed = new Vector2(Speed.X, (float)(Speed.Y + GameParameters.Gravity * GameParameters.GetElapsedGameTime(gameTime)));
                    break;
                case ObjectState.Clipping:
                    PushUntilNoCollisions();
                    break;
            }

            if (State == ObjectState.Falling)
            {
                if (!CanMoveLeft() && Speed.X < 0)
                    Speed = new Vector2(Math.Abs(Speed.X) * (1 - BounceEnergyLoss / 6), Speed.Y);
                if (!CanMoveRight() && Speed.X > 0)
                    Speed = new Vector2(-Math.Abs(Speed.X) * (1 - BounceEnergyLoss / 6), Speed.Y);
                if (!CanMoveUp() && Speed.Y < 0)
                    Speed = new Vector2(Speed.X, Math.Abs(Speed.Y) * (1 - BounceEnergyLoss));
                if (!CanMoveDown() && Speed.Y > 0)
                    Speed = new Vector2(Speed.X, -Math.Abs(Speed.Y) * (1 - BounceEnergyLoss));

                Move(gameTime);
            }

            TimeElapsed += GameParameters.GetElapsedGameTime(gameTime);
        }
    }
}
