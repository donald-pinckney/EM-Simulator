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

        List<Arrow> xDirArrows = new List<Arrow>(); // Contains grey grid arrows (lines) parallel to x-axis
        List<Arrow> zDirArrows = new List<Arrow>(); // Contains grey grid arrows (lines) parallel to z-axis

        public Grid(GraphicsDevice d, float scale = 1)
        {
            float xLen = 40;
            float yLen = 40;
            float zLen = 40;

            xArrow = Arrow.ArrowWithStartAndEnd(d, new Vector3(-xLen, 0, 0), new Vector3(xLen, 0, 0));
            yArrow = Arrow.ArrowWithStartAndEnd(d, new Vector3(0, -yLen, 0), new Vector3(0, yLen, 0));
            zArrow = Arrow.ArrowWithStartAndEnd(d, new Vector3(0, 0, -zLen), new Vector3(0, 0, zLen));

            xArrow.color = new Color(94, 118, 27).ToVector3();
            yArrow.color = new Color(94, 118, 27).ToVector3();
            zArrow.color = new Color(94, 118, 27).ToVector3();

            xArrow.isColored = true;
            yArrow.isColored = true;
            zArrow.isColored = true;

            xArrow.hasArrowhead = true;
            zArrow.hasArrowhead = true;

            // Generate grey grid arrows parallel to x-axis
            Vector3 gridColor = new Color(0, 31, 39).ToVector3();
            for (int z = -(int)zLen; z <= zLen; z++)
            {
                if (z == 0) continue;

                Arrow arrow = Arrow.ArrowWithStartAndEnd(d, new Vector3(-xLen, 0, z), new Vector3(xLen, 0, z));
                arrow.color = gridColor;
                arrow.isColored = true;
                xDirArrows.Add(arrow);
            }

            // Generate grey grid arrows parallel to z-axis
            for (int x = -(int)xLen; x <= xLen; x++)
            {
                if (x == 0) continue;

                Arrow arrow = Arrow.ArrowWithStartAndEnd(d, new Vector3(x, 0, -zLen), new Vector3(x, 0, zLen));
                arrow.color = gridColor;
                arrow.isColored = true;
                zDirArrows.Add(arrow);
            }

        }


        public void Draw(Matrix view, Matrix proj)
        {
            xArrow.Draw(view, proj);
            yArrow.Draw(view, proj);
            zArrow.Draw(view, proj);

            foreach (Arrow arrow in xDirArrows)
            {
                arrow.Draw(view, proj);
            }

            foreach (Arrow arrow in zDirArrows)
            {
                arrow.Draw(view, proj);
            }
        }
    }
}
