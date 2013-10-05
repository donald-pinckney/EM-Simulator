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
using EventInput;

namespace EM_Sim
{
    class EMConsole
    {
        GraphicsDevice device;
        SpriteFont font;
        SpriteBatch spriteBatch;
        EMSim sim;
        RectangleOverlay overlay;

        private List<string> lines = new List<string>();
        public EMConsole(GraphicsDevice d, ContentManager content, EMSim sim)
        {
            device = d;
            this.sim = sim;
            font = content.Load<SpriteFont>("ConsoleFont");
            spriteBatch = new SpriteBatch(device);

            EventInput.EventInput.CharEntered += CharEnteredHandler;

            int fullWidth = sim.fullscreenWidth;
            int fullHeight = sim.fullscreenHeight;

            overlay = new RectangleOverlay(new Rectangle(10, device.Viewport.Height - 200, device.Viewport.Width - 20, 200), new Rectangle(10, fullHeight - 200, fullWidth - 20, 200), new Color(0.2f, 0.2f, 0.2f, 0.2f), device);
            overlay.LoadContent();
        }

        public void Log(String line)
        {
            lines.Add(line);
        }
        public void ClearLog()
        {
            lines.Clear();
        }

        private bool tabToggle = false;
        private string inputText = "";


        public void CharEnteredHandler(object sender, CharacterEventArgs e)
        {
            if (e.Character == 9) // If tab was pressed, toggle the console
            {
                tabToggle = !tabToggle;
                sim.SetInputEnabled(!tabToggle);
            }

            if (tabToggle)
            {
                Console.WriteLine((int)e.Character);
                if (e.Character >= 32 && e.Character <= 126) // If the key is a printable ascii character, add it to the current input line
                {
                    inputText += e.Character;
                }
                else if (e.Character == 13) // If the enter key was pressed, evaluate current line, and reset the input
                {
                    string output = EvaluateInput(inputText);
                    Log(output);    
                    inputText = "";
                }
                else if (e.Character == 8) // If backspace was pressed, delete 1 character from the input
                {
                    if (inputText.Length > 0)
                    {
                        inputText = inputText.Substring(0, inputText.Length - 1);
                    }
                }
            }
        }

        
        

        // This method will evaluate a line of text entered from the console
        private string EvaluateInput(string input)
        {
            string[] words = input.Split(' ');
            if (words.Length < 1) return "";

            string command = words[0];
            string[] args = new string[words.Length-1];
            for(int i = 1; i < words.Length; i++)
            {
                args[i-1] = words[i];
            }

            return Commands.EvaluateCommand(command, args, sim, this);
        }

        const int maxLineWidth = 70;
        public void Draw()
        {
            if (tabToggle)
            {
                spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, DepthStencilState.Default, null);

                overlay.Draw(spriteBatch);

                // Draw previous messages
                List<string> linesToRender = new List<string>(lines);
                for (int i = 0; i < linesToRender.Count; i++)
                {
                    string line = linesToRender[i];
                    if (line.Length > maxLineWidth)
                    {
                        // Wrap by word... find the last space before the linebreak
                        int lineLength = 0;
                        for (int j = maxLineWidth - 1; j >= 0; j--)
                        {
                            if (line[j].Equals(' '))
                            {
                                lineLength = j;
                                break;
                            }
                        }
                        string fitsOnThisLine = line.Substring(0, lineLength);
                        string nextLine = line.Substring(lineLength + 1);
                        linesToRender[i] = fitsOnThisLine;
                        linesToRender.Insert(i + 1, nextLine);
                    }
                }

                Vector2 startPos = new Vector2(10, device.Viewport.Height - 50);
                for (int i = linesToRender.Count - 1; i >= 0; i--)
                {
                    spriteBatch.DrawString(font, linesToRender[i], startPos, Color.White);
                    startPos += new Vector2(0, -20);
                }

                // Draw input line
                Vector2 inputPos = new Vector2(10, device.Viewport.Height - 30);
                spriteBatch.DrawString(font, "> " + inputText, inputPos, Color.White);

                spriteBatch.End();
            }
        }

    }
}
