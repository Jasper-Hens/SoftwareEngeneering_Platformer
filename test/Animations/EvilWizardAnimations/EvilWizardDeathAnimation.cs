using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using test.Animations;

namespace test.Animations.EvilWizardAnimations
{
    public class EvilWizardDeathAnimation : Animation
    {
        public EvilWizardDeathAnimation(Texture2D texture) : base(texture)
        {
            Frames = new List<Rectangle>
            {
                new Rectangle(55, 51, 36, 46),
                new Rectangle(209, 44, 40, 56),
                new Rectangle(359, 63, 44, 38),
                new Rectangle(509, 77, 44, 25),
                new Rectangle(657, 75, 47, 27)
            };

            FrameSpeed = 150f;
            IsLooping = false;
        }
    }
}