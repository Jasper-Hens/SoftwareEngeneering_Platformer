using Microsoft.Xna.Framework.Graphics;
using test.Block_Classes;
using test.Blocks;

namespace test.Level
{
    // Enum voor de verschillende bloktypes, gekoppeld aan de int in de _gameboard array.
    public enum BlockType
    {
        Empty = 0,
        Floor = 1, // ISolid (Vloer)
        JumpThrough = 2, // IPlatform (Platform)
        // Water = 3, // IPassable (Indien later toegevoegd)
        Wall = 3, // Isolid Muur
    }

    public static class BlockFactory
    {
        public const int TILE_SIZE = 64;

        public static Block CreateBlock(int type, int gridX, int gridY, Texture2D tileset)
        {
            BlockType blockType = (BlockType)type;

            switch (blockType)
            {
                case BlockType.Floor:
                    // Maak een FloorBlock, de constructor berekent zelf de WorldRect.
                    return new FloorBlock(gridX, gridY, tileset);

                case BlockType.JumpThrough:
                    // Maak een JumpThroughPlatform, de constructor berekent zelf de WorldRect.
                    return new JumpThroughPlatform(gridX, gridY, tileset);
                case BlockType.Wall:
                    return new Wall(gridX, gridY, tileset);
                case BlockType.Empty:
                default:
                    return null; // Geen blok voor type 0
            }
        }
    }
}