using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace test.Animations.DemonAnimations
{
    public class Demon2DeathAnimation : Animation
    {
        public Demon2DeathAnimation(Texture2D texture) : base(texture)
        {
            Frames = new List<Rectangle>
            {
                new Rectangle(23, 70, 74, 58),
                new Rectangle(150, 87, 86, 41),
                new Rectangle(277, 102, 99, 26)
            };
            FrameSpeed = 150;
            IsLooping = false;
        }
    }
}