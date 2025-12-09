using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test.Animations.BossAnimations
{
    internal class BossRunAnimation : Animation
    {
        public BossRunAnimation(Texture2D texture) : base(texture)
        {
            Frames.Add(new Rectangle(69, 157, 112, 99));      // 1 (sprite12)
            Frames.Add(new Rectangle(323, 161, 112, 95));     // 2 (sprite20)
            Frames.Add(new Rectangle(579, 158, 112, 96));     // 3 (sprite15)
            Frames.Add(new Rectangle(849, 158, 124, 98));     // 4 (sprite16)
            Frames.Add(new Rectangle(1116, 157, 112, 99));    // 5 (sprite14)
            Frames.Add(new Rectangle(1373, 163, 113, 93));    // 6 (sprite23)
            Frames.Add(new Rectangle(1621, 160, 121, 94));    // 7 (sprite19)
            Frames.Add(new Rectangle(1873, 161, 124, 95));    // 8 (sprite21)

        }
    }
}
