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
using EventInput;
using System.IO;

namespace EM_Sim
{

    public class EMSim : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        GraphicsDevice device;

        Grid grid;
        EField field;

        Camera cam;

        EMConsole console;

        int windowedWidth;
        int windowedHeight;

        public int fullscreenWidth;
        public int fullscreenHeight;

        RenderTarget2D screenshotRender;

        public EMSim()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            windowedWidth = graphics.PreferredBackBufferWidth;
            windowedHeight = graphics.PreferredBackBufferHeight;

            fullscreenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            fullscreenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            device = this.GraphicsDevice;

            EventInput.EventInput.Initialize(this.Window);

            cam = new Camera(new Vector3(-8, 8, -8), -MathHelper.PiOver4*3, -0.2f, 0.001f, 0.1f, device);

            Arrow.InitEffect(device);

            // Setup axes grid 
            grid = new Grid(device, 0.1f);

            // Field
            field = new EField(device);

            // Setup console
            console = new EMConsole(device, Content, this);

            EventInput.EventInput.KeyUp += KeyUpEvent;

            screenshotRender = new RenderTarget2D(device, windowedWidth, windowedHeight, false, SurfaceFormat.Color, DepthFormat.Depth24);
            console.Log("Welcome to EM Simulator!  To get started, move your position in space with WASD, and rotate camera with mouse.  Run help() for a list of available commands.");
        }

        public EField GetField()
        {
            return field;
        }

        bool hasInput = true;
        protected override void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.Escape))
                this.Exit();

            if (hasInput)
            {
                cam.UpdateCam(1.0f);
            }

            base.Update(gameTime);
        }

        #region Key Handling for Fullscreen and Screenshots
        void KeyUpEvent(object sender, KeyEventArgs e)
        {
            if (hasInput)
            {
                if (e.KeyCode == Keys.F11)
                {
                    if (graphics.IsFullScreen)
                    {
                        graphics.IsFullScreen = false;
                        graphics.PreferredBackBufferWidth = windowedWidth;
                        graphics.PreferredBackBufferHeight = windowedHeight;
                        screenshotRender = new RenderTarget2D(device, windowedWidth, windowedHeight, false, SurfaceFormat.Color, DepthFormat.Depth24);
                    }
                    else
                    {
                        graphics.IsFullScreen = true;
                        graphics.PreferredBackBufferWidth = fullscreenWidth;
                        graphics.PreferredBackBufferHeight = fullscreenHeight;
                        screenshotRender = new RenderTarget2D(device, fullscreenWidth, fullscreenHeight, false, SurfaceFormat.Color, DepthFormat.Depth24);
                    }
                    graphics.ApplyChanges();
                }
            }

            if (e.KeyCode == Keys.PrintScreen || e.KeyCode == Keys.F12)
            {
                device.SetRenderTarget(screenshotRender);
                MainDraw();
                device.SetRenderTarget(null);

                string baseDir = "C:\\Users\\" + Environment.UserName + "\\Desktop\\EMShots";
                if(!Directory.Exists(baseDir))
                {
                    Console.WriteLine("Making dir");
                    Directory.CreateDirectory(baseDir);
                }

                int x = 0;
                string path = baseDir + "\\" + x + ".png"; 
                while(File.Exists(path))
                {
                    x++;
                    path = baseDir + "\\" + x + ".png";
                }
                FileStream fs = new FileStream(path, FileMode.CreateNew);
                screenshotRender.SaveAsPng(fs, screenshotRender.Width, screenshotRender.Height);
            }
        }
        #endregion


        public void AddCharge(Vector3 pos, float chargeInMicroCouls)
        {
            field.AddCharge(pos, chargeInMicroCouls);
        }
        public void SetInputEnabled(bool input)
        {
            hasInput = input;
        }

        protected override void Draw(GameTime gameTime)
        {
            MainDraw();

            base.Draw(gameTime);
        }

        void MainDraw()
        {
            GraphicsDevice.Clear(ClearOptions.DepthBuffer | ClearOptions.Target, Color.Black, 1.0f, 0);

            // Draw the E field
            field.Draw(cam.GetViewMatrix(), cam.GetProjMatrix());

            // Draw the axes
            grid.Draw(cam.GetViewMatrix(), cam.GetProjMatrix());

            // Draw the console
            console.Draw();
        }
    }
}
