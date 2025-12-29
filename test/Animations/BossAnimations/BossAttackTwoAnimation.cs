using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace test.Animations.BossAnimations
{
    internal class BossAttackTwoAnimation : Animation
    {
        public BossAttackTwoAnimation(Texture2D texture) : base(texture)
        {
            IsLooping = false;
            FrameSpeed = 150;
            Frames.Add(new Rectangle(89, 146, 112, 110));    // 1 (sprite10)
            Frames.Add(new Rectangle(347, 151, 114, 105));   // 2 (sprite14)
            Frames.Add(new Rectangle(612, 93, 47, 163));     // 3 (sprite2)
            Frames.Add(new Rectangle(872, 92, 43, 164));     // 4 (sprite1)
            Frames.Add(new Rectangle(1127, 104, 148, 152));  // 5 (sprite3)
            Frames.Add(new Rectangle(1381, 157, 151, 99));   // 6 (sprite20)
            Frames.Add(new Rectangle(1638, 154, 150, 102));  // 7 (sprite17)
            Frames.Add(new Rectangle(1876, 150, 129, 106));  // 8 (sprite13)

        }
    }
}
