using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace EM_Sim
{

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        GraphicsDevice device;

        Grid grid;
        Camera cam;

        PointCharge testCharge;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            device = this.GraphicsDevice;

            cam = new Camera(new Vector3(-5, 5, 5), -MathHelper.PiOver4, -MathHelper.PiOver4, 0.001f, 0.1f, device);
            

            // Setup axes grid 
            grid = new Grid(device, 0.1f);

            // Test charge
            testCharge = new PointCharge(device, 1.0f, -1.0f);
        }
        protected override void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.Escape))
                this.Exit();

            cam.UpdateCam(1.0f);


            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(ClearOptions.DepthBuffer | ClearOptions.Target, Color.Black, 1.0f, 0);

            testCharge.Draw(cam.GetViewMatrix(), cam.GetProjMatrix());

            grid.Draw(cam.GetViewMatrix(), cam.GetProjMatrix());

            base.Draw(gameTime);
        }
    }
}
