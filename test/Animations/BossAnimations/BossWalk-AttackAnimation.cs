using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace test.Animations.BossAnimations
{
    internal class BossWalk_AttackAnimation : Animation
    {
        public BossWalk_AttackAnimation(Texture2D texture) : base(texture)
        {
            IsLooping = false;
            FrameSpeed = 200;
            Frames.Add(new Rectangle(82, 130, 119, 110));      // 1 (sprite15)
            Frames.Add(new Rectangle(330, 130, 124, 106));     // 2 (sprite21)
            Frames.Add(new Rectangle(583, 130, 123, 107));     // 3 (sprite19)
            Frames.Add(new Rectangle(835, 130, 124, 103));     // 4 (sprite24)
            Frames.Add(new Rectangle(1121, 130, 145, 104));    // 5 (sprite23)
            Frames.Add(new Rectangle(1379, 130, 144, 98));     // 6 (sprite26)
            Frames.Add(new Rectangle(1637, 130, 142, 105));    // 7 (sprite22)
            Frames.Add(new Rectangle(1876, 130, 129, 108));    // 8 (sprite18)

        }
    }
}
