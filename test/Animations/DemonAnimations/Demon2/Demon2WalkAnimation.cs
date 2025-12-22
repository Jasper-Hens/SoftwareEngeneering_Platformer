using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace test.Animations.DemonAnimations
{
    public class Demon2WalkAnimation : Animation
    {
        public Demon2WalkAnimation(Texture2D texture) : base(texture)
        {
            Frames = new List<Rectangle>
            {
                new Rectangle(434, 41, 44, 87),
                new Rectangle(561, 41, 45, 87),
                new Rectangle(1330, 41, 44, 87),
                new Rectangle(300, 42, 50, 86),
                new Rectangle(686, 42, 48, 86),
                new Rectangle(1202, 42, 44, 86),
                new Rectangle(1454, 42, 48, 86),
                new Rectangle(41, 43, 53, 85),
                new Rectangle(168, 43, 54, 85),
                new Rectangle(808, 43, 54, 85),
                new Rectangle(934, 43, 56, 85),
                new Rectangle(1067, 43, 51, 85)
            };
            FrameSpeed = 100;
            IsLooping = true;
        }
    }
}