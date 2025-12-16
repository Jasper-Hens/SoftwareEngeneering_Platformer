using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using test.Items;
using test.Objects;
using test.Level;
using test.Blocks;

namespace test.Levels
{
    public abstract class BaseLevel
    {
        public List<Block> Blocks { get; protected set; } = new List<Block>();
        public List<Enemy> Enemies { get; protected set; } = new List<Enemy>();
        public List<Item> Items { get; protected set; } = new List<Item>();
        public Door ExitDoor { get; protected set; }
        public Vector2 StartPosition { get; protected set; }
        public int Width { get; protected set; }
        public int Height { get; protected set; }

        public abstract void LoadContent(ContentManager content, GraphicsDevice graphics);
    }
}