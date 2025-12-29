using Microsoft.Xna.Framework; // <--- HEEL BELANGRIJK VOOR Vector2
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using test.Blocks;
using test.Items;
using test.Objects;

namespace test.Levels
{
    public abstract class BaseLevel
    {
        public List<Block> Blocks { get; protected set; } = new List<Block>();
        public List<Enemy> Enemies { get; protected set; } = new List<Enemy>();
        public List<Item> Items { get; protected set; } = new List<Item>();

        public List<Spikes> SpikesObjects { get; protected set; } = new List<Spikes>();
        public List<SpinningBlade> Blades { get; protected set; } = new List<SpinningBlade>();

        public Door ExitDoor { get; protected set; }
        public Door EntryDoor { get; protected set; }

        public Vector2 StartPosition { get; protected set; }
        public int Width { get; protected set; }
        public int Height { get; protected set; }

        public abstract void LoadContent(ContentManager content, GraphicsDevice graphics);
    }
}