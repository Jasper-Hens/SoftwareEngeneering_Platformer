using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace test.Animations;

public class AttackOneAnimation : Animation
{
    public AttackOneAnimation(Texture2D texture) : base(texture)
    {
        IsLooping = false; // Belangrijk: Aanvallen loopen niet!
        FrameSpeed = 60;   // Aanvallen zijn vaak snel

        Frames.Add(new Rectangle(341, 12, 45, 74));   // sprite1
        Frames.Add(new Rectangle(400, 14, 26, 53));   // sprite2
        Frames.Add(new Rectangle(26, 22, 43, 64));    // sprite3
        Frames.Add(new Rectangle(92, 22, 59, 64));    // sprite4
        Frames.Add(new Rectangle(174, 22, 64, 64));   // sprite5


    }
}