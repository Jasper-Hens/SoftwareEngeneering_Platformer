using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test.Animations;

internal class AttackThreeAnimation : Animation
{
    public AttackThreeAnimation(Texture2D texture) : base(texture)
    {

        IsLooping = false; // Belangrijk: Aanvallen loopen niet!
        FrameSpeed = 60;   // Aanvallen zijn vaak snel

        Frames.Add(new Rectangle(12, 22, 48, 64));   // sprite1
        Frames.Add(new Rectangle(116, 22, 49, 64));  // sprite2
        Frames.Add(new Rectangle(220, 24, 85, 62));  // sprite3
        Frames.Add(new Rectangle(318, 27, 80, 59));  // sprite4


    }
}
