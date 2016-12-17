using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]

public class Chunk : MonoBehaviour
{

  public Block[, ,] blocks = new Block[chunkSize, chunkSize, chunkSize];

  public static int chunkSize = 16;
  public bool update = false;
  public bool DisplayLimits = true;
  public bool rendered;

  MeshFilter filter;
  MeshCollider coll;

  public World world;
  public WorldPos pos;

  void Start()
  {
    filter = gameObject.GetComponent<MeshFilter>();
    coll = gameObject.GetComponent<MeshCollider>();
  }

  //Update is called once per frame
  void Update()
  {
    if (update)
    {
      update = false;
      UpdateChunk();
    }

    if (DisplayLimits) {
      Debug.DrawLine(new Vector3(pos.x, pos.y, pos.z), new Vector3(pos.x, pos.y+chunkSize, pos.z), Color.red);
      Debug.DrawLine(new Vector3(pos.x+chunkSize, pos.y, pos.z), new Vector3(pos.x+chunkSize, pos.y+chunkSize, pos.z), Color.red);
      Debug.DrawLine(new Vector3(pos.x+chunkSize, pos.y, pos.z+chunkSize), new Vector3(pos.x+chunkSize, pos.y+chunkSize, pos.z+chunkSize), Color.red);
      Debug.DrawLine(new Vector3(pos.x, pos.y, pos.z+chunkSize), new Vector3(pos.x, pos.y+chunkSize, pos.z+chunkSize), Color.red);

      Debug.DrawLine(new Vector3(pos.x, pos.y, pos.z), new Vector3(pos.x+chunkSize, pos.y, pos.z), Color.red);
      Debug.DrawLine(new Vector3(pos.x+chunkSize, pos.y, pos.z), new Vector3(pos.x+chunkSize, pos.y, pos.z+chunkSize), Color.red);
      Debug.DrawLine(new Vector3(pos.x+chunkSize, pos.y, pos.z+chunkSize), new Vector3(pos.x, pos.y, pos.z+chunkSize), Color.red);
      Debug.DrawLine(new Vector3(pos.x, pos.y, pos.z), new Vector3(pos.x, pos.y, pos.z+chunkSize), Color.red);

      Debug.DrawLine(new Vector3(pos.x, pos.y+chunkSize, pos.z), new Vector3(pos.x+chunkSize, pos.y+chunkSize, pos.z), Color.red);
      Debug.DrawLine(new Vector3(pos.x+chunkSize, pos.y+chunkSize, pos.z), new Vector3(pos.x+chunkSize, pos.y+chunkSize, pos.z+chunkSize), Color.red);
      Debug.DrawLine(new Vector3(pos.x+chunkSize, pos.y+chunkSize, pos.z+chunkSize), new Vector3(pos.x, pos.y+chunkSize, pos.z+chunkSize), Color.red);
      Debug.DrawLine(new Vector3(pos.x, pos.y+chunkSize, pos.z), new Vector3(pos.x, pos.y+chunkSize, pos.z+chunkSize), Color.red);
    }
  }

  public Block GetBlock(int x, int y, int z)
  {
    if (InRange(x) && InRange(y) && InRange(z))
    return blocks[x, y, z];
    return world.GetBlock(pos.x + x, pos.y + y, pos.z + z);
  }

  public static bool InRange(int index)
  {
    if (index < 0 || index >= chunkSize)
    return false;

    return true;
  }

  public void SetBlock(int x, int y, int z, Block block)
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

  public void SetBlocksUnmodified()
  {
    foreach (Block block in blocks)
    {
      block.changed = false;
    }
  }

  // Updates the chunk based on its contents
  void UpdateChunk()
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

  // Sends the calculated mesh information
  // to the mesh and collision components
  void RenderMesh(MeshData meshData)
  {
    filter.mesh.Clear();
    filter.mesh.vertices = meshData.vertices.ToArray();
    filter.mesh.triangles = meshData.triangles.ToArray();

    filter.mesh.uv = meshData.uv.ToArray();
    filter.mesh.RecalculateNormals();

    coll.sharedMesh = null;
    Mesh mesh = new Mesh();
    mesh.vertices = meshData.colVertices.ToArray();
    mesh.triangles = meshData.colTriangles.ToArray();
    mesh.RecalculateNormals();

    coll.sharedMesh = mesh;
  }

}
