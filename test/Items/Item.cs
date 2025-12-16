using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace test.Items
{
    public abstract class Item
    {
        protected Texture2D _texture;
        protected Rectangle _sourceRect;

        public Vector2 Position { get; set; }
        public bool IsActive { get; set; } = true;

        // NIEUW: De grootte variabele (1.0f = normaal, 0.5f = de helft)
        public float Scale { get; set; } = 1.0f;

        // AANGEPAST: De Hitbox wordt nu ook kleiner als de schaal kleiner is!
        public Rectangle Hitbox => new Rectangle(
            (int)Position.X,
            (int)Position.Y,
            (int)(_sourceRect.Width * Scale),
            (int)(_sourceRect.Height * Scale)
        );

        public Item(Texture2D texture, Vector2 position, Rectangle sourceRect)
        {
            _texture = texture;
            Position = position;
            _sourceRect = sourceRect;
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public abstract void OnPickup(Hero hero);

        public virtual void Draw(SpriteBatch sb)
        {
            if (IsActive)
            {
                // AANGEPAST: We gebruiken nu de uitgebreide Draw functie met 'Scale'
                sb.Draw(_texture,
                        Position,
                        _sourceRect,
                        Color.White,
                        0f,           // Rotatie
                        Vector2.Zero, // Origin
                        Scale,        // HIER WORDT HIJ VERKLEIND
                        SpriteEffects.None,
                        0f);
            }
        }
    }
}