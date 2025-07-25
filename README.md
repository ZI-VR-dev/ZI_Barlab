# ZI_Barlab

This repository contains the development of a virtual reality environment, called "The Barlab".

# Overview

This project consists of three parts:

1. Environment (the barlab)
2. Bottles and Liquids
3. Interactions in VR

## Environment

The environment is built as a 3D-Unity-project. It consists of normal 3D-objects that are either custom-made or downloaded for free from the Unity Asset Store.

## Bottles and Liquids

As this project is supposed to be used in exposure-therapy, it is important that the bottles and liquids look realistic. Thus, the bottles are custom-made to look similar to real bottles.
The liquids are split into 3 parts: Liquids in Bottles, Pouring Liquids into Glasses and Liquids in Glasses. The liquid-simulation in the bottles and glasses is done by using custom Shader Graphs, while the pouring simulation is implemented with ObiFluid.

## Interactions

The prototype uses hand-tracking as a default mode. No controllers are required to use the prototype. Bottles and Glasses are grabbable with custom hand gestures.

# Dependencies

This project has been developed with the Meta Quest 3.

The operability with other HMDs has not been tested, except for the Quest 2.

# Changing Bottles

Changing the bottles and glasses is done in the editor of Unity. There, simply deactivate bottles and glasses that you want to remove and activate those that are required. This project has been designed with the requirement that only one bottle and one glass at a time will be tested.

Bottles do not to need to be moved to a specific place as on start they will snap in front of the player.
