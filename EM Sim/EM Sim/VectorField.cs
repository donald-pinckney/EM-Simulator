using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace EM_Sim
{
    public class VectorField
    {
        private List<Arrow> arrows = new List<Arrow>();
        public bool shouldDrawVectors = true;
        protected GraphicsDevice device;
        public VectorField(GraphicsDevice d)
        {
            device = d;
        }

        public VectorField()
        {

        }

        float startX = -10;
        float startY = -10;
        float startZ = -10;

        float endX = 10;
        float endY = 10;
        float endZ = 10;
        public void SetBounds(float startX, float startY, float startZ, float endX, float endY, float endZ)
        {
            this.startX = startX;
            this.startY = startY;
            this.startZ = startZ;

            this.endX = endX;
            this.endY = endY;
            this.endZ = endZ;

            //generateArrows();
        }


        public void generateArrows()
        {
            arrows = new List<Arrow>();

            float xRes = 6f / 20f;
            float yRes = 6f / 20f;
            float zRes = 6f / 20f;
            int xNumArrows = (int)(xRes * (endX - startX));
            int yNumArrows = (int)(yRes * (endY - startY));
            int zNumArrows = (int)(zRes * (endZ - startZ));

            for (int x = 0; x <= xNumArrows; x++)
            {
                for (int z = 0; z <= zNumArrows; z++)
                {
                    for (int y = 0; y <= yNumArrows; y++)
                    {
                        float xCoord = MathHelper.Lerp(startX, endX, (float)x / (float)xNumArrows);
                        float yCoord = MathHelper.Lerp(startY, endY, (float)y / (float)yNumArrows);
                        float zCoord = MathHelper.Lerp(startZ, endZ, (float)z / (float)zNumArrows);

                        Vector3 pos = new Vector3(xCoord, yCoord, zCoord);
                        Vector3 vec = r(pos) * 0.2f;
                        if (vec.Length() > 50) continue;
                        Arrow arrow = Arrow.ArrowWithStartAndVector(device, pos, vec);
                        arrows.Add(arrow);
                    }
                }
            }
        }




        public virtual Vector3 r(float x, float y, float z)
        {
            return Vector3.UnitY;
        }

        protected Vector3 r(Vector3 pos)
        {
            return r(pos.X, pos.Y, pos.Z);
        }


        public virtual void Draw(Matrix view, Matrix proj)
        {
            if (shouldDrawVectors)
            {
                for(int i = 0; i < arrows.Count; i++)
                {
                    Arrow arrow = arrows[i];
                    arrow.Draw(view, proj);
                }
            }
        }
    }
}