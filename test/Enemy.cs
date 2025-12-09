// Bestand: test/Enemy.cs
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
        public bool IsDead = false;

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
            if (IsDead) return;

            if (_invincibilityTimer > 0)
            {
                _invincibilityTimer -= gameTime.ElapsedGameTime.TotalMilliseconds;
                _color = (_invincibilityTimer % 200 < 100) ? Color.Red : Color.White;
            }
            else
            {
                _color = Color.White;
            }

            Velocity.Y += _gravity;

            UpdateAI(gameTime, hero);

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
                }
            }
        }

        // =========================================================
        // ABSTRACTE METHODES (Deze MOETEN in KnightBoss staan!)
        // =========================================================
        protected abstract void UpdateAI(GameTime gameTime, Hero hero);
        protected abstract void UpdateHitbox();
        public abstract void Draw(SpriteBatch sb);

        // --- COLLISION LOGICA ---
        protected void ResolveCollisionsX(List<Block> blocks)
        {
            UpdateHitbox();
            foreach (var block in blocks)
            {
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