using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace test.SpriteFrames
{
    internal class RunSprite
    {
        public Rectangle Rect { get; }

        public RunSprite(int x, int y, int width, int height)
        {
            Rect = new Rectangle(x, y, width, height);
        }

      

    }
}
