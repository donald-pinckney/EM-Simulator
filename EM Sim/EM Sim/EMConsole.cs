using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using EventInput;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using Microsoft.CSharp;
using System.Collections.Generic;

namespace EM_Sim
{
    class EMConsole
    {
        GraphicsDevice device;
        SpriteFont font;
        SpriteBatch spriteBatch;
        EMSim sim;
        RectangleOverlay overlay;

        ScriptEngine pythonEngine;
        dynamic pythonScope;

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


            // Setup python interepreter
            Commands.console = this;
            Commands.sim = this.sim;
            pythonEngine = Python.CreateEngine();
            var paths = pythonEngine.GetSearchPaths();
            paths.Add(@"C:\Program Files (x86)\IronPython 2.7\Lib");
            pythonEngine.SetSearchPaths(paths);
            pythonScope = pythonEngine.CreateScope();
            pythonEngine.Execute(@"from __future__ import print_function
def print(*args, **kwargs):
    logf(args, kwargs)

", pythonScope);
            

            pythonScope.logf = new Action<dynamic, dynamic>(Logf);
            pythonScope.log = new Action<dynamic>(Log);
            pythonScope.help = new Action<string>(Commands.Help);
            pythonScope.quit = new Action(Commands.Quit);
            pythonScope.clear = new Action(Commands.Clear);
            pythonScope.addCharge = new Action<float, float, float, float>(Commands.AddCharge);
            pythonScope.addSphere = new Action<float, float, float, float, float>(Commands.AddSphere);
            pythonScope.ls = new Action(Commands.Ls);
            pythonScope.select = new Action<int>(Commands.Select);
            pythonScope.deleteID = new Action<int>(Commands.Delete);
            pythonScope.delete = new Action(Commands.Delete);
            pythonScope.toggleVectors = new Action(Commands.ToggleVectors);
            pythonScope.toggleLines = new Action(Commands.ToggleLines);
            pythonScope.genVectors = new Action(Commands.GenVectors);
            pythonScope.genLines = new Action(Commands.GenLines);
            pythonScope.autogen = new Action<bool>(Commands.Autogen);
            pythonScope.evalE = new Action<float, float, float>(Commands.EvalE);
            pythonScope.lsScripts = new Action(Commands.LsScripts);
            pythonScope.run = new Action<string>(Commands.Run);
        }

        public void Logf(dynamic arg0, dynamic arg1)
        {
            Log(arg0[0].ToString());
        }
        public void Log(dynamic line)
        {
            lines.Add(line.ToString());
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
                //Console.WriteLine((int)e.Character);
                if (e.Character >= 32 && e.Character <= 126) // If the key is a printable ascii character, add it to the current input line
                {
                    inputText += e.Character;
                }
                else if (e.Character == 13) // If the enter key was pressed, evaluate current line, and reset the input
                {
                    EvaluateInput(inputText);   
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
        private void EvaluateInput(string input)
        {
            try
            {
                pythonEngine.Execute(input, pythonScope);
            }
            catch(Exception e)
            {
                Log("Error evaluating input: " + e.Message);
            }
        }

        public void LogAvailableScripts()
        {
            Log("Available scripts:");
            string[] scripts = System.IO.Directory.GetFiles("Content", "*.ems");
            string scriptNames = "";
            foreach (string script in scripts)
            {
                string scriptName = script.Replace("Content\\", "");
                scriptName = scriptName.Replace(".ems", "");
                scriptNames += scriptName + " ";
            }
            Log(scriptNames);
        }
        public void RunScript(string scriptName)
        {
            try
            {
                string[] script = System.IO.File.ReadAllLines("Content\\" + scriptName + ".ems");
                ExecuteScriptLines(script);

            }
            catch
            {
                Log("No script named \"" + scriptName + "\" found.");
            }
        }

        private string Evaluate(string expression)
        {
            try
            {
                DataTable table = new DataTable();
                table.Reset();
                table.Columns.Add("expression", typeof(string), expression);
                DataRow row = table.NewRow();
                table.Rows.Add(row);

                string result = (string)row["expression"];

                return result;
            }
            catch
            {
                return expression;
            }
        }

        private string EvaluateVariableExpression(string expr, Dictionary<string, string> vars)
        {
            foreach (string varName in vars.Keys)
            {
                string val = vars[varName];
                string literalVar = "$" + varName;
                expr = expr.Replace(literalVar, val);
            }
            Console.WriteLine("Expression: " + expr);
            if (expr.Contains("+") || expr.Contains("-") || expr.Contains("*") || expr.Contains("/") || expr.Contains("=") || expr.Contains("<") || expr.Contains(">"))
            {
                /*Expression e = new Expression(expr);
                try
                {
                    return e.Evaluate().ToString();
                }
                catch
                {
                    Console.WriteLine("Error with expression... returning original!");
                    return expr;
                }*/
                return Evaluate(expr);
            }
            else
            {
                return expr;
            }
        }
        private void ExecuteScriptLines(string[] lines)
        {
            Dictionary<string, string> variables = new Dictionary<string, string>();
            Dictionary<string, int> labels = new Dictionary<string,int>();

            for (int i = 0; i < lines.Length; i++)
            {
                string input = lines[i];
                if (input.Length == 0) // Allows whitespace in script
                {
                    continue;
                }

                string[] words = input.Split(' ');
                string command = words[0];
                if (command.Contains("#")) // Allows for single-line comments in script
                {
                    continue;
                }
                string[] args = new string[words.Length-1];
                for(int j = 1; j < words.Length; j++)
                {
                    args[j-1] = words[j];
                }



                // Process input for goto labels
                if (command == "label")
                {
                    if (args.Length < 1)
                    {
                        Log("Invalid label command");
                        continue;
                    }

                    string labelName = EvaluateVariableExpression(args[0], variables);
                    labels[labelName] = i;

                    command = args[1];
                    input = command;
                    string[] newArgs = new string[args.Length - 2];
                    for (int j = 2; j < args.Length; j++)
                    {
                        newArgs[j - 2] = args[j];
                        input += " " + args[j];
                    }
                    args = newArgs;
                }

                if (command == "if")
                {
                    if (args.Length < 2)
                    {
                        Log("Invalid if command");
                        continue;
                    }

                    string condition = args[0];
                    string result = EvaluateVariableExpression(condition, variables);
                    if (result == "False")
                    {
                        continue;
                    }
                    else if (result != "True")
                    {
                        Log("Invalid if condition");
                    }

                    // Set the stuff after the condition to the new command to evaluate
                    command = args[1];
                    input = command;
                    string[] newArgs = new string[args.Length - 2];
                    for (int j = 2; j < args.Length; j++)
                    {
                        newArgs[j - 2] = args[j];
                        input += " " + args[j];
                    }
                    args = newArgs;
                }

                if (command == "set")
                {
                    if (args.Length != 2)
                    {
                        Log("Invalid set command");
                        continue;
                    }

                    string varName = args[0];
                    string varVal = args[1];
                    variables[varName] = EvaluateVariableExpression(varVal, variables);
                }
                else if (command == "echo")
                {
                    if (args.Length != 1)
                    {
                        Log("Invalid echo command");
                        continue;
                    }

                    string result = EvaluateVariableExpression(args[0], variables);
                    Console.WriteLine("Result: " + result);
                    Log(result);
                }
                else if (command == "goto")
                {
                    if (args.Length != 1)
                    {
                        Log("Invalid goto command");
                        continue;
                    }

                    int lineNumber = 0;
                    bool isLineNumber = int.TryParse(args[0], out lineNumber);
                    
                    if (isLineNumber)
                    {
                        lineNumber--;
                    }
                    else
                    {
                        lineNumber = labels[args[0]];
                    }

                    i = lineNumber - 1;
                    continue;
                }
                else
                {
                    if (command.Length == 0 || command.Contains("#"))
                    {
                        continue;
                    }
                    EvaluateInput(EvaluateVariableExpression(input, variables));
                }
            }
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
