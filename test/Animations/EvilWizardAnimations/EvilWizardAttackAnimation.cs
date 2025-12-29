using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace test.Animations.EvilWizardAnimations
{
    public class EvilWizardAttackAnimation : Animation
    {
        public EvilWizardAttackAnimation(Texture2D texture) : base(texture)
        {
            Frames = new List<Rectangle>
            {
                new Rectangle(59, 51, 75, 49),
                new Rectangle(209, 51, 71, 50),
                new Rectangle(359, 51, 75, 49),
                new Rectangle(509, 51, 70, 50),
                new Rectangle(659, 51, 75, 49),
                new Rectangle(809, 51, 71, 50),
                new Rectangle(959, 51, 75, 49),
                new Rectangle(1109, 51, 70, 50)
            };

            FrameSpeed = 100;
            IsLooping = false;

            // Damage tijdens de zwaai (frames 4, 5, 6)
            DamageFrames = new List<int> { 4, 5, 6 };
        }
    }
}