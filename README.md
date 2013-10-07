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

Open the console by pressing *tab*.

*Syntax Conventions:* When a command asks for a vector argument, always specify it in the format: `x,y,z`

*Math Conventions:* EM Simulator uses a left-handed, Y up coordinate system.

####Examples
To get familiar with the console in EM Simulator, run these examples sequentially.  Use the mouse and keyboard controlls
to fully interact with EM Simulator, to get the best view of the charges and electric field in the world.

**Electric dipole setup**
```
add charge -10,0,0 -0.2
add charge 10,0,0 0.2
```

**Now view field lines instead of vectors**
```
toggle lines
toggle vectors
```
![Dipole Screenshot](https://raw.github.com/donald-pinckney/EM-Simulator/screenshots/screenshots/1.png "Dipole Screenshot")

**Add a third charge**
```
add charge -5,20,25 0.4
```

**Now view the vector field rather than the field lines**
```
toggle vectors
toggle lines
```

![Vector Field Screenshot](https://raw.github.com/donald-pinckney/EM-Simulator/screenshots/screenshots/0.png "Vector Field Screenshot")

##Console Command Documentation

+ `help {command}`

  Provide a list of available commands, or help for a specific command.
  <br />
  <br />
  
+ `quit`

  Immediately exist EM Simulator.
  <br />
  <br />
  
+ `clear`

  Clear the console of all text.
  <br />
  <br />

+ `add [positionVec] [chargeInMicroCoul]`

  Add a new point charge to the world at the specified position, with the specified charge in micro Coulombs.
  <br />
  *Examples*:
  <br />
  `add 6,0,10 0.2`
  <br />
  `add 12,4,-3 -0.3`
  <br />
  <br />
  
+ `ls`

  List all the point charges in the world, including their unique ID, position, and charge.<br />
  All point charges are identifed by a unique ID number: you use this ID number when deleting charges.<br />
  `ls` is useful to find the ID of the charge you want to delete.
  <br />
  <br />
  
+ `select [ID]`

  Sets the currently selected charge to the one with the specified ID. 
  The selected charge can be later modified or deleted.
  <br />
  <br />

+ `delete {ID}`

  Delete a charge specified by ID, or pass no ID argument to delete the currently selected charge (see above).
  <br />
  <br />

+ `toggle [vectors|lines]`

  Toggle on or off rendering of vectors or field lines. <br />
  Vectors default to *on*, and field lines default to *off*.
  <br />
  <br />
  
+ `eval [E|B] [positionVec]`

  Evaluate the vector value of either the electric or magnetic (not implemented yet) field at a specified point.
  <br />
  *Example*:
  <br />
  `eval E 5,2,3`
  <br />
  <br />

+ `gen [vectors|lines]`

  Force the recalculation of the vector field or the field lines.
  <br />
  <br />
  
