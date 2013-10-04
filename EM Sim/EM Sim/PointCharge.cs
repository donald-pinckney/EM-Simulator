using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace EM_Sim
{
    public class PointCharge
    {
        VertexBuffer vertBuffer;
        IndexBuffer indexBuffer;
        GraphicsDevice device;
        BasicEffect effect;

        int numVertices;
        int numPrimitives;

        public int id;

        public float charge;
        public float radius;
        public Vector3 center;

        public PointCharge(GraphicsDevice d, float radius, Vector3 c, float chargeInMicroCoulombs, int id)
        {
            //chargeInMicroCoulombs *= 1000000f;

            device = d;

            this.radius = radius;
            center = c;
            CreateSphere(radius, center);

            effect = new BasicEffect(device);

            this.charge = chargeInMicroCoulombs * 0.000001f;

            if (charge > 0)
            {
                effect.DiffuseColor = new Vector3(1.0f, 0.0f, 0.0f);
            }
            else if (charge < 0)
            {
                effect.DiffuseColor = new Vector3(0.0f, 0.0f, 1.0f);
            }
            else
            {
                effect.DiffuseColor = new Vector3(1.0f, 1.0f, 1.0f);
            }
            effect.LightingEnabled = true;
            effect.AmbientLightColor = new Vector3(0.2f, 0.2f, 0.2f);

            this.id = id;
        }

        protected void CreateSphere(float radius, Vector3 center)
        {
            int slices = 20;
            int stacks = 20;

            VertexPositionNormalTexture[] verts = new VertexPositionNormalTexture[(slices + 1) * (stacks + 1)];

            float phi, theta;
            float dphi = MathHelper.Pi / stacks;
            float dtheta = MathHelper.TwoPi / slices;
            float x, y, z, sc;
            int index = 0;

            for (int stack = 0; stack <= stacks; stack++)
            {
                phi = MathHelper.PiOver2 - stack * dphi;
                y = radius * (float)Math.Sin(phi);
                sc = -radius * (float)Math.Cos(phi);

                for (int slice = 0; slice <= slices; slice++)
                {
                    theta = slice * dtheta;
                    x = sc * (float)Math.Sin(theta);
                    z = sc * (float)Math.Cos(theta);
                    verts[index++] = new VertexPositionNormalTexture(new Vector3(x + center.X, y + center.Y, z + center.Z),
                                                                    new Vector3(x, y, z),
                                                                    new Vector2((float)slice / (float)slices, (float)stack / (float)stacks));
                }
            }

            short[] indices = new short[slices * stacks * 6];
            index = 0;
            int k = slices + 1;

            for (int stack = 0; stack < stacks; stack++)
            {
                for (int slice = 0; slice < slices; slice++)
                {
                    indices[index++] = (short)((stack + 0) * k + slice);
                    indices[index++] = (short)((stack + 1) * k + slice);
                    indices[index++] = (short)((stack + 0) * k + slice + 1);

                    indices[index++] = (short)((stack + 0) * k + slice + 1);
                    indices[index++] = (short)((stack + 1) * k + slice);
                    indices[index++] = (short)((stack + 1) * k + slice + 1);
                }
            }


            vertBuffer = new VertexBuffer(device, VertexPositionNormalTexture.VertexDeclaration, verts.Length, BufferUsage.WriteOnly);
            vertBuffer.SetData(verts);
            numVertices = verts.Length;

            indexBuffer = new IndexBuffer(device, IndexElementSize.SixteenBits, indices.Length, BufferUsage.WriteOnly);
            indexBuffer.SetData(indices);
            numPrimitives = indices.Length / 3;

        }

        public void Draw(Matrix view, Matrix proj)
        {
            device.SetVertexBuffer(vertBuffer);
            device.Indices = indexBuffer;

            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.CullClockwiseFace;
            device.RasterizerState = rs;

            effect.CurrentTechnique = effect.Techniques[0];
            effect.View = view;
            effect.Projection = proj;
            


            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, numVertices, 0, numPrimitives);
            }

            rs = new RasterizerState();
            rs.CullMode = CullMode.CullCounterClockwiseFace;
            device.RasterizerState = rs;
        }

        public override String ToString()
        {
            return String.Format("ID {0}: Pos = {1}  Charge = {2}\u00b5C", new object[] { this.id, this.center.ToString(), this.charge * 1000000f });
        }
    }
}
