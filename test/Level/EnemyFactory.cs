using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace test.Level
{
    public static class EnemyFactory
    {
        public static Enemy CreateEnemy(int type, Vector2 position, ContentManager content, GraphicsDevice graphics)
        {
            switch (type)
            {
                case 30: // Demon
                    Texture2D dWalk = content.Load<Texture2D>("Demon2/Walk");
                    Texture2D dAttack = content.Load<Texture2D>("Demon2/Attack");
                    Texture2D dDeath = content.Load<Texture2D>("Demon2/Dead");
                    // Eventueel Y-positie corrigeren als hij in de grond zakt
                    return new Demon2(dWalk, dAttack, dDeath, position);

                case 31: // Evil Wizard
                    Texture2D wizIdle = content.Load<Texture2D>("EvilWizard/Idle");
                    Texture2D wizAtk = content.Load<Texture2D>("EvilWizard/Attack");
                    Texture2D wizDeath = content.Load<Texture2D>("EvilWizard/Death");
                    return new EvilWizard(wizIdle, wizAtk, wizDeath, position);

                case 35: // Boss (KnightBoss)
                    // Voor de boss hebben we die witte pixel texture nodig
                    Texture2D blokTex = new Texture2D(graphics, 1, 1);
                    blokTex.SetData(new[] { Color.White });

                    Texture2D bIdle = content.Load<Texture2D>("Boss/Idle");
                    Texture2D bWalk = content.Load<Texture2D>("Boss/Walk");
                    Texture2D bRun = content.Load<Texture2D>("Boss/Run");
                    Texture2D bJump = content.Load<Texture2D>("Boss/Jump");
                    Texture2D bAtk1 = content.Load<Texture2D>("Boss/Attack");
                    Texture2D bAtk2 = content.Load<Texture2D>("Boss/Attack2");
                    Texture2D bAtk3 = content.Load<Texture2D>("Boss/Attack3");
                    Texture2D bWAtk = content.Load<Texture2D>("Boss/Walk_Attack");
                    Texture2D bSpec = content.Load<Texture2D>("Boss/Special");
                    Texture2D bHurt = content.Load<Texture2D>("Boss/Hurt");
                    Texture2D bDeath = content.Load<Texture2D>("Boss/Death");

                    return new KnightBoss(position, blokTex, bIdle, bWalk, bRun, bJump, bAtk1, bAtk2, bAtk3, bWAtk, bSpec, bHurt, bDeath);

                default:
                    return null; // Geen vijand voor dit nummer
            }
        }
    }
}