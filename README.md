# The-Great-Expanse
A space exploration game developed by J Hamilton.

Controlling the game
--------------------
To perform a maneuver, click on a position in the craft's orbit to create a maneuver node. Maneuver nodes can have the maneuver modified by dragging the thrust vector handle. Accelerate time with '.' and decelerate time with ','. Game can be paused with space bar. Input is controlled by: Assets/Scripts/Ship/InputController.cs. 

Setting up the Game
-------------------
The world consists of many objects, each of which is explained below. 

**Rules for all game objects**
>1. Most components come with a debug mode 
checkbox, toggle it to see detailed visual information 
about the component.
>2. Spheres of influence are not meant to intersect, bad
things could happen if they do.
>3. All public attribute fields should be given a value.
Some components might throw exceptions if a public 
attribute field is left blank.

**The Black Hole Game Object**
- Components: Sprite Renderer, Black Hole Elements, 
Massive Body Elements, Black Hole Behavior, Gravity 
Elements, Black Hole Massive Body Behavior, Sprite 
Renderer.
- There can only be one
- Best practice is to place it at the origin
- Must be given the tag "MassiveBody"

**The Star Game object**
- Components: Star Behavior, Massive Body Elements,
Star Elements, Gravity Elements, Star Gravity Behavior,
Star Massive Body Behavior, Sprite Renderer.
- Must be given the tag "MassiveBody"
- Can create as many stars as your computer can handle.
- Patched conics are drawn in yellow.

**The Planet Game Object**
- Components: Planet Behavior, Massive Body Elements,
Planet Elements, Gravity Elements, Planet Gravity Behavior,
Planet Massive Body Behavior, Sprite Renderer.
- Must be given the tag "MassiveBody"
- Patched conics are drawn in blue.

**The Ship Game Object**
- Components: Input Controller, Sprite Renderer, Gravity
Elements, Ship Gravity Behavior.
- No tag
- There can only be one
- Patched conics are drawn in red.
- Maneuver patched conics are drawn in cyan.

**Global Scene Manager**
- Components: PreUpdate
- PreUpdate clears the lines drawn last frame so they can be redrawn this frame.
