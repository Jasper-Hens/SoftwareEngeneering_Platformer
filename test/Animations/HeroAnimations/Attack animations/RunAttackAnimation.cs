using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace test.Animations
{
    public class RunAttackAnimation : Animation
    {
        public RunAttackAnimation(Texture2D texture) : base(texture)
        {
            IsLooping = false; // Ook ren-aanval speelt 1x af
            FrameSpeed = 60;

            Frames.Add(new Rectangle(11, 11, 56, 75));   // sprite1
            Frames.Add(new Rectangle(79, 17, 60, 69));   // sprite2
            Frames.Add(new Rectangle(151, 23, 57, 63));  // sprite3
            Frames.Add(new Rectangle(220, 24, 65, 62));  // sprite4
            Frames.Add(new Rectangle(297, 24, 56, 62));  // sprite5
            Frames.Add(new Rectangle(373, 28, 68, 58));  // sprite6
            Frames.Add(new Rectangle(358, 53, 3, 21));   // sprite7

        }
    }
}