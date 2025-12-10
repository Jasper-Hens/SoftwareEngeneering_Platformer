using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test.Animations.BossAnimations
{
    internal class BossWalkAnimation : Animation
    {
        public BossWalkAnimation(Texture2D texture) : base(texture)
        {
            IsLooping = true;
            FrameSpeed = 150;
            Frames.Add(new Rectangle(82, 140, 111, 110));      // 1 (sprite15)
            Frames.Add(new Rectangle(340, 140, 111, 106));     // 2 (sprite21)
            Frames.Add(new Rectangle(600, 140, 110, 107));     // 3 (sprite19)
            Frames.Add(new Rectangle(857, 140, 112, 103));     // 4 (sprite24)
            Frames.Add(new Rectangle(1116, 140, 112, 104));    // 5 (sprite23)
            Frames.Add(new Rectangle(1369, 140, 112, 98));     // 6 (sprite26)
            Frames.Add(new Rectangle(1624, 140, 110, 105));    // 7 (sprite22)
            Frames.Add(new Rectangle(1877, 140, 111, 108));    // 8 (sprite18)

        }
    }
}
