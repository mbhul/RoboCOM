***************************************
#		ROBOCOM
***************************************
This program was created as a method of sending custom command line inputs to a real-time controller via serial interface (UART). 

I initially created this several years ago when I needed a method to send input to the microcontroller running our parallel cable robot. Consequently, some of the conventions used (ex. command syntax) may seem arbitrary, in that they require the receiving controller to correctly interpret them to be of any use.

---------------------------------------
The following input methods are supported:

1) Text 
- Commands can be typed into the input field

2) Keyboard arrow keys
- Commands are auto-generated based on arrow key inputs

3) Gamepad/Joystick
- Commands are auto-generated based on inputs from a DirectX compatible gamepad or joystick
Note: This project uses the SlimDX open-source library to interface with external controllers

4) Vector graphics
- Commands are auto-generated from a parsed vector graphics (SVG) file
Note: The following standard SVG fields are supported
	- line
	- polyline
	- polygon
	- path: (Only 'M' [move to], 'L' [line to], 'C' [curve to])

SVG parser does not support transformations. Only very simple graphics so far.

---------------------------------------
The generated command line syntax is loosely modelled after CNC G-code.
Existing commands are of the following form:

G9{#}X{a}Y{b}Z{c}A{d}B{e};

where:
# = 0 --> Absolute position input. Tells the receiving controller that the parameters following inline denote an absolute position.
# = 1 --> Relative position input. Tells the receiving controller that the parameters following inline denote a vector from its current position.

Note: both input methods rely on the receiver to validate the commands before consuming them.

X{a} --> Position/distance on the x-axis
Y{b} --> Position/distance on the y-axis
Z{c} --> Position/distance on the z-axis
A{d} --> Angle about the x-axis (in radians)
B{e} --> Angle about the y-axis (in radians)
