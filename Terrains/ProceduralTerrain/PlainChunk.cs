using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlainChunk : Chunk
{
  private MeshData meshData = new MeshData();

  public override void Generate(Generator generator) {
    meshData.useRenderDataForCol = true;

    for (int xi = 0; xi < Chunk.chunkSize; xi++)
    {
      for (int zi = 0; zi < Chunk.chunkSize; zi++)
      {
        float[] height = new float[4] {
          generator.GetHeight(pos.x + xi, 0, pos.z + zi),
          generator.GetHeight(pos.x + xi, 0, pos.z + zi + 1),
          generator.GetHeight(pos.x + xi + 1, 0, pos.z + zi + 1),
          generator.GetHeight(pos.x + xi + 1, 0, pos.z + zi)
        };

        if (!InRange((int)(height[0]-pos.y))) {
          continue;
        }

        meshData.AddVertex(new Vector3(xi, height[0]-pos.y, zi));

        meshData.AddVertex(new Vector3(xi, height[1]-pos.y, zi + 1));

        meshData.AddVertex(new Vector3(xi + 1, height[2]-pos.y, zi + 1));

        meshData.AddVertex(new Vector3(xi + 1, height[3]-pos.y, zi));

        meshData.AddQuadTriangles(0);
      }
    }
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
    rendered = true;
    RenderMesh(meshData);
  }
}
