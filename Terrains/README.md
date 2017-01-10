# Terrains
This folder contains terrains generated from scripts

## Boxel
Voxel world generate a world of blocks and chunks, for now their is only a basic
perlin noise generation. The chunk size can set in chunck.cs,
please note that the bigger the chunks to less fps ;)

### Blocks
The blocks all inherit from Block.cs and should be in the Voxel/Blocks folder.
Each block as a msubMesh index corresponding to one of the chunk mesh materials
depending on the face orientation.
To get a voxel world like minecraft without slopes juste set smoothEdge to false
in Block.cs

## ProceduralTerrain
This is a non editable chunk for faster rendering. The chunks only render the
world in grass for now.

`to be used for static worlds. NON EDITABLE`

## Generators
The generators are still a work in progress. The idea is to use this class and
one (or maybe more) threads to generate the chunks datas. For now it only
generates the heightmap.

##Saves
The world save only saves edited blocks to avoid huge save files in the folder:
voxelGameSaves/worldname/x,y,z.bin
1 file for each chunk
I've disabled the saves for now it's only 3 line commented in world.cs
