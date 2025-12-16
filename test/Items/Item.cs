using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace test.Items
{
    public abstract class Item
    {
        protected Texture2D _texture;
        protected Rectangle _sourceRect; //Welk stukje van de sheet?

        public Vector2 Position { get; set; }
        public bool IsActive { get; set; } = true;

        // Hitbox is nu gebaseerd op de grootte van de sourceRect 
        public Rectangle Hitbox => new Rectangle((int)Position.X, (int)Position.Y, _sourceRect.Width, _sourceRect.Height);

     
        public Item(Texture2D texture, Vector2 position, Rectangle sourceRect)
        {
            _texture = texture;
            Position = position;
            _sourceRect = sourceRect;
        }

        public abstract void OnPickup(Hero hero);

        public virtual void Draw(SpriteBatch sb)
        {
            if (IsActive)
            {
                // Teken alleen _sourceRect)
                sb.Draw(_texture, Position, _sourceRect, Color.White);
            }
        }
    }
}