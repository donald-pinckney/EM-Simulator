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
    class Arrow
    {
        GraphicsDevice device;
        public static BasicEffect effect;

        private Vector3 startPoint;
        private Vector3 endPoint;

        public Vector3 color = new Vector3(1, 1, 1);
        public bool isColored = false;
        private bool _hasArrowhead = false;
        public bool hasArrowhead
        {
            get { return _hasArrowhead; }
            set
            {
                _hasArrowhead = value;
                MakeVertices(startPoint, endPoint);
            }
        }

        public static void InitEffect(GraphicsDevice device)
        {
            effect = new BasicEffect(device);
            effect.LightingEnabled = false;
        }

        private Arrow(GraphicsDevice d, Vector3 start, Vector3 end)
        {
            device = d;

            startPoint = start;
            endPoint = end;
            MakeVertices(start, end);
        }


        public static Arrow ArrowWithStartAndEnd(GraphicsDevice device, Vector3 start, Vector3 end)
        {
            return new Arrow(device, start, end);
        }

        public static Arrow ArrowWithStartAndVector(GraphicsDevice device, Vector3 start, Vector3 vector)
        {
            return new Arrow(device, start, start + vector);
        }


        public static Arrow ArrowWithStartAndDirectionAndMagnitude(GraphicsDevice device, Vector3 direction, float magnitude, Vector3 start)
        {
            return new Arrow(device, start, start + direction * magnitude);
        }

        VertexPositionColor[] lineVerts;

        private void MakeVertices(Vector3 startPoint, Vector3 endPoint)
        {
            // Create vertices for the line of the arrow
            if (hasArrowhead)
            {
                lineVerts = new VertexPositionColor[6];
                lineVerts[0] = new VertexPositionColor(startPoint, new Color(100, 100, 100));
                lineVerts[1] = new VertexPositionColor(endPoint, Color.White);

                Vector3 leftCross = Vector3.Normalize(Vector3.Cross(endPoint - startPoint, -Vector3.UnitY));
                Vector3 rightCross = Vector3.Normalize(Vector3.Cross(endPoint - startPoint, Vector3.UnitY));
                Vector3 smallerArrowshaft = Vector3.Normalize(startPoint - endPoint);
                Vector3 leftArrowTipVector = smallerArrowshaft + leftCross;
                Vector3 rightArrowTipVector = smallerArrowshaft + rightCross;
                lineVerts[2] = new VertexPositionColor(endPoint, Color.White);
                lineVerts[3] = new VertexPositionColor(endPoint + leftArrowTipVector, Color.White);
                lineVerts[4] = new VertexPositionColor(endPoint, Color.White);
                lineVerts[5] = new VertexPositionColor(endPoint + rightArrowTipVector, Color.White);
            }
            else
            {
                lineVerts = new VertexPositionColor[2];
                lineVerts[0] = new VertexPositionColor(startPoint, new Color(100, 100, 100));
                lineVerts[1] = new VertexPositionColor(endPoint, Color.White);
            }
        }


        public void Draw(Matrix view, Matrix proj)
        {
            effect.CurrentTechnique = effect.Techniques[0];
            effect.View = view;
            effect.Projection = proj;
            effect.VertexColorEnabled = true;
            effect.DiffuseColor = color;

            if (isColored)
            {
                effect.VertexColorEnabled = false;
            }

            if (hasArrowhead)
            {
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    device.DrawUserPrimitives(PrimitiveType.LineList, lineVerts, 0, 3);
                }
            }
            else
            {
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    device.DrawUserPrimitives(PrimitiveType.LineList, lineVerts, 0, 1);
                }
            }
        }
    }
}
