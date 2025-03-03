# Bottles and Glasses Package

This is a short documentation about this package. The package contains various bottles that are similar to bottles in German supermarkets. Additionally, glasses are added that can be used. Finally, a flow mechanism has been built to simulate the filling of glasses and the reducing of the "beverage" in the bottles.

## Dependencies

The bottles and glasses have been built with Blender. The models, designs and materials are in this package.

The only dependency is for the filling system. The asset "Legacy Particle Pack" has been used for this. It has been downloaded from the Unity Asset Store to simulate the flow of liquid. Link: https://assetstore.unity.com/packages/vfx/particles/legacy-particle-pack-73777

The "WaterDrip" from the asset is used (and modified).

## Package Overview

This package has three categories: Bottles, Glasses and WaterFlow.

Bottles: Contains all the models, designs and materials for different types of bottles for different types of beverages (named accordingly to the use of the beverage).
- This folder also contains the LiquidShaders. They contain the logic behind the behaviors of the beverages.

Glasses: Contains all the models, designs and materials for different types of glasses for different types of beverages (named accordingly to the use of the beverage).

WaterFlow: Contains the script to coordinate the pouring of a liquid from a bottle to a glass and the following "filling" and "reducing" of the glass or bottle, respectively.

## Logic

The bottles and glasses contain a scaled down version of their meshes inside of them. The scaled down mesh contains a shader as a material. This shader can change the "fullness" of the scaled down mesh. This way, the filling and reducing is simulated. The value ("Fill") is accessed through the WaterFlow script that is attached to each bottle.

The script contains six GameObjects: 
- Bottle: The bottle, whose content is supposed to be reduced.
- Glass: The glass, whose content is supposed to be increased.
- LiquidFlow: The flow that is supposed to come out of the bottle into a glass.
- TopOfBottle: The point from which the flow is supposed to happen.
- LiquidInBottle: The scaled down mesh of the bottle that simulates the liquid.
- LiquidInGlass: The scaled down mesh of the glass that simulated the liquid.

The script works correctly, if you add the following GameObjects to the script. The script itself should be added to the parent-element of the bottle you want to simulate.

- Bottle: Parent Object (that the script is attached to).
- Glass: Parent Object of the glass.
- LiquidFlow: Child Object inside the Bottle Object.
- TopOfBottle: (Empty) Child Object inside the Bottle Object.
- LiquidInBottle: Child Object inside the Bottle Object (named after the specific beverage of the bottle).
- LiquidInGlass: Child Object inside the Glass Object (named after the specific beverage of the glass).
