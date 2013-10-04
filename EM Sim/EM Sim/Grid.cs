using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace EM_Sim
{
    class Grid
    {

        Arrow xArrow;
        Arrow yArrow;
        Arrow zArrow;


        public Grid(GraphicsDevice d, float scale = 1)
        {
            float xLen = 40;
            float yLen = 40;
            float zLen = 40;

            xArrow = Arrow.ArrowWithStartAndEnd(d, new Vector3(-xLen, 0, 0), new Vector3(xLen, 0, 0));
            yArrow = Arrow.ArrowWithStartAndEnd(d, new Vector3(0, -yLen, 0), new Vector3(0, yLen, 0));
            zArrow = Arrow.ArrowWithStartAndEnd(d, new Vector3(0, 0, -zLen), new Vector3(0, 0, zLen));


            xArrow.color = new Vector3(1, 0, 0);
            yArrow.color = new Vector3(1, 0, 0);
            zArrow.color = new Vector3(1, 0, 0);

            xArrow.isColored = true;
            yArrow.isColored = true;
            zArrow.isColored = true;

        }


        public void Draw(Matrix view, Matrix proj)
        {
            xArrow.Draw(view, proj);
            yArrow.Draw(view, proj);
            zArrow.Draw(view, proj);
        }
    }
}
