using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlainChunk : Chunk
{
  private MeshData meshData = new MeshData();

  public override IEnumerator Generate(Generator generator) {
    meshData.useRenderDataForCol = true;

    for (int xi = 0; xi < Chunk.chunkSize; xi++)
    {
      for (int zi = 0; zi < Chunk.chunkSize; zi++)
      {
        float[] height = new float[4] {
          generator.GetHeight(pos.x + xi, 0, pos.z + zi)-pos.y,
          generator.GetHeight(pos.x + xi, 0, pos.z + zi + 1)-pos.y,
          generator.GetHeight(pos.x + xi + 1, 0, pos.z + zi + 1)-pos.y,
          generator.GetHeight(pos.x + xi + 1, 0, pos.z + zi)-pos.y
        };

        if (!InRange(height[0]) && !InRange(height[1]) && !InRange(height[2]) && !InRange(height[3])) {
          continue;
        }

        meshData.AddVertex(new Vector3(xi, height[0], zi));

        meshData.AddVertex(new Vector3(xi, height[1], zi + 1));

        meshData.AddVertex(new Vector3(xi + 1, height[2], zi + 1));

        meshData.AddVertex(new Vector3(xi + 1, height[3], zi));

        meshData.AddQuadTriangles(0);
      }
    }
    generated = true;

    // yield return new WaitForSeconds(2);
    // world.UpdateAround(pos);
    yield return null;
  }

  public override Block GetBlock(int x, int y, int z)
  {
    return new Block();
  }

  public override void SetBlock(int x, int y, int z, Block block)
  {

  }

  public override void SetBlocksUnmodified()
  {

  }

  protected override void UpdateChunk()
  {
    if(!generated) return;

    rendered = true;
    RenderMesh(meshData);
  }
}
