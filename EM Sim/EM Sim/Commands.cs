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
            {"help", "Commands: help quit clear add ls select delete toggle eval gen autogen run"},
            {"quit", "Quit the program"},
            {"clear", "Clear the console of text"},
            {"add", "Add a point charge: add charge [positionVec] [chargeInMicroCoul]    OR, Add a sphere of point charges: add sphere [centerPosVec] [radius] [chargeInMicroCoul]"},
            {"ls", "List all the charges in the world"},
            {"select", "Select a charge with a given ID for editing or deletion: select [ID]"},
            {"delete", "Delete a specified charge, or pass no ID argument to delete the selected charge: delete {ID}"},
            {"toggle", "Toggle different visualizations on and off: toggle [vectors|lines]"},
            {"eval", "Evaluate the value of the E or B field at a given position vecotor: eval [E|B] [positionVec]"},
            {"gen", "Force the recalculation of either the E field or the E field lines: gen [vectors|lines]"},
            {"autogen", "Turn on or off automatic generation of E field and E field lines when adding charges. Defaults to on: autogen [on|off]"},
            {"run", "Runs a script with a given name in the Content directory. Specify the script without a file extension: run [script]"}
        };

        static int selectedID = -1;
        static bool shouldAutogen = true;


        public static EMConsole console;
        public static EMSim sim;

        public static void Help(string arg0)
        {
            if (arg0 != null && helpTexts.Keys.Contains(arg0))
            {
                console.Log(helpTexts[arg0]);
            }
            else
            {
                console.Log(helpTexts["help"]);
            }
        }

        public static void Quit()
        {
            Environment.Exit(0);
        }

        public static void Clear()
        {
            console.ClearLog();
        }

        public static void AddCharge(float x, float y, float z, float charge)
        {
            Vector3 pos = new Vector3(x, y, z);
            sim.AddCharge(pos, charge);
            if (shouldAutogen)
            {
                sim.GetField().generateArrows();
                sim.GetField().generateEFieldLines();
            }
        }

        public static void AddSphere(float xC, float yC, float zC, float radius, float charge)
        {
            Vector3 center = new Vector3(xC, yC, zC);

            int numLat = 20;
            for (int i = 0; i < numLat; i++)
            {
                double lat = -MathHelper.PiOver2 + Math.PI * ((double)i / (double)(numLat - 1));
                float y = radius * (float)Math.Sin(lat) + center.Y;

                double circumference = 2 * Math.PI * radius * (float)Math.Cos(lat);
                int numLon = (int)(40 * (circumference / (40 * Math.PI)));
                for (int n = 0; n < numLon; n++)
                {
                    double lon = MathHelper.TwoPi * ((double)n / (double)(numLon));

                    float x = radius * (float)Math.Cos(lat) * (float)Math.Cos(lon) + center.X;
                    float z = radius * (float)Math.Cos(lat) * (float)Math.Sin(lon) + center.Z;

                    Vector3 pos = new Vector3(x, y, z);
                    sim.AddCharge(pos, charge);
                }
            }
        }

        public static void Ls()
        {
            List<PointCharge> charges = sim.GetField().GetCharges();
            for (int i = 0; i < charges.Count; i++)
            {
                console.Log(charges[i].ToString());
            }
        }

        public static void Select(int id)
        {
            selectedID = id;
        }

        public static void Delete(int id)
        {
            sim.GetField().DeleteChargeWithID(id);
            console.Log("Deleted ID " + id);
        }

        public static void Delete()
        {
            if (selectedID != -1)
            {
                sim.GetField().DeleteChargeWithID(selectedID);
                console.Log("Deleted ID " + selectedID);
                selectedID = -1;
            }
        }

        public static void ToggleVectors()
        {
            if (sim.GetField().shouldDrawVectors)
            {
                sim.GetField().shouldDrawVectors = false;
                console.Log("Disabled vectors");
            }
            else
            {
                sim.GetField().shouldDrawVectors = true;
                console.Log("Enabled vectors");
            }
        }

        public static void ToggleLines()
        {
            if (sim.GetField().shouldDrawLines)
            {
                sim.GetField().shouldDrawLines = false;
                console.Log("Disabled lines");
            }
            else
            {
                sim.GetField().shouldDrawLines = true;
                console.Log("Enabled lines");
            }
        }

        public static void GenVectors()
        {
            sim.GetField().generateArrows();
        }
        public static void GenLines()
        {
            sim.GetField().generateEFieldLines();
        }

        public static void Autogen(bool isOn)
        {
            shouldAutogen = isOn;
        }

        public static void EvalE(float x, float y, float z)
        {
            console.Log(sim.GetField().r(x, y, z).ToString());
        }

        public static void LsScripts()
        {
            console.LogAvailableScripts();
        }

        public static void Run(string scriptName)
        {
            console.RunScript(scriptName);
        }
    }
}
