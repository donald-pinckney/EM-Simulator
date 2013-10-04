using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace EM_Sim
{
    class Commands
    {
        const string helpText = "Commands: help add quit ls select delete toggle clear";
        const string addHelpText = "add a point charge: add charge [positionVec] [chargeInMicroCoul]";
        const string quitHelpText = "quit the program";
        const string lsHelpText = "list the charges";
        const string selectHelpText = "select a charge ID: select [ID]";
        const string toggleHelpText = "toggle [vectors|lines]";
        const string clearHelpText = "clears the console of text";
        const string evalHelpText = "evaluates the value of the E or B field at a given position vecotor. \neval [E|B] [positionVec]";

        static int selectedID = -1;

        // Massive method to parse all possible commands
        public static string EvaluateCommand(string command, string[] args, EMSim sim, EMConsole console)
        {
            if (command == "help")
            {
                if (args.Length == 0)
                {
                    return helpText;
                }
                else
                {
                    if (args[0] == "add")
                    {
                        return addHelpText;
                    }
                    else if (args[0] == "quit")
                    {
                        return quitHelpText;
                    }
                    else if (args[0] == "clear")
                    {
                        return clearHelpText;
                    }
                    else
                    {
                        return helpText;
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
                    return addHelpText;
                }

                if (args[0] == "charge")
                {
                    if (args.Length != 3) return addHelpText;

                    string vectorString = args[1];
                    string chargeString = args[2];

                    Vector3 pos = ParseVector3(vectorString);
                    float charge = float.Parse(chargeString);
                    sim.AddCharge(pos, charge);
                    return "";
                }
                else if (args[0] == "circle")
                {
                    
                    float radius = 20;
                    
                    int numLat = 20;
                    for (int i = 0; i < numLat; i++)
                    {
                        double lat = -MathHelper.PiOver2 + Math.PI * ((double)i / (double)(numLat - 1));

                        double circumference = 2 * Math.PI * radius * (float)Math.Cos(lat);
                        int numLon = (int)(40 * (circumference / (40 * Math.PI)));
                        for (int n = 0; n < numLon; n++)
                        {
                            double lon = MathHelper.TwoPi * ((double)n / (double)(numLon));
                            
                            float x = radius * (float)Math.Cos(lat) * (float)Math.Cos(lon);
                            float z = radius * (float)Math.Cos(lat) * (float)Math.Sin(lon);
                            float y = radius * (float)Math.Sin(lat);

                            Vector3 pos = new Vector3(x, y, z);
                            float charge = 0.1f;
                            sim.AddCharge(pos, charge);
                        }
                    }
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
                    return selectHelpText;
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
                if (args.Length != 1) return toggleHelpText;

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
                if (args.Length != 1) return "Wrong number of arguments for gen";

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
                    return evalHelpText;
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
