using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace EM_Sim
{
    class Commands
    {
        readonly static Dictionary<string, string> helpTexts = new Dictionary<string, string>
        {
            {"help", "Commands: help quit clear add ls select delete toggle eval gen"},
            {"quit", "Quit the program"},
            {"clear", "Clear the console of text"},
            {"add", "Add a point charge: add charge [positionVec] [chargeInMicroCoul]"},
            {"ls", "List all the charges in the world"},
            {"select", "Select a charge with a given ID for editing or deletion: select [ID]"},
            {"delete", "Delete a specified charge, or pass no ID argument to delete the selected charge: delete {ID}"},
            {"toggle", "Toggle different visualizations on and off: toggle [vectors|lines]"},
            {"eval", "Evaluate the value of the E or B field at a given position vecotor: eval [E|B] [positionVec]"},
            {"gen", "Force the recalculation of either the E field or the E field lines: gen [vectors|lines]"}
        };

        static int selectedID = -1;

        // Massive method to parse all possible commands
        public static string EvaluateCommand(string command, string[] args, EMSim sim, EMConsole console)
        {
            if (command == "help")
            {
                if (args.Length == 0)
                {
                    return helpTexts["help"];
                }
                else
                {
                    string helpArg = args[0];
                    if (helpTexts.ContainsKey(helpArg) == true)
                    {
                        return helpTexts[helpArg];
                    }
                    else
                    {
                        return helpTexts["help"];
                    }
        
                }
            }
            else if (command == "quit")
            {
                Environment.Exit(0);
                return "";
            }
            else if (command == "clear")
            {
                console.ClearLog();
                return "";
            }
            else if (command == "add")
            {
                if (args.Length < 1)
                {
                    return helpTexts["add"];
                }

                if (args[0] == "charge")
                {
                    if (args.Length != 3) return helpTexts["add"];

                    string vectorString = args[1];
                    string chargeString = args[2];

                    Vector3 pos = ParseVector3(vectorString);
                    float charge = float.Parse(chargeString);
                    sim.AddCharge(pos, charge);
                    sim.GetField().generateArrows();
                    sim.GetField().generateEFieldLines();
                    return "";
                }
                else if (args[0] == "sphere")
                {
                    float radius = 20;
                    
                    int numLat = 20;
                    for (int i = 0; i < numLat; i++)
                    {
                        double lat = -MathHelper.PiOver2 + Math.PI * ((double)i / (double)(numLat - 1));
                        float y = radius * (float)Math.Sin(lat);

                        double circumference = 2 * Math.PI * radius * (float)Math.Cos(lat);
                        int numLon = (int)(40 * (circumference / (40 * Math.PI)));
                        for (int n = 0; n < numLon; n++)
                        {
                            double lon = MathHelper.TwoPi * ((double)n / (double)(numLon));
                            
                            float x = radius * (float)Math.Cos(lat) * (float)Math.Cos(lon);
                            float z = radius * (float)Math.Cos(lat) * (float)Math.Sin(lon);

                            Vector3 pos = new Vector3(x, y, z);
                            float charge = 0.1f;
                            sim.AddCharge(pos, charge);
                        }
                    }
                    sim.GetField().generateArrows();
                    sim.GetField().generateEFieldLines();
                    return "";
                }
                else
                {
                    return "Unknown object to add";
                }
            }
            else if (command == "ls")
            {
                List<PointCharge> charges = sim.GetField().GetCharges();
                for (int i = 0; i < charges.Count; i++)
                {
                    console.Log(charges[i].ToString());
                }
                return "";
            }
            else if (command == "select")
            {
                if (args.Length != 1)
                {
                    return helpTexts["select"];
                }
                else
                {
                    selectedID = int.Parse(args[0]);
                    return "Selected ID " + selectedID;
                }
            }
            else if (command == "delete")
            {
                if (args.Length == 1)
                {
                    int id = int.Parse(args[0]);
                    sim.GetField().DeleteChargeWithID(id);
                    return "Deleted ID " + id;
                }
                sim.GetField().DeleteChargeWithID(selectedID);
                string ret = "Deleted ID " + selectedID;
                selectedID = -1;
                return ret;
            }
            else if (command == "toggle")
            {
                if (args.Length != 1) return helpTexts["toggle"];

                if (args[0] == "vectors")
                {
                    if (sim.GetField().shouldDrawVectors)
                    {
                        sim.GetField().shouldDrawVectors = false;
                        return "Disabled vectors";
                    }
                    else
                    {
                        sim.GetField().shouldDrawVectors = true;
                        return "Enabled vectors";
                    }
                }
                else if (args[0] == "lines")
                {
                    if (sim.GetField().shouldDrawLines)
                    {
                        sim.GetField().shouldDrawLines = false;
                        return "Disabled lines";
                    }
                    else
                    {
                        sim.GetField().shouldDrawLines = true;
                        return "Enabled lines";
                    }
                }
                else
                {
                    return "Unknown toggle argument";
                }
            }
            else if (command == "gen")
            {
                if (args.Length != 1) return helpTexts["gen"];

                if (args[0] == "vectors")
                {
                    sim.GetField().generateArrows();
                    return "";
                }
                else if (args[0] == "lines")
                {
                    sim.GetField().generateEFieldLines();
                    return "";
                }
                else
                {
                    return "Unknown argument for gen";
                }
            }
            else if (command == "eval")
            {
                if (args.Length < 2)
                {
                    return helpTexts["eval"];
                }
                if (args[0] == "E")
                {
                    Vector3 pos = ParseVector3(args[1]);
                    Vector3 val = sim.GetField().r(pos.X, pos.Y, pos.Z);
                    return val.ToString();
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "Unknown command";
            }
        }


        private static Vector3 ParseVector3(string vectorString)
        {
            string[] components = vectorString.Split(',');
            if (components.Length != 3) return Vector3.Zero;

            return new Vector3(float.Parse(components[0]), float.Parse(components[1]), float.Parse(components[2])); 
        }
    }
}
