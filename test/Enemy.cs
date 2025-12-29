using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using test.Blocks;
using test.block_Interfaces;
using test.Interfaces; // Vergeet deze niet!

namespace test
{
    public abstract class Enemy : IDamageable
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public bool FacingRight = false;

        // IDamageable implementatie
        public bool IsDead { get; protected set; } = false;

        // Een flag om de enemy veilig uit de lijst te verwijderen in PlayingState
        public bool ReadyToRemove { get; protected set; } = false;

        // SOLID: Eigenschap om te bepalen of de speler hierop kan springen (Mario-style)
        public virtual bool IsStompable { get; protected set; } = false;

        public int MaxHealth { get; protected set; }
        public int CurrentHealth { get; protected set; }

        // Hitboxes
        public Rectangle Hitbox { get; protected set; }
        public Rectangle AttackHitbox { get; protected set; }
        public bool IsHitting { get; protected set; } = false;

        protected float _gravity = 0.5f;
        protected double _invincibilityTimer = 0;
        protected Color _color = Color.White;

        public Enemy(Vector2 startPos, int maxHealth)
        {
            Position = startPos;
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;
        }

        // SOLID: Nieuwe virtuele methode. 
        // Hierdoor hoeft PlayingState niet te weten welk type enemy het is.
        public virtual void OnPlayerHit()
        {
            // Standaard gedrag: doe niets. 
            // Specifieke vijanden (zoals EvilWizard) kunnen dit overschrijven.
        }

        public virtual void Update(GameTime gameTime, List<Block> blocks, Hero hero)
        {
            // 1. Visuals & Invincibility frames
            if (_invincibilityTimer > 0)
            {
                _invincibilityTimer -= gameTime.ElapsedGameTime.TotalMilliseconds;
                // Knipper effect als je geraakt bent
                _color = _invincibilityTimer % 200 < 100 ? Color.Red : Color.White;
            }
            else
            {
                _color = Color.White;
            }

            // 2. Physics & AI
            Velocity.Y += _gravity;

            // AI wordt geregeld door de subklassen (Template Method Pattern)
            UpdateAI(gameTime, hero, blocks);

            // 3. Beweging toepassen
            Position.X += Velocity.X;
            ResolveCollisionsX(blocks);

            Position.Y += Velocity.Y;
            ResolveCollisionsY(blocks);

            UpdateHitbox();
        }

        // IDamageable implementatie
        public virtual void TakeDamage(int damage)
        {
            if (_invincibilityTimer <= 0 && !IsDead)
            {
                CurrentHealth -= damage;
                _invincibilityTimer = 200; // Korte onsterfelijkheid na hit

                if (CurrentHealth <= 0)
                {
                    CurrentHealth = 0;
                    IsDead = true;
                }
            }
        }

        // Abstracte methodes die subklassen MOETEN invullen
        protected abstract void UpdateAI(GameTime gameTime, Hero hero, List<Block> blocks);
        protected abstract void UpdateHitbox();
        public abstract void Draw(SpriteBatch sb);

        // --- Collision Logic (Hetzelfde gebleven, werkt prima) ---
        protected void ResolveCollisionsX(List<Block> blocks)
        {
            UpdateHitbox();
            Rectangle skinnyRect = Hitbox;
            skinnyRect.Y += 4;
            skinnyRect.Height -= 8;

            foreach (var block in blocks)
            {
                if (block is ISolid && skinnyRect.Intersects(block.BoundingBox))
                {
                    Rectangle intersection = Rectangle.Intersect(Hitbox, block.BoundingBox);
                    if (Velocity.X > 0) Position.X -= intersection.Width;
                    else if (Velocity.X < 0) Position.X += intersection.Width;
                    Velocity.X = 0;
                    UpdateHitbox();
                }
            }
        }

        protected void ResolveCollisionsY(List<Block> blocks)
        {
            UpdateHitbox();
            foreach (var block in blocks)
            {
                if ((block is ISolid || block is IPlatform) && Hitbox.Intersects(block.BoundingBox))
                {
                    Rectangle intersection = Rectangle.Intersect(Hitbox, block.BoundingBox);
                    if (intersection.Height < intersection.Width)
                    {
                        if (Velocity.Y > 0)
                        {
                            if (block is IPlatform && Hitbox.Bottom - intersection.Height > block.BoundingBox.Top) return;
                            Position.Y -= intersection.Height;
                            Velocity.Y = 0;
                        }
                        else if (Velocity.Y < 0 && block is ISolid)
                        {
                            Position.Y += intersection.Height;
                            Velocity.Y = 0;
                        }
                        UpdateHitbox();
                    }
                }
            }
        }
    }
}