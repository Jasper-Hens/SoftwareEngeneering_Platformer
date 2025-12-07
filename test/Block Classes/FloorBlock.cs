using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using test.block_Interfaces;

namespace test.Blocks
{
    // Implementeert ISolid voor volledige botsing van alle kanten.
    public class FloorBlock : Block, ISolid
    {
        // Source Rect voor Floor/Sprite 12: 325, 79, 64, 50
        private static readonly Rectangle SourceRect = new Rectangle(325, 79, 64, 50);
        private const int TILE_GRID_SIZE = 64;

        public FloorBlock(int gridX, int gridY, Texture2D texture)
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

        bool ISolid.IsSolidBlock()
        {
            return true;
        }
    }
}