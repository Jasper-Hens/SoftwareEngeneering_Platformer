using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace test.Blocks
{
    public class Block
    {
        // De rechthoek in de wereld. Dit is de HITBOX en de positie.
        public Rectangle BoundingBox { get; private set; }

        // De rechthoek op de spritesheet. Dit is de UITSNEDE.
        public Rectangle SourceRectangle { get; private set; }

        // De volledige tilesheet texture
        private Texture2D _texture;

        // Het Block is standaard niet passeerbaar (solide).
        public bool IsPassable { get; private set; } = false;

        /// <summary>
        /// Constructor voor een level-blok.
        /// </summary>
        /// <param name="worldRect">De positie en grootte in de game wereld (de hitbox).</param>
        /// <param name="sourceRect">De uitsnede op de tilesheet.</param>
        /// <param name="texture">De tilesheet Texture2D.</param>
        public Block(Rectangle worldRect, Rectangle sourceRect, Texture2D texture)
        {
            this.BoundingBox = worldRect;
            this.SourceRectangle = sourceRect;
            this._texture = texture;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Gebruik de overload die zowel de Source (uitsnede) als de Destination (wereld-positie) gebruikt.
            // Dit tekent de tile (SourceRectangle) op de plaats van de hitbox (BoundingBox).
            spriteBatch.Draw(_texture, BoundingBox, SourceRectangle, Color.White);
        }

        // Update methode voor eventuele animatie of beweging
        public virtual void Update(GameTime gameTime)
        {
            // Voor statische blokken blijft deze leeg
        }
    }
}