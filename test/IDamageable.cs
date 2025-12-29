using Microsoft.Xna.Framework;

namespace test.Interfaces
{
    public interface IDamageable
    {
        Rectangle Hitbox { get; }
        void TakeDamage(int damage);
        bool IsDead { get; }
    }
}