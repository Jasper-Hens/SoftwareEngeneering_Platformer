using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test
{
    public class Hitbox
    {
        public Rectangle HitboxRect { get; private set; }

        public void Update(Vector2 position, Rectangle frame)
        {
            HitboxRect = new Rectangle(
                (int)position.X,
                (int)position.Y,
                frame.Width,
                frame.Height
            );
        }

        public bool Intersects(Hitbox other)
        {
            return HitboxRect.Intersects(other.HitboxRect);
        }
    }
}
