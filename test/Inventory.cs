using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace test
{
    // Een klein hulp-objectje voor items in je zak
    public class InventoryItem
    {
        public Texture2D Texture;
        public Rectangle SourceRect;

        public InventoryItem(Texture2D texture, Rectangle sourceRect)
        {
            Texture = texture;
            SourceRect = sourceRect;
        }
    }

    public class Inventory
    {
        // 1. De lijst met spullen die we hebben (voor de HUD)
        public List<InventoryItem> Items { get; private set; } = new List<InventoryItem>();

        // 2. Specifieke check voor de deur (zodat je logica niet breekt)
        public bool HasKey { get; private set; } = false;

        public void AddKey(Texture2D texture, Rectangle sourceRect)
        {
            HasKey = true;

            // Voeg het plaatje toe aan de lijst!
            Items.Add(new InventoryItem(texture, sourceRect));
        }

        public void RemoveKey()
        {
            HasKey = false;
            // Optioneel: Je zou hier ook het item uit de lijst kunnen halen
        }

        // voor een volgend level
        public void Clear()
        {
            Items.Clear();   // Gooi alle plaatjes weg
            HasKey = false;  // Reset de sleutel status
        }


    }
}