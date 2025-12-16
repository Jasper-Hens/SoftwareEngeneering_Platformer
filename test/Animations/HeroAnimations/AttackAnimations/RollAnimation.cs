using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace test.Animations;

public class RollAnimation : Animation
{
    public RollAnimation(Texture2D texture) : base(texture)
    {
       
        int frames = 6;
        int frameWidth = texture.Width / frames;
        int frameHeight = texture.Height;

        for (int i = 0; i < frames; i++)
        {
            Frames.Add(new Rectangle(i * frameWidth, 0, frameWidth, frameHeight));
        }

        // Een rol is snel! Zet de snelheid lager voor een snellere animatie.
        FrameSpeed = 60;
        IsLooping = false; // Rolt maar 1 keer
    }
}