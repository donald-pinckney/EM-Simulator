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
            {"help", "Commands: help() helpc(command) quit() clear() addCharge(x,y,z,\u00b5C) addSphere(x,y,z,radius,\u00b5C) modifyCharge(property, value) modifyChargeID(id, propery, value) ls() select(id) delete() deleteID(id) toggleVectors() toggleLines() evalE(x,y,z) genVectors() genLines() autogen(isOn) lsScripts() run(script)"},
            {"helpc", "helpc(command): Display help for a specific command. Ex: helpc(\"addCharge\")"},
            {"quit", "quit(): Quit the program."},
            {"clear", "clear(): Clear the console of text."},
            {"addCharge", "addCharge(x,y,z,\u00b5C): Add a point charge. Ex: addCharge(3,0,0,0.2)"},
            {"addSphere", "addSphere(x,y,z,radius,\u00b5C): Add a charged sphere. Ex: addSphere(0,2,0,10,0.1)"},
            {"modifyCharge", "modifyCharge(property, value): Modifies a property (\"x\", \"y\", \"z\", \"charge\")  for the currently selected charge (see help(\"select\")). Ex: modifyCharge(\"y\", 5)"},
            {"modifyChargeID", "modifyCharge(property, value): Modifies a property (\"x\", \"y\", \"z\", \"charge\")  for a charge with a given ID. Ex: modifyChargeID(2, \"y\", 5)"},
            {"ls", "ls(): List all the charges in the world."},
            {"select", "select(id): Select a charge with a given ID for editing or deletion. Ex: select(2)"},
            {"delete", "delete(): Delete the currently selected charge (see help(\"select\"))."},
            {"deleteID", "deleteID(id): Delete a charge with a given ID. Ex: deleteID(2)"},
            {"toggleVectors", "toggleVectors(): Toggle visualization of the E (vector) field on and off."},
            {"toggleLines", "toggleLines(): Toggle visualization of the E field lines on and off."},
            {"evalE", "evalE(x,y,z): Evaluate the value of the E field at a given position vecotor."},
            {"genVectors", "genVectors(): Force the recalculation of the E (vector) field."},
            {"genLines", "genLines(): Force the recalculation of the E field lines."},
            {"autogen", "autogen(isOn): Turn on or off automatic generation of E field and E field lines when adding charges. Defaults to True."},
            {"lsScripts", "lsScripts(): List the python scripts available in the Content directory"},
            {"run", "run(script): Run a script with a given name in the Content. Specify the script without a file extension. Ex: run(\"capacitor\")"}
        };

        static int selectedID = -1;
        static bool shouldAutogen = true;


        public static EMConsole console;
        public static EMSim sim;

        public static void Help()
        {
            console.Log(helpTexts["help"]);
        }

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

        public static void ModifyCharge(string property, float value)
        {
            ModifyChargeID(selectedID, property, value);
        }

        public static void ModifyChargeID(int id, string property, float value)
        {
            PointCharge oldC = sim.GetField().GetChargeWithID(id);

            if (oldC == null)
            {
                console.Log("Charge with ID " + id + " not defined");
                return;
            }

            float x = oldC.center.X;
            float y = oldC.center.Y;
            float z = oldC.center.Z;
            float charge = oldC.charge * 1000000;

            if (property == "x")
            {
                x = value;
            }
            else if (property == "y")
            {
                y = value;
            }
            else if (property == "z")
            {
                z = value;
            }
            else if (property == "charge")
            {
                charge = value;
            }
            else
            {
                console.Log("Invalid property \"" + value + "\"");
                return;
            }

            PointCharge c = new PointCharge(sim.GraphicsDevice, new Vector3(x, y, z), charge, id);
            sim.GetField().DeleteChargeWithID(id);
            sim.GetField().AddCharge(c);

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
