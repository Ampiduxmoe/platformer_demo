using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using System.Text;

using platformer_demo.Environment;
using platformer_demo.Enums;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace platformer_demo.Character
{
    class Recto : Hero
    {
        public Recto(Map map) : base(map) 
        {
            PrimarySkillCooldown = 0;
            MovementSkillCooldown = 0;
        }

        public override void DamageSkill()
        {
            if (DamageSkillRemainingCooldown > 0)
                return;
        }

        public override void MovementSkill()
        {
            if (MovementSkillRemainingCooldown > 0)
                return;

            Vector2 direction = GetHeroToMouseDirection();
            AcceleratingProjectile projectile = new AcceleratingProjectile(CurrentMap);
            projectile.Initialize();
            projectile.Load();
            projectile.Center = new Point(Center.X + (int)(direction.X * 40), Center.Y + (int)(direction.Y * 40));
            projectile.Speed = direction * projectile.StartingSpeed;
            CurrentMap.AddEntity(projectile);

            const int force = 700;
            Vector2 newSpeed = Vector2.Zero;
            bool needToChangeSpeed = false;
            if (Speed.X > 0 && -direction.X * force > Speed.X || Speed.X < 0 && -direction.X * force < Speed.X || Speed.X * -direction.X <= 0)
            {
                newSpeed = new Vector2(-direction.X * force, Speed.Y);
                needToChangeSpeed = true;
            }
            if (Speed.Y > 0 && -direction.Y * force > Speed.Y || Speed.Y < 0 && -direction.Y * force < Speed.Y || Speed.Y * -direction.Y <= 0)
            { 
                newSpeed = new Vector2(newSpeed.X, -direction.Y * force);
                needToChangeSpeed = true;
            }
            if (needToChangeSpeed)
            {
                Speed = new Vector2(newSpeed.X, Speed.Y);
                ChangeSpeedSmoothly(newSpeed, 4);
            }

            const int
                shakePower = 5,
                flickCount = 2,
                flickDuration = 3;
            CurrentMap.MapCamera.GenerateShake(shakePower, flickCount, flickDuration);

            MovementSkillRemainingCooldown = MovementSkillCooldown;
        }

        public override void PrimarySkill()
        {
            if (PrimarySkillRemainingCooldown > 0)
                return;

            Vector2 direction = GetHeroToMouseDirection();
            Projectile projectile = new Projectile(CurrentMap);
            projectile.Initialize();
            projectile.Load();
            projectile.Center = Center;
            projectile.Speed = direction * projectile.StartingSpeed;
            CurrentMap.AddEntity(projectile);

            const int force = 100;
            if (Speed.X > 0 && -direction.X * force > Speed.X || Speed.X < 0 && -direction.X * force < Speed.X || Speed.X * -direction.X <= 0)
                Speed = new Vector2(-direction.X * force, Speed.Y);
            if (Speed.Y > 0 && -direction.Y * force > Speed.Y || Speed.Y < 0 && -direction.Y * force < Speed.Y || Speed.Y * -direction.Y <= 0)
                Speed = new Vector2(Speed.X, -direction.Y * force);

            PrimarySkillRemainingCooldown = PrimarySkillCooldown;
        }

        public override void SecondarySkill()
        {
            if (SecondarySkillRemainingCooldown > 0)
                return;
        }
    }
}
