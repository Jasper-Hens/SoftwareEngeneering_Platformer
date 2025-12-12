using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test.Animations.BossAnimations
{
    internal class BossDeathAnimation : Animation
    {
        public BossDeathAnimation(Texture2D texture) : base(texture)
        {
            IsLooping = false;
            FrameSpeed = 500;
            Frames.Add(new Rectangle(83, 145, 118, 104));    // 1 (sprite15)
            Frames.Add(new Rectangle(332, 145, 124, 106));   // 2 (sprite12)
            Frames.Add(new Rectangle(588, 145, 124, 100));   // 3 (sprite16)
            Frames.Add(new Rectangle(844, 145, 124, 87));    // 4 (sprite20)
            Frames.Add(new Rectangle(1100, 145, 124, 14));   // 5 (sprite45)
            Frames.Add(new Rectangle(1356, 191, 124, 14));   // 6 (sprite46)
            Frames.Add(new Rectangle(1612, 211, 124, 14));   // 7 (sprite47)
            //Frames.Add(new Rectangle(1868, 242, 124, 14));   // 8 (sprite48)

        }
    }
}
