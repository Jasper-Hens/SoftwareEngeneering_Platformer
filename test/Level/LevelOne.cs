using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using test.Items;
using test.Objects;
using test.Blocks;
using test.Level;

namespace test.Levels
{
    public class LevelOne : BaseLevel
    {
        // --- LEGENDA ---
        // 0 = Leeg, 1 = Vloer, 2 = Platform, 3 = Muur
        // 10 = Spikes, 11 = Blade
        // 20 = Key, 21 = Hartje
        // 30 = Demon, 31 = Wizard, 35 = Boss
        // 50 = Deur, 99 = Start Speler

        public override void LoadContent(ContentManager content, GraphicsDevice graphics)
        {
            // We laden hier alleen de textures die de Factory NIET afhandelt (Blokken & Items)
            Texture2D tiles = content.Load<Texture2D>("Tiles/tilesSpriteSheet");
            Texture2D objSheet = content.Load<Texture2D>("Objects/ObjectSpriteSheet");
            Texture2D itemSheet = content.Load<Texture2D>("Items/ItemSpritesheet");
            Texture2D keyTex = content.Load<Texture2D>("Items/KeyV2");

            // --- DE MAP (100 breed, 12 hoog) ---
            int[,] gameboard = new int[,]
            {
                { 3,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,3 },
                { 3,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,3 },
                { 3,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,3 },
                { 3,0,99,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,50,0,0,3 }, // 99=Start, 50=Deur
                { 3,1,1,1,1,1,0,0,0,2, 2,2,0,0,0,1,1,1,0,0, 0,0,0,30,0,0,0,0,21,0, 0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0, 1,1,1,1,1,1,1,1,1,3 }, 
                { 3,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0, 1,1,1,1,1,0,0,2,2,0, 0,0,31,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,3 },
                { 3,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0, 1,1,1,1,0,0,0,0,0,2, 2,2,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,3 },
                { 3,0,0,0,0,0,0,0,0,0, 0,0,20,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,3 },
                { 3,1,1,1,1,1,1,1,0,0, 1,1,1,1,1,1,1,0,0,1, 1,1,1,1,1,1,0,0,0,0, 0,0,0,0,0,0,0,0,0,0, 0,0,0,0,1,1,1,1,1,0, 0,1,1,1,1,1,0,0,0,3 },
                // HIER ZAT DE FOUT: Nu 60 items (waren er 50)
                { 3,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,10,10,1, 1,1,1,1,1,10,10,10,10,10, 10,10,10,10,10,10,10,10,10,1, 1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,3 }, 
                { 3,3,3,3,3,3,3,3,3,3, 3,3,3,3,3,3,3,3,3,3, 3,3,3,3,3,3,3,3,3,3, 3,3,3,3,3,3,3,3,3,3, 3,3,3,3,3,3,3,3,3,3, 3,3,3,3,3,3,3,3,3,3 }
            };

            // Level afmetingen instellen
            Width = gameboard.GetLength(1) * 64;
            Height = gameboard.GetLength(0) * 64;

            for (int y = 0; y < gameboard.GetLength(0); y++)
            {
                for (int x = 0; x < gameboard.GetLength(1); x++)
                {
                    int tileId = gameboard[y, x];
                    Vector2 pos = new Vector2(x * 64, y * 64);

                    // 1. Blokken (via BlockFactory)
                    Block b = BlockFactory.CreateBlock(tileId, x, y, tiles);
                    if (b != null)
                    {
                        Blocks.Add(b);
                        continue;
                    }

                    // 2. Vijanden (via de NIEUWE EnemyFactory)
                    // We geven 'content' mee zodat de factory zelf textures laadt.
                    Enemy e = EnemyFactory.CreateEnemy(tileId, pos, content, graphics);
                    if (e != null)
                    {
                        Enemies.Add(e);
                        continue;
                    }

                    // 3. Overige Objecten (Handmatig of via nog een andere factory)
                    switch (tileId)
                    {
                        case 99: // Start
                            StartPosition = pos;
                            break;

                        case 10: // Spikes
                            SpikesObjects.Add(new Spikes(objSheet, new Vector2(pos.X, pos.Y + 14)));
                            break;

                        case 11: // Blade
                            Blades.Add(new SpinningBlade(objSheet, pos));
                            break;

                        case 20: // Sleutel
                            Items.Add(new KeyItem(keyTex, pos));
                            break;

                        case 21: // Hartje
                            Items.Add(new HeartItem(itemSheet, pos));
                            break;

                        case 50: // Deur
                            ExitDoor = new Door(objSheet, objSheet, new Vector2(pos.X, pos.Y - 5));
                            break;
                    }
                }
            }
        }
    }
}