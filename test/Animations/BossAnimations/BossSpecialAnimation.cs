using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test.Animations.BossAnimations
{
    internal class BossSpecialAnimation : Animation
    {
        public BossSpecialAnimation(Texture2D texture) : base(texture)
        {
            Frames.Add(new Rectangle(89, 140, 111, 109));      // 1 (sprite22)
            Frames.Add(new Rectangle(344, 140, 111, 104));     // 2 (sprite26)
            Frames.Add(new Rectangle(600, 121, 118, 135));     // 3 (sprite4)
            Frames.Add(new Rectangle(854, 119, 113, 137));     // 4 (sprite2)
            Frames.Add(new Rectangle(1110, 120, 113, 124));    // 5 (sprite11)
            Frames.Add(new Rectangle(1368, 130, 112, 116));    // 6 (sprite15)
            Frames.Add(new Rectangle(1625, 130, 112, 102));    // 7 (sprite27)
            Frames.Add(new Rectangle(1882, 130, 112, 106));    // 8 (sprite25)

        }
    }
}
