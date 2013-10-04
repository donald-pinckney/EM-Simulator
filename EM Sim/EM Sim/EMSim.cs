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

            cam = new Camera(new Vector3(-5, 5, 5), -MathHelper.PiOver4, -MathHelper.PiOver4, 0.001f, 0.1f, device);

            Arrow.InitEffect(device);

            // Setup axes grid 
            grid = new Grid(device, 0.1f);

            // Field
            field = new EField(device);
            /*field.AddCharge(new Vector3(10, 0, 0), 0.1f);
            field.AddCharge(new Vector3(-10, 0, 0), -0.1f);
            field.generateArrows();
            field.generateEFieldLines();*/

            // Setup console
            console = new EMConsole(device, Content, this);

            EventInput.EventInput.KeyUp += KeyUpEvent;

            screenshotRender = new RenderTarget2D(device, windowedWidth, windowedHeight, false, SurfaceFormat.Color, DepthFormat.Depth24);
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

            if (e.KeyCode == Keys.PrintScreen)
            {
                device.SetRenderTarget(screenshotRender);
                MainDraw();
                device.SetRenderTarget(null);

                if(!Directory.Exists("C:\\Users\\Donald\\Desktop\\EMShots"))
                {
                    Console.WriteLine("Making dir");
                    Directory.CreateDirectory("C:\\Users\\Donald\\Desktop\\EMShots");
                }

                int x = 0;
                string path = "C:\\Users\\Donald\\Desktop\\EMShots\\" + x + ".png"; 
                while(File.Exists(path))
                {
                    x++;
                    path = "C:\\Users\\Donald\\Desktop\\EMShots\\" + x + ".png";
                }
                FileStream fs = new FileStream(path, FileMode.CreateNew);
                screenshotRender.SaveAsPng(fs, screenshotRender.Width, screenshotRender.Height);
            }
        }


        public void AddCharge(Vector3 pos, float chargeInMicroCouls)
        {
            field.AddCharge(pos, chargeInMicroCouls);
            //field.generateArrows();
            //field.generateEFieldLines();
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
