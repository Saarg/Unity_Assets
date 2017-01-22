using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoxelChunk : Chunk
{
  public ChunkData chunkData;

  public override void Generate(Generator generator){
    save = true;

    chunkData = generator.GetChunkData(pos.x, pos.y, pos.z);

    // TODO better way to do this
    for (int xi = 0; xi < chunkSize; xi+=chunkSize-1)
    {
      for (int yi = 0; yi < chunkSize; yi+=chunkSize-1)
      {
        for (int zi = 0; zi < chunkSize; zi+=chunkSize-1)
        {
          world.SetBlock(pos.x + xi, pos.y + yi, pos.z + zi, chunkData._blocks[xi, yi, zi]);
        }
      }
    }

    SetBlocksUnmodified();

    generated = true;
    world.UpdateAround(pos);
  }

  public override Block GetBlock(int x, int y, int z)
  {
    if (InRange(x) && InRange(y) && InRange(z))
      return chunkData._blocks[x, y, z];
    return world.GetBlock(pos.x + x, pos.y + y, pos.z + z);
  }

  public override void SetBlock(int x, int y, int z, Block block)
  {
    if (InRange(x) && InRange(y) && InRange(z))
    {
      chunkData._blocks[x, y, z] = block;
    }
    else
    {
      world.SetBlock(pos.x + x, pos.y + y, pos.z + z, block);
    }
  }

  public override void SetBlocksUnmodified()
  {
    foreach (Block block in chunkData._blocks)
    {
      block.changed = false;
    }
  }

  protected override void UpdateChunk()
  {
    if(!generated) return;

    rendered = true;
    MeshData meshData = new MeshData();

    for (int x = 0; x < chunkSize; x++)
    {
      for (int y = 0; y < chunkSize; y++)
      {
        for (int z = 0; z < chunkSize; z++)
        {
          meshData = chunkData._blocks[x, y, z].Blockdata(this, x, y, z, meshData);
        }
      }
    }

    RenderMesh(meshData);
  }
}
