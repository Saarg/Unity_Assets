# Terrains
This folder contains will contain my terrains generated from scripts

## Voxel
Voxel world generate a world of blocks and chunks, for now their is only a basic
perlin noise generation. The chunk size can set in chunck.cs or chunk prefab,
please note that the bigger the chunks to less fps ;)

### Blocks
The blocks all inherit from Block.cs and should be in the Voxel/Blocks folder.
Each block as a msubMesh index corresponding to one of the chunk mesh materials
depending on the face orientation.
To get a voxel world like minecraft without slopes juste set smoothEdge to false
in Block.cs

## Generation
SOON different generation algorithm working with every Terrains.
That's for later since it's time for some vacation \o/

##Saves
The world save only saves edited blocks to avoid huge save files in the folder:
voxelGameSaves/worldname/x,y,z.bin
1 file for each chunk
