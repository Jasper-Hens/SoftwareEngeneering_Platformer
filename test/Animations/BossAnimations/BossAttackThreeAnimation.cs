using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test.Animations.BossAnimations
{
    internal class BossAttackThreeAnimation : Animation
    {
        public BossAttackThreeAnimation(Texture2D texture) : base(texture)
        {
            Frames.Add(new Rectangle(89, 146, 112, 110));    // 1 (sprite10)
            Frames.Add(new Rectangle(343, 151, 112, 105));   // 2 (sprite14)
            Frames.Add(new Rectangle(612, 93, 136, 163));    // 3 (sprite2)
            Frames.Add(new Rectangle(872, 92, 43, 164));     // 4 (sprite1)
            Frames.Add(new Rectangle(1127, 107, 142, 149));  // 5 (sprite3)
            Frames.Add(new Rectangle(1381, 157, 138, 99));   // 6 (sprite20)
            Frames.Add(new Rectangle(1638, 154, 118, 102));  // 7 (sprite17)
            Frames.Add(new Rectangle(1876, 150, 129, 106));  // 8 (sprite13)
        }
    }
}
