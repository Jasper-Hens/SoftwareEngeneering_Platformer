using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using test.Blocks;
using test.block_Interfaces;

namespace test
{
    public abstract class Enemy
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public bool FacingRight = false;

        // Status
        public bool IsDead = false;
        public bool ReadyToRemove { get; protected set; } = false; // NIEUW: Voor verdwijnen

        public int MaxHealth { get; protected set; }
        public int CurrentHealth { get; protected set; }

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

        public virtual void Update(GameTime gameTime, List<Block> blocks, Hero hero)
        {
            // OUDE FOUT: if (IsDead) return; <--- VERWIJDER DEZE REGEL!
            // We moeten blijven updaten om de Death animatie af te spelen.

            // 1. Visuals
            if (_invincibilityTimer > 0)
            {
                _invincibilityTimer -= gameTime.ElapsedGameTime.TotalMilliseconds;
                _color = (_invincibilityTimer % 200 < 100) ? Color.Red : Color.White;
            }
            else _color = Color.White;

            // 2. Physics
            Velocity.Y += _gravity;

            UpdateAI(gameTime, hero, blocks); // Boss logica

            // 3. Beweging
            Position.X += Velocity.X;
            ResolveCollisionsX(blocks);

            Position.Y += Velocity.Y;
            ResolveCollisionsY(blocks);

            UpdateHitbox();
        }

        public virtual void TakeDamage(int damage)
        {
            if (_invincibilityTimer <= 0 && !IsDead)
            {
                CurrentHealth -= damage;
                _invincibilityTimer = 500;
                if (CurrentHealth <= 0)
                {
                    CurrentHealth = 0;
                    IsDead = true;
                    // Let op: De KnightBoss klasse zet de State straks op 'Death'
                }
            }
        }

        protected abstract void UpdateAI(GameTime gameTime, Hero hero, List<Block> blocks);
        protected abstract void UpdateHitbox();
        public abstract void Draw(SpriteBatch sb);

        // --- COLLISION ---
        protected void ResolveCollisionsX(List<Block> blocks)
        {
            UpdateHitbox();
            foreach (var block in blocks)
            {
                // Alleen botsen tegen SOLID blokken (Muren), niet tegen Platforms (waar je doorheen loopt)
                if (block is ISolid && Hitbox.Intersects(block.BoundingBox))
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