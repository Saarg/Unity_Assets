using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoxelChunk : Chunk
{
  public Block[, ,] blocks = new Block[chunkSize, chunkSize, chunkSize];

  public override void Generate(Generator generator){
    float startTime = Time.realtimeSinceStartup;
    save = true;

    for (int xi = 0; xi < Chunk.chunkSize; xi++)
    {
      for (int zi = 0; zi < Chunk.chunkSize; zi++)
      {
        float height = generator.GetHeight(pos.x + xi, 0, pos.z + zi);

        for (int yi = 0; yi < Chunk.chunkSize; yi++)
        {
          if (pos.y + yi <= height - 2)
          {
            world.SetBlock(pos.x + xi, pos.y + yi, pos.z + zi, new Block());
          }
          else if (pos.y + yi <= height)
          {
            world.SetBlock(pos.x + xi, pos.y + yi, pos.z + zi, new BlockGrass());
          }
          else
          {
            world.SetBlock(pos.x + xi, pos.y + yi, pos.z + zi, new BlockAir());
          }
        }
      }
    }

    SetBlocksUnmodified();
    Debug.Log("Chunk generated in: " + ((Time.realtimeSinceStartup - startTime)*1000) + "ms");
  }

  public override Block GetBlock(int x, int y, int z)
  {
    if (InRange(x) && InRange(y) && InRange(z))
    return blocks[x, y, z];
    return world.GetBlock(pos.x + x, pos.y + y, pos.z + z);
  }

  public override void SetBlock(int x, int y, int z, Block block)
  {
    if (InRange(x) && InRange(y) && InRange(z))
    {
      blocks[x, y, z] = block;
    }
    else
    {
      world.SetBlock(pos.x + x, pos.y + y, pos.z + z, block);
    }
  }

  public override void SetBlocksUnmodified()
  {
    foreach (Block block in blocks)
    {
      block.changed = false;
    }
  }

  protected override void UpdateChunk()
  {
    rendered = true;
    MeshData meshData = new MeshData();

    for (int x = 0; x < chunkSize; x++)
    {
      for (int y = 0; y < chunkSize; y++)
      {
        for (int z = 0; z < chunkSize; z++)
        {
          meshData = blocks[x, y, z].Blockdata(this, x, y, z, meshData);
        }
      }
    }

    RenderMesh(meshData);
  }
}
