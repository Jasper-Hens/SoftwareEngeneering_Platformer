using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace test.Animations.HeroAnimations
{
    public class RunAnimation : Animation
    {
        public RunAnimation(Texture2D texture) : base(texture)
        {
            Frames.Add(new Rectangle(227, 23, 51, 63));
            Frames.Add(new Rectangle(159, 23, 49, 62));
            Frames.Add(new Rectangle(437, 23, 48, 62));
            Frames.Add(new Rectangle(19, 23, 48, 61));
            Frames.Add(new Rectangle(297, 23, 48, 60));
            Frames.Add(new Rectangle(86, 23, 54, 59));
            Frames.Add(new Rectangle(364, 23, 54, 59));
        }
    }
}
