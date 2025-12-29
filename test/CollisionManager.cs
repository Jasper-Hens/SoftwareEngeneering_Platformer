using Microsoft.Xna.Framework;
using System.Collections.Generic;
using test.Interfaces; // Voor IDamageable

namespace test.Level
{
    public static class CollisionManager
    {
        // Deze methode roep je 1x aan in PlayingState.Update
        public static void HandleCombat(Hero hero, List<Enemy> enemies)
        {
            if (hero.IsDead) return;

            // We itereren achteruit omdat enemies soms doodgaan (hoewel we ze nu via ReadyToRemove wissen)
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                Enemy enemy = enemies[i];
                if (enemy.IsDead) continue;

                // 1. Speler slaat vijand (AttackHitbox vs Enemy Hitbox)
                if (hero.IsHitting)
                {
                    if (hero.AttackHitbox.Intersects(enemy.Hitbox))
                    {
                        enemy.TakeDamage(20); // Of haal damage uit Hero stats
                    }
                }

                // 2. Vijand slaat speler (Enemy AttackHitbox vs Player Hitbox)
                if (enemy.IsHitting)
                {
                    // Check of speler niet aan het rollen is (invincibility frame tijdens roll)
                    if (!hero.IsRolling && enemy.AttackHitbox.Intersects(hero.Hitbox))
                    {
                        hero.TakeDamage(1);

                        // SOLID: Roep de specifieke logica van de vijand aan (bijv. Wizard cooldown)
                        enemy.OnPlayerHit();
                    }
                }

                // 3. Stomp Mechanic (Speler springt bovenop vijand)
                HandleStompMechanic(hero, enemy);
            }
        }

        private static void HandleStompMechanic(Hero hero, Enemy enemy)
        {
            // Speler moet naar beneden vallen
            if (hero.Velocity.Y > 0)
            {
                if (hero.Hitbox.Intersects(enemy.Hitbox))
                {
                    // Check: Zijn de voeten van de speler hoger dan de onderkant van de vijand?
                    // (Ongeveer bovenop)
                    if (hero.Hitbox.Bottom < enemy.Hitbox.Bottom)
                    {
                        if (enemy.IsStompable)
                        {
                            enemy.TakeDamage(enemy.MaxHealth); // Instant kill (of damage)
                            hero.Velocity.Y = -12f; // Stuiter omhoog
                        }
                    }
                }
            }
        }
    }
}