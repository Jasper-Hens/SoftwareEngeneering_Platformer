using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using test.block_Interfaces;
using test.Blocks;

namespace test.Block_Classes
{
    public class Wall : Block, ISolid
    {
        // Source Rect voor Floor/Sprite 12: 325, 79, 64, 50
        private static readonly Rectangle SourceRect = new Rectangle(195, 325, 64, 64);
        private const int TILE_GRID_SIZE = 64;

        public Wall(int gridX, int gridY, Texture2D texture)
            : base(CalculateWorldRect(gridX, gridY, SourceRect.Width, SourceRect.Height), SourceRect, texture)
        {
        }

        // Plaatst de sprite onderaan de 64x64 grid cel.
        private static Rectangle CalculateWorldRect(int gridX, int gridY, int spriteWidth, int spriteHeight)
        {
            int worldX = gridX * TILE_GRID_SIZE;
            // Verschuif Y om de onderkant van de 50px sprite gelijk te laten lopen met de onderkant van de 64px cel.
            int worldY = (gridY * TILE_GRID_SIZE) + (TILE_GRID_SIZE - spriteHeight);

            return new Rectangle(worldX, worldY, spriteWidth, spriteHeight);
        }

        public bool IsSolidBlock()
        {
            return true;
        }
    }
}
