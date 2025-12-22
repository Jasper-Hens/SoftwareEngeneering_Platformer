using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace test.Animations.DemonAnimations
{
    public class Demon2AttackAnimation : Animation
    {
        public Demon2AttackAnimation(Texture2D texture) : base(texture)
        {
            Frames = new List<Rectangle>
            {
                new Rectangle(156, 43, 78, 85),
                new Rectangle(291, 43, 72, 85),
                new Rectangle(24, 44, 72, 84),
                new Rectangle(427, 55, 80, 73),
                new Rectangle(555, 56, 62, 72)
            };
            FrameSpeed = 100;
            IsLooping = false;
        }
    }
}