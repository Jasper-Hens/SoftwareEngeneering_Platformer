using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace test.Animations.BossAnimations
{
    public class BossIdleAnimation : Animation
    {
        public BossIdleAnimation(Texture2D texture) : base(texture)
        {
            IsLooping = true;
            FrameSpeed = 120; // Bosses bewegen vaak zwaar en traag

            Frames.Add(new Rectangle(89, 146, 112, 110));
            Frames.Add(new Rectangle(345, 151, 112, 105));
            Frames.Add(new Rectangle(600, 150, 112, 106));
            Frames.Add(new Rectangle(855, 152, 112, 104));
            Frames.Add(new Rectangle(1112, 151, 111, 105));
            Frames.Add(new Rectangle(1368, 159, 112, 97));
            Frames.Add(new Rectangle(1625, 153, 112, 103));
            Frames.Add(new Rectangle(1882, 150, 112, 106));
        }
    }
}