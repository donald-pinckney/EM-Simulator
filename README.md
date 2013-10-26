#EM Simulator

EM Simulator is a 3D simulation and visualization software written in C# and XNA for [electric](http://en.wikipedia.org/wiki/Electric_field) (and soon, magnetic) fields.
The simulator allows you to place point charges in a 3D world and view the resulting vector field and field lines.

##System Requirements

EM Simulator uses [XNA](http://en.wikipedia.org/wiki/Microsoft_XNA), and thus has specific system requirements:

1. Windows XP or newer
2. DirectX 9 or newer compatible graphics card (Intel integrated graphics *may* suffice - not tested)

In the future I plan to either rewrite EM Simulator in OpenGL or utilize [Mono](http://www.mono-project.com/Main_Page)
to achieve greater platform compatibility.

##Quick Setup
Express setup:

1. Download the latest build under [releases.](https://github.com/donald-pinckney/EM-Simulator/releases)
2. Unzip, and run *setup.exe*.  
3. The installer will automatically install the .NET Framework 4 and XNA, if necessary.
3. The installer will then automatically start EM Simulator.
4. Read below for a brief introduction of how to use.
5. Enjoy!

##Getting started
###Controls
<dl>
  <dt>W, A, S, D, Mouse</dt>
  <dd>Move and rotate the camera in space</dd>
  <dt>Esc</dt>
  <dd>Exit EM Simulator</dd>
  <dt>Tab</dt>
  <dd>Open EM Simulator console (see below)</dd>
  <dt>F11</dt>
  <dd>Toggle Fullscreen</dd>
  <dt>F12</dt>
  <dd>Take Screenshot</dd>
  <dt>Ctrl</dt>
  <dd>Free mouse</dd>
</dl>

###Console
The EM Simulator console is the primary way to interact with the world: you use it to add charges, toggle vectors and
field lines on or off, delete charges, etc.

The console uses **Python**, specificaly [IronPython](http://ironpython.net/) as an interpreter.

*Math Conventions:* EM Simulator uses a left-handed, Y up coordinate system.

Open the console by pressing **tab**.

####Examples
To get familiar with the console in EM Simulator, run these examples sequentially.  Use the mouse and keyboard controlls
to fully interact with EM Simulator, to get the best view of the charges and electric field in the world.

**Electric dipole setup**
```
addCharge(-10,0,0,-0.2)
addCharge(10,0,0,0.2)
```

**Now view field lines instead of vectors**
```
toggleLines()
toggleVectors()
```
![Dipole Screenshot](https://raw.github.com/donald-pinckney/EM-Simulator/screenshots/screenshots/1.png "Dipole Screenshot")

**Add a third charge**
```
addCharge(-5,20,25,0.4)
```

**Now view the vector field rather than the field lines**
```
toggleVectors()
toggleLines()
```

![Vector Field Screenshot](https://raw.github.com/donald-pinckney/EM-Simulator/screenshots/screenshots/0.png "Vector Field Screenshot")

##Console Command Documentation
Please refer to the [wiki page](https://github.com/donald-pinckney/EM-Simulator/wiki/List-of-Console-Commands) for documentaion and a full list of all available console commands.

##Scripting Documentation
The EM Simulator console uses Python as interpreter.
Thus, it can elegantly load and execute Python scripts, automating commands in the console.
To see a quick demo of script execution in action, simply type `run("capacitor")` in the console, and after a few seconds it will generatea  parallel plate capacitor.

To make your own Python scripts, write Python code as you normally would, and then make the same Python function calls as you do in the console.
For example, consider the follow Python code, which generates a row of point charges:
```
for x in range(0, 10):
    addCharge(x,0,0,0.05)
```
Then, place your Python files in the Content directory available in %userprofile%\AppData\Local\Apps.
Unfortunately, as of now, you will have to hunt in subfolders, as the installation folder is obsfuscated by XNA.
Future versions of EM Simulator will allow much easier access to the Content folder.
