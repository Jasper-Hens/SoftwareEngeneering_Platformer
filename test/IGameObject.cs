using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test
{
    internal interface IGameObject
    {
        public void Update(GameTime gameTime);

        public void Draw(SpriteBatch SpriteBatch);
    }
}
