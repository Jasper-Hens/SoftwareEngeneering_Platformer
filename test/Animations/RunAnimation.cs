using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace test.Animations
{
    public class RunAnimation : Animation
    {
        public RunAnimation(Texture2D texture) : base(texture)
        {
            Frames.Add(new Rectangle(227, 23, 51, 63));
            Frames.Add(new Rectangle(159, 24, 49, 62));
            Frames.Add(new Rectangle(437, 24, 48, 62));
            Frames.Add(new Rectangle(19, 25, 48, 61));
            Frames.Add(new Rectangle(297, 26, 48, 60));
            Frames.Add(new Rectangle(86, 27, 54, 59));
            Frames.Add(new Rectangle(364, 27, 54, 59));
        }
    }
}
