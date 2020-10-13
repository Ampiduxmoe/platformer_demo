using MonoGame.Extended;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace platformer_demo
{
    public static class Extensions
    {
        public static bool Crosses(this Ray2 ray, BoundingRectangle boundingRectangle, out float near, out float far)
        {
            if (ray.Direction.X < 0 && ray.Direction.Y < 0)
            {
                if (boundingRectangle.Center.X - boundingRectangle.HalfExtents.X > ray.Position.X || 
                    boundingRectangle.Center.Y - boundingRectangle.HalfExtents.Y > ray.Position.Y)
                {
                    near = float.NaN;
                    far = float.NaN;
                    return false;
                }

                BoundingRectangle dummyRect = new BoundingRectangle(
                    new Point2(2 * ray.Position.X - boundingRectangle.Center.X, 2 * ray.Position.Y - boundingRectangle.Center.Y),
                    boundingRectangle.HalfExtents);
                Ray2 dummyRay = new Ray2(ray.Position, new Vector2(-ray.Direction.X, -ray.Direction.Y));
                return dummyRay.Intersects(dummyRect, out near, out far);
            }

            if (ray.Direction.X < 0)
            {
                if (boundingRectangle.Center.X - boundingRectangle.HalfExtents.X > ray.Position.X || 
                    boundingRectangle.Center.Y + boundingRectangle.HalfExtents.Y < ray.Position.Y)
                {
                    near = float.NaN;
                    far = float.NaN;
                    return false;
                }

                BoundingRectangle dummyRect = new BoundingRectangle(
                    new Point2(2 * ray.Position.X - boundingRectangle.Center.X, boundingRectangle.Center.Y),
                    boundingRectangle.HalfExtents);
                Ray2 dummyRay = new Ray2(ray.Position, new Vector2(-ray.Direction.X, ray.Direction.Y));
                return dummyRay.Intersects(dummyRect, out near, out far);
            }

            if (ray.Direction.Y < 0)
            {
                if (boundingRectangle.Center.X + boundingRectangle.HalfExtents.X < ray.Position.X || 
                    boundingRectangle.Center.Y - boundingRectangle.HalfExtents.Y > ray.Position.Y)
                {
                    near = float.NaN;
                    far = float.NaN;
                    return false;
                }

                BoundingRectangle dummyRect = new BoundingRectangle(
                    new Point2(boundingRectangle.Center.X, 2 * ray.Position.Y - boundingRectangle.Center.Y),
                    boundingRectangle.HalfExtents);
                Ray2 dummyRay = new Ray2(ray.Position, new Vector2(ray.Direction.X, -ray.Direction.Y));
                return dummyRay.Intersects(dummyRect, out near, out far);
            }

            if (boundingRectangle.Center.X + boundingRectangle.HalfExtents.X < ray.Position.X || 
                boundingRectangle.Center.Y + boundingRectangle.HalfExtents.Y < ray.Position.Y)
            {
                near = float.NaN;
                far = float.NaN;
                return false;
            }

            return ray.Intersects(boundingRectangle, out near, out far);
        }

        public static void OffsetDraw(this SpriteBatch spriteBatch, Texture2D texture, Rectangle destinationRectangle, Point offset, Color color)
        {
            spriteBatch.Draw(
                texture,
                new Rectangle(new Point(destinationRectangle.X + offset.X, destinationRectangle.Y + offset.Y), destinationRectangle.Size),
                color);
        }

        public static void OffsetDrawString(this SpriteBatch spriteBatch, SpriteFont spriteFont, string text, Vector2 position, Point offset, Color color)
        {
            spriteBatch.DrawString(
                spriteFont,
                text,
                new Vector2(position.X + offset.X, position.Y + offset.Y),
                color);
        }

        public static void OffsetDrawLine(this SpriteBatch spriteBatch, float x1, float y1, float x2, float y2, Point offset, Color color, float thickness = 1, float layerDepth = 0)
        {
            spriteBatch.DrawLine(
                x1 + offset.X, 
                y1 + offset.Y, 
                x2 + offset.X, 
                y2 + offset.Y, 
                color);
        }
    }
}
