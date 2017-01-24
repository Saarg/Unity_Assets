using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]

public class Chunk : MonoBehaviour
{
  public static int chunkSize = 10;
  public bool save = false;

  public bool update = false;
  public bool DisplayLimits = true;
  public bool generated = false;
  public bool rendered;

  protected MeshFilter filter;
  protected MeshCollider coll;

  public World world;
  public WorldPos pos;

  void Start() {
    filter = gameObject.GetComponent<MeshFilter>();
    coll = gameObject.GetComponent<MeshCollider>();
  }

  void Update() {
    if (update) {
      update = false;
      StartCoroutine(UpdateChunk());
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

  public virtual void Generate(Generator generator) {
    generated = true;
  }

  public virtual Block GetBlock(int x, int y, int z) {
    return new BlockAir();
  }

  public virtual void SetBlock(int x, int y, int z, Block block) {

  }

  public virtual void SetBlocksUnmodified() {

  }

  // Updates the chunk based on its contents
  protected virtual IEnumerator UpdateChunk() {
    yield return null;
  }

  // Sends the calculated mesh information
  // to the mesh and collision components
  protected void RenderMesh(MeshData meshData) {
    filter.mesh.Clear();
    filter.mesh.vertices = meshData.vertices.ToArray();
    filter.mesh.uv = meshData.uv.ToArray();

    //filter.mesh.subMeshCount = 2;
    foreach (int key in meshData.trianglesDict.Keys) {
      filter.mesh.subMeshCount = filter.mesh.subMeshCount <= key ? key+1 : filter.mesh.subMeshCount;

      filter.mesh.SetTriangles(meshData.trianglesDict[key].ToArray(), key);
    }

    filter.mesh.RecalculateNormals();

    coll.sharedMesh = null;
    Mesh mesh = new Mesh();
    mesh.vertices = meshData.colVertices.ToArray();
    mesh.triangles = meshData.colTriangles.ToArray();
    mesh.RecalculateNormals();

    coll.sharedMesh = mesh;
  }

  public static bool InRange(int index)
  {
    if (index < 0 || index >= chunkSize)
    return false;

    return true;
  }

  public static bool InRange(float index)
  {
    if (index < 0.0f || index >= chunkSize)
    return false;

    return true;
  }
}
