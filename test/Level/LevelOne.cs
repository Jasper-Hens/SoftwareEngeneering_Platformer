using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using test.Items;
using test.Objects;
using test.Blocks;
using test.Level;

namespace test.Levels
{
    public class LevelOne : BaseLevel
    {
        public override void LoadContent(ContentManager content, GraphicsDevice graphics)
        {
            StartPosition = new Vector2(100, 300);

            // MAP DATA (Ingekort voor voorbeeld, gebruik jouw volledige map hier!)
            int[,] gameboard = new int[,]
            {
                { 3,3,3,3,3,3,3,3,3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
                { 3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
                { 3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
                { 3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
                { 3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
                { 3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
                { 3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
                { 3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
                { 3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
                { 3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
                { 3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
                { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 }
            };

            Texture2D tiles = content.Load<Texture2D>("Tiles/tilesSpriteSheet");
            for (int y = 0; y < gameboard.GetLength(0); y++)
            {
                for (int x = 0; x < gameboard.GetLength(1); x++)
                {
                    Block b = BlockFactory.CreateBlock(gameboard[y, x], x, y, tiles);
                    if (b != null) Blocks.Add(b);
                }
            }

            Width = gameboard.GetLength(1) * 64;
            Height = gameboard.GetLength(0) * 64;

            // ITEMS
            //Texture2D itemSheet = content.Load<Texture2D>("Items/ItemSpritesheet");
            // Items.Add(new KeyItem(itemSheet, new Vector2(100, 100)));

            Texture2D Key = content.Load<Texture2D>("Items/KeyV2");
            Items.Add(new KeyItem(Key, new Vector2(400, 575)));

            // DEUR
            Texture2D objSheet = content.Load<Texture2D>("Objects/ObjectSpriteSheet");
            ExitDoor = new Door(objSheet, objSheet, new Vector2(300, 635));

            // BOSS
            Texture2D blokTex = new Texture2D(graphics, 1, 1); blokTex.SetData(new[] { Color.White });

            Texture2D bIdle = content.Load<Texture2D>("Boss/Idle");
            Texture2D bWalk = content.Load<Texture2D>("Boss/Walk");
            Texture2D bRun = content.Load<Texture2D>("Boss/Run");
            Texture2D bJump = content.Load<Texture2D>("Boss/Jump");
            Texture2D bAtk1 = content.Load<Texture2D>("Boss/Attack");
            Texture2D bAtk2 = content.Load<Texture2D>("Boss/Attack2");
            Texture2D bAtk3 = content.Load<Texture2D>("Boss/Attack3");
            Texture2D bWAtk = content.Load<Texture2D>("Boss/Walk_Attack");
            Texture2D bSpec = content.Load<Texture2D>("Boss/Special");
            Texture2D bHurt = content.Load<Texture2D>("Boss/Hurt");
            Texture2D bDeath = content.Load<Texture2D>("Boss/Death");

            KnightBoss boss = new KnightBoss(
                new Vector2(600, 500),
                blokTex,
                bIdle, bWalk, bRun, bJump,
                bAtk1, bAtk2, bAtk3, bWAtk, bSpec,
                bHurt, bDeath
            );
            Enemies.Add(boss);
        }
    }
}