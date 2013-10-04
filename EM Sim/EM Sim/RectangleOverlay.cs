using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace EM_Sim
{
    public class RectangleOverlay
    {
        Texture2D dummyTexture;
        Rectangle dummyRectangle;
        Rectangle dummyFullRect;
        GraphicsDevice device;
        Color color;

        public RectangleOverlay(Rectangle rect, Rectangle fullRect, Color color, GraphicsDevice d)
        {
            device = d;
            dummyRectangle = rect;
            dummyFullRect = fullRect;
            this.color = color;
        }

        public void LoadContent()
        {
            dummyTexture = new Texture2D(device, 1, 1);
            dummyTexture.SetData(new Color[] { Color.White });
        }

        public void Draw(SpriteBatch batch)
        {
            if (device.PresentationParameters.IsFullScreen)
            {
                batch.Draw(dummyTexture, dummyFullRect, color);
            }
            else
            {
                batch.Draw(dummyTexture, dummyRectangle, color);
            }
        }
    }
}
