README for Graphics and Interaction Project 2
Mitchell Brunton

Extreme Sailing

TODO: 
- access ocean heights (ocean is being rippled by gpu, and currently I can't access its height to create buoyant forces on the player)
- display wind direction/speed on screen
- add fish (which jump in and out of water, following random paths). Player can eat fish to increase health
- add collisions between coastguard/player

Objective: sail around and avoid getting shot by the coast guard
- sailing is realistic (speed is determined by angle between sail and wind, wind strength, and whether wind is in front or behind)
- if traveling into wind, avoid heading directly into it
- optimal upwind sail angle is 35 degrees
- if traveling downwind, attempt to sail with wind directly behind, with sail slack fully released
- avoid getting shot by the coastguard (they don't like casual sailors)
- boat can go on land (this is EXTREME sailing)
- wind direction and speed updates randomly every 20 seconds

How I modeled entities
- terrain and ocean defined by vertex mesh created at runtime
- all moving objects (player, coast guard) were created in blender
- I used 3 separate custom shaders:
 - one for terrain
 - a geometric shader for ocean (creates ripple effect)
 - one for model objects created in blender

Graphics and camera motion
- world has two point light sources: a sun and moon
- the camera follows behind the player, and the faster the player travels the further behind the camera stays (to get a wider field of view)

- the only code I have used from the internet is a modified phong.fx, supplied in labs