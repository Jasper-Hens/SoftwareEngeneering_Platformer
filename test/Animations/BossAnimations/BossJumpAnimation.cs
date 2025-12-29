using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace test.Animations.BossAnimations
{
    internal class BossJumpAnimation : Animation
    {
        public BossJumpAnimation(Texture2D texture) : base(texture)
        {
            IsLooping = false;
            FrameSpeed = 130;
            Frames.Add(new Rectangle(89, 146, 112, 110));      // 1 (sprite5)
            Frames.Add(new Rectangle(354, 159, 131, 97));      // 2 (sprite17)
            Frames.Add(new Rectangle(609, 159, 132, 97));      // 3 (sprite18)
            Frames.Add(new Rectangle(869, 146, 46, 109));      // 4 (sprite6)
            Frames.Add(new Rectangle(1124, 135, 48, 106));     // 5 (sprite2)
            Frames.Add(new Rectangle(1377, 149, 93, 105));     // 6 (sprite10)
            Frames.Add(new Rectangle(1634, 159, 131, 97));     // 7 (sprite19)
            Frames.Add(new Rectangle(1882, 154, 112, 102));     // 8 (sprite15)

        }
    }
}
