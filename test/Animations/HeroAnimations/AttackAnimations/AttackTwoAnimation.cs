using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace test.Animations;

internal class AttackTwoAnimation : Animation
{
    public AttackTwoAnimation(Texture2D texture) : base(texture)
    {

        IsLooping = false; // Belangrijk: Aanvallen loopen niet!
        FrameSpeed = 60;   // Aanvallen zijn vaak snel

        Frames.Add(new Rectangle(33, 11, 42, 75));   // sprite1
        Frames.Add(new Rectangle(128, 15, 43, 71));  // sprite2
        Frames.Add(new Rectangle(281, 18, 32, 55));  // sprite3
        Frames.Add(new Rectangle(326, 22, 66, 64));  // sprite5

    }
}
