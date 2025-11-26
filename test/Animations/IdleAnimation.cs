using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace test.Animations
{
    public class IdleAnimation : Animation
    {
        public IdleAnimation(Texture2D texture) : base(texture)
        {
            Frames.Add(new Rectangle(24, 22, 43, 64));
            Frames.Add(new Rectangle(91, 22, 43, 64));
            Frames.Add(new Rectangle(158, 22, 43, 64));
            Frames.Add(new Rectangle(225, 22, 43, 64));
        }
    }
}
