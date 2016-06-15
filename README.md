# The-Great-Expanse
A space exploration game developed by J Hamilton.

Controlling the game
--------------------
The game controls are left joystick to control the 
direction of thrust, pess A to charge up the thrust,
release A to thrust, B to fast forward time, and X 
slow down time. The games controls are bound to an 
xbox360 controller, the key bindings can be changed 
in Edit->project settings->input. The interpretation 
of the inputs is done in the file 
Assets/Scripts/Ship/InputController.cs. 

Setting up the Game
-------------------
The world consists of 5 different objects: A single 
black hole, stars orbitting the black hole, planets 
orbiting the stars, a ship for the player to 
control, and a GlobalSceneManager to store global 
variables. 

*Rules for all game objects*
>1. Most components come with a debug mode 
checkbox, toggle it to see detailed visual information 
about the component.
>2. Spheres of influence are not meant to intersect, bad
things could happen if they do.
>3. All public attribute fields should be given a value.
Some components might throw exceptions if a public 
attribute field is left blank.

*The Black Hole Game Object*
>Components: Sprite Renderer, Black Hole Elements, 
Massive Body Elements, Black Hole Behavior, Gravity 
Elements, Black Hole Massive Body Behavior, Sprite 
Renderer.
>There can only be one
>Best practice is to place it at the origin
>Must be given the tag "MassiveBody"

*The Star Game object*
>Components: Star Behavior, Massive Body Elements,
Star Elements, Gravity Elements, Star Gravity Behavior,
Star Massive Body Behavior, Sprite Renderer.
>Must be given the tag "MassiveBody"
>Can create as many stars as your computer can handle.

*The Planet Game Object*
>Components: Planet Behavior, Massive Body Elements,
Planet Elements, Gravity Elements, Planet Gravity Behavior,
Planet Massive Body Behavior, Sprite Renderer.
>Must be given the tag "MassiveBody"

*The Ship Game Object*
>Components: Input Controller, Sprite Renderer, Gravity
Elements, Ship Gravity Behavior.
>No tag
>There can only be one

*Global Scene Manager*
>Components: Global Elements.
>It's single component is a little special, the attributes
contained in GlobalElements.cs are static, meaning they 
wont show up in Unity's inspector and must be modified
from visual studio. 
