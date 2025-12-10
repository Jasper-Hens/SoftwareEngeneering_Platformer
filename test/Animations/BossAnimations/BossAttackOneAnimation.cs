using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test.Animations.BossAnimations
{
    internal class BossAttackOneAnimation : Animation
    {
        public BossAttackOneAnimation(Texture2D texture) : base(texture)
        {
            IsLooping = false;
            FrameSpeed = 150;
            Frames.Add(new Rectangle(89, 146, 112, 110));    // 1 (sprite10)
            Frames.Add(new Rectangle(330, 151, 124, 105));   // 2 (sprite17)
            Frames.Add(new Rectangle(583, 151, 123, 105));   // 3 (sprite18)
            Frames.Add(new Rectangle(835, 154, 124, 102));   // 4 (sprite22)
            Frames.Add(new Rectangle(1127, 151, 148, 105));  // 5 (sprite19)
            Frames.Add(new Rectangle(1381, 157, 151, 99));   // 6 (sprite26)
            Frames.Add(new Rectangle(1638, 154, 150, 102));  // 7 (sprite23)
            Frames.Add(new Rectangle(1876, 150, 129, 106));  // 8 (sprite16)
        }
    }
}
