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
    public class EField : VectorField
    {
        private List<PointCharge> charges = new List<PointCharge>();

        private VertexPositionColor[] eFieldLineVerts;

        BasicEffect lineEffect;

        int idCounter = 0;

        public bool shouldDrawLines = true;

        public EField(GraphicsDevice d) : base(d) 
        {
            lineEffect = new BasicEffect(d);
            lineEffect.VertexColorEnabled = true;
        }

        public List<PointCharge> GetCharges()
        {
            return charges;
        }

        public void DeleteChargeWithID(int id)
        {
            for (int i = 0; i < charges.Count; i++)
            {
                PointCharge c = charges[i];
                if (c.id == id)
                {
                    charges.RemoveAt(i);
                    generateArrows();
                    generateEFieldLines();
                    return;
                }
            }
        }

        private void AddCharge(PointCharge charge)
        {
            idCounter++;
            charges.Add(charge);

            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float minZ = float.MaxValue;

            float maxX = float.MinValue;
            float maxY = float.MinValue;
            float maxZ = float.MinValue;

            for (int i = 0; i < charges.Count; i++)
            {
                PointCharge c = charges[i];
                if (c.center.X < minX) minX = c.center.X;
                if (c.center.Y < minY) minY = c.center.Y;
                if (c.center.Z < minZ) minZ = c.center.Z;

                if (c.center.X > maxX) maxX = c.center.X;
                if (c.center.Y > maxY) maxY = c.center.Y;
                if (c.center.Z > maxZ) maxZ = c.center.Z;
            }

            minX -= 5;
            minY -= 5;
            minZ -= 5;

            maxX += 5;
            maxY += 5;
            maxZ += 5;
            this.SetBounds(minX, minY, minZ, maxX, maxY, maxZ);
        }

        public void AddCharge(Vector3 pos, float chargeInMicroCoulombs)
        {
            PointCharge pointCharge = new PointCharge(this.device, 1.0f, pos, chargeInMicroCoulombs, idCounter);
            AddCharge(pointCharge);
        }

        public void generateEFieldLines()
        {
            if (charges.Count == 0)
            {
                eFieldLineVerts = null;
                return;
            }

            int numLinesPerCharge = 25;


            List<VertexPositionColor> tempVerts = new List<VertexPositionColor>();

            Random random = new Random();
            int lineSegmentsPerLine = 65535 / (charges.Count * numLinesPerCharge);
            for (int i = 0; i < charges.Count; i++)
            {
                // Non-random line starting algorithm
                PointCharge charge = charges[i];

                float minLon = 0;
                float maxLon = (float)Math.PI;

                float minLat = (float)-Math.PI;
                float maxLat = (float)Math.PI;

                int numLoops = (int)Math.Sqrt(numLinesPerCharge);

                for (int lonIndex = 0; lonIndex < numLoops; lonIndex++)
                {
                    float lon = MathHelper.Lerp(minLon, maxLon, (float)lonIndex / (float)numLoops);
                    for (int latIndex = 0; latIndex < numLoops; latIndex++)
                    {
                        float lat = MathHelper.Lerp(minLat, maxLat, (float)latIndex / (float)numLoops);
                        Vector3 startOffset = 2 * charge.radius * Vector3.Transform(Vector3.UnitX, Matrix.CreateRotationY(lon) * Matrix.CreateRotationX(lat));
                        Vector3 startPos = charge.center + startOffset;

                        VertexPositionColor[] line = calculateVerticesForEFieldLineFromStartPosition(startPos, charge.charge < 0, lineSegmentsPerLine);
                        tempVerts.AddRange(line);
                    }
                }


                // Random line starting algorithm
                /*PointCharge charge = charges[i];
                for (int n = 0; n < numLinesPerCharge; n++)
                {
                    float lon = (float)(random.NextDouble() * Math.PI);
                    float lat = (float)(random.NextDouble() * MathHelper.TwoPi - Math.PI);

                    Vector3 startOffset = 2 * charge.radius * Vector3.Transform(Vector3.UnitX, Matrix.CreateRotationY(lon) * Matrix.CreateRotationX(lat));
                    Vector3 startPos = charge.center + startOffset;

                    VertexPositionColor[] line = calculateVerticesForEFieldLineFromStartPosition(startPos, charge.charge < 0, lineSegmentsPerLine);
                    tempVerts.AddRange(line);
                }*/

            }

            eFieldLineVerts = tempVerts.ToArray();
        }


        private VertexPositionColor[] calculateVerticesForEFieldLineFromStartPosition(Vector3 startPos, bool isNegative, int numSegments)
        {
            List<VertexPositionColor> tempLine = new List<VertexPositionColor>();

            float lineDistance = 2.0f; // Arbitrary "distance" covered by the line
            float scale = lineDistance / numSegments;
            Vector3 currentVector = startPos;
            for (int i = 0; i < numSegments; i++)
            {
                Vector3 offsetVector = r(currentVector) * scale * (isNegative ? -1 : 1);
                Vector3 newVector = currentVector + offsetVector;
                tempLine.Add(new VertexPositionColor(currentVector, Color.Green));
                tempLine.Add(new VertexPositionColor(newVector, Color.Green));
                //Console.WriteLine(offsetVector);
                currentVector = newVector;
            }

            Console.WriteLine("\n\n");

            return tempLine.ToArray();
        }


        private const float k = 9000000000f;
        public override Vector3 r(float x, float y, float z)
        {
            Vector3 pos = new Vector3(x, y, z);

            Vector3 sum = Vector3.Zero;

            for (int i = 0; i < charges.Count; i++)
            {
                PointCharge charge = charges[i];
                float distance = Vector3.Distance(pos, charge.center);

                float magnitude = -k * charge.charge / (distance * distance);

                Vector3 vec = charge.center - pos;
                vec.Normalize();
                vec *= magnitude;
                sum += vec;
            }


            return sum;
        }

        public override void Draw(Matrix view, Matrix proj)
        {
            base.Draw(view, proj);

            // Draw charges
            for (int i = 0; i < charges.Count; i++)
            {
                charges[i].Draw(view, proj);
            }

            if (shouldDrawLines)
            {
                if (eFieldLineVerts != null)
                {
                    lineEffect.CurrentTechnique = lineEffect.Techniques[0];
                    lineEffect.View = view;
                    lineEffect.Projection = proj;
                    foreach (EffectPass pass in lineEffect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        device.DrawUserPrimitives(PrimitiveType.LineList, eFieldLineVerts, 0, eFieldLineVerts.Length / 2);
                    }
                }
            }
        }
    }
}
