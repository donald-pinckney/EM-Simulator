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
    class Magnet
    {
        private VertexBuffer vertBuffer;
        private IndexBuffer indexBuffer;

        private Effect magnetEffect;
        private GraphicsDevice device;

        public Magnet(GraphicsDevice d, ContentManager Content)
        {
            device = d;
            // Vertex generation code for a bar magnet (rectangular prism)
            vertBuffer = new VertexBuffer(device, VertexPositionPolarity.VertexDeclaration, 8, BufferUsage.WriteOnly);
            VertexPositionPolarity[] verts = new VertexPositionPolarity[8];

            verts[0] = new VertexPositionPolarity(new Vector3(-0.2f, -0.2f, -1.0f), 1);
            verts[1] = new VertexPositionPolarity(new Vector3(0.2f, -0.2f, -1.0f), 1);
            verts[2] = new VertexPositionPolarity(new Vector3(0.2f, 0.2f, -1.0f), 1);
            verts[3] = new VertexPositionPolarity(new Vector3(-0.2f, 0.2f, -1.0f), 1);

            verts[4] = new VertexPositionPolarity(new Vector3(-0.2f, -0.2f, 1.0f), -1);
            verts[5] = new VertexPositionPolarity(new Vector3(0.2f, -0.2f, 1.0f), -1);
            verts[6] = new VertexPositionPolarity(new Vector3(0.2f, 0.2f, 1.0f), -1);
            verts[7] = new VertexPositionPolarity(new Vector3(-0.2f, 0.2f, 1.0f), -1);

            vertBuffer.SetData(verts);


            indexBuffer = new IndexBuffer(device, IndexElementSize.SixteenBits, 36, BufferUsage.WriteOnly);
            short[] indices = new short[36];

            // Back side
            indices[0] = 0;
            indices[1] = 2;
            indices[2] = 3;

            indices[3] = 0;
            indices[4] = 1;
            indices[5] = 2;


            // Front side
            indices[6] = 4;
            indices[7] = 7;
            indices[8] = 6;

            indices[9] = 4;
            indices[10] = 6;
            indices[11] = 5;


            // Right side
            indices[12] = 5;
            indices[13] = 6;
            indices[14] = 2;

            indices[15] = 5;
            indices[16] = 2;
            indices[17] = 1;


            // Left side
            indices[18] = 0;
            indices[19] = 3;
            indices[20] = 7;

            indices[21] = 0;
            indices[22] = 7;
            indices[23] = 4;


            // Top side
            indices[24] = 7;
            indices[25] = 3;
            indices[26] = 2;

            indices[27] = 7;
            indices[28] = 2;
            indices[29] = 6;


            // Bottom side
            indices[30] = 0;
            indices[31] = 4;
            indices[32] = 5;

            indices[33] = 0;
            indices[34] = 5;
            indices[35] = 1;


            indexBuffer.SetData(indices);

            magnetEffect = Content.Load<Effect>("magnet");
        }

        public void Draw(Matrix view, Matrix proj)
        {
            magnetEffect.CurrentTechnique = magnetEffect.Techniques[0];
            magnetEffect.Parameters["xWorld"].SetValue(Matrix.Identity);
            magnetEffect.Parameters["xView"].SetValue(view);
            magnetEffect.Parameters["xProjection"].SetValue(proj);
            magnetEffect.Parameters["xNorthColor"].SetValue(Color.Red.ToVector4());
            magnetEffect.Parameters["xSouthColor"].SetValue(Color.Blue.ToVector4());

            foreach (EffectPass pass in magnetEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                device.Indices = indexBuffer;
                device.SetVertexBuffer(vertBuffer);
                device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 8, 0, 12);
            }
        }
    }
}
