using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoxelChunk : Chunk
{
  public ChunkData chunkData;
  private Block[, ,] _blocks = new Block[chunkSize, chunkSize, chunkSize];

  public override void Generate(Generator generator){
    save = true;

    chunkData = generator.GetChunkData(pos.x, pos.y, pos.z);

    for (int xi = 0; xi < chunkSize; xi++)
    {
      for (int zi = 0; zi < chunkSize; zi++)
      {
        float height = chunkData._heightMap[xi, zi];

        for (int yi = 0; yi < chunkSize; yi++)
        {
           world.SetBlock(pos.x + xi, pos.y + yi, pos.z + zi, pos.y + yi < height - 2 ? new Block() : (pos.y + yi < height ? new BlockGrass() as Block : new BlockAir() as Block));
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
      return _blocks[x, y, z];
    return world.GetBlock(pos.x + x, pos.y + y, pos.z + z);
  }

  public override void SetBlock(int x, int y, int z, Block block)
  {
    if (InRange(x) && InRange(y) && InRange(z))
    {
      _blocks[x, y, z] = block;
    }
    else
    {
      world.SetBlock(pos.x + x, pos.y + y, pos.z + z, block);
    }
  }

  public override void SetBlocksUnmodified()
  {
    foreach (Block block in _blocks)
    {
      block.changed = false;
    }
  }

  protected override IEnumerator UpdateChunk()
  {
    if(!generated) yield break;

    rendered = true;
    MeshData meshData = new MeshData();
    yield return null;

    for (int x = 0; x < chunkSize; x++)
    {
      for (int y = 0; y < chunkSize; y++)
      {
        for (int z = 0; z < chunkSize; z++)
        {
          meshData = _blocks[x, y, z].Blockdata(this, x, y, z, meshData);
        }
      }
      yield return null;
    }

    yield return null;

    RenderMesh(meshData);
    yield return null;
  }
}
