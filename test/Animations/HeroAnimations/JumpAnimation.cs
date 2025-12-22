using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test.Animations.HeroAnimations
{
    internal class JumpAnimation : Animation
    {
        public JumpAnimation(Texture2D texture) : base(texture)
        {
            Frames.Add(new Rectangle(255, 21, 69, 65));
            Frames.Add(new Rectangle(107, 22, 49, 64));
            Frames.Add(new Rectangle(179, 22, 46, 64));
            Frames.Add(new Rectangle(331, 28, 64, 58));
            Frames.Add(new Rectangle(28, 29, 49, 57));
            Frames.Add(new Rectangle(411, 29, 61, 57));

            FrameSpeed = 150;
        }
    }
}
