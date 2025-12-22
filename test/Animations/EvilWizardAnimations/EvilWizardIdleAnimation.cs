using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using test.Animations;

namespace test.Animations.EvilWizardAnimations
{
    public class EvilWizardIdleAnimation : Animation
    {
        public EvilWizardIdleAnimation(Texture2D texture) : base(texture)
        {
            Frames = new List<Rectangle>
            {
                new Rectangle(59, 46, 30, 54),
                new Rectangle(209, 49, 30, 52),
                new Rectangle(358, 48, 31, 54),
                new Rectangle(507, 49, 32, 53),
                new Rectangle(657, 50, 32, 52),
                new Rectangle(807, 48, 32, 53),
                new Rectangle(957, 45, 32, 56),
                new Rectangle(1106, 44, 33, 57)
            };

            FrameSpeed = 150;
            IsLooping = true;
        }
    }
}