using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class World : MonoBehaviour {

  public Dictionary<WorldPos, Chunk> chunks = new Dictionary<WorldPos, Chunk>();
  public GameObject chunkPrefab;

  public string worldName = "world";

  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }

  public void CreateChunk(int x, int y, int z)
  {
    WorldPos worldPos = new WorldPos(x, y, z);

    //Instantiate the chunk at the coordinates using the chunk prefab
    GameObject newChunkObject = Instantiate(
                                  chunkPrefab, new Vector3(worldPos.x, worldPos.y, worldPos.z),
                                  Quaternion.Euler(Vector3.zero)
                                ) as GameObject;

    newChunkObject.transform.SetParent(transform);

    Chunk newChunk = newChunkObject.GetComponent<Chunk>();

    newChunk.pos = worldPos;
    newChunk.world = this;

    //Add it to the chunks dictionary with the position as the key
    chunks.Add(worldPos, newChunk);

    for (int xi = 0; xi < Chunk.chunkSize; xi++)
    {
      for (int yi = 0; yi < Chunk.chunkSize; yi++)
      {
        for (int zi = 0; zi < Chunk.chunkSize; zi++)
        {
          if (y + yi <= 2+Mathf.Sqrt((x + xi)*(x + xi) + (z + zi)*(z + zi))/10 - 1)
          {
            SetBlock(x + xi, y + yi, z + zi, new Block());
          }
          else if (y + yi <= 2+Mathf.Sqrt((x + xi)*(x + xi) + (z + zi)*(z + zi))/10)
          {
            SetBlock(x + xi, y + yi, z + zi, new BlockGrass());
          }
          else
          {
            SetBlock(x + xi, y + yi, z + zi, new BlockAir());
          }
        }
      }
    }

    newChunk.SetBlocksUnmodified();

    Serialization.Load(newChunk);
  }

  public void DestroyChunk(int x, int y, int z)
  {
    Chunk chunk = null;
    if (chunks.TryGetValue(new WorldPos(x, y, z), out chunk))
    {
      Serialization.SaveChunk(chunk);
      Object.Destroy(chunk.gameObject);
      chunks.Remove(new WorldPos(x, y, z));
    }
  }

  public Chunk GetChunk(int x, int y, int z)
  {
    WorldPos pos = new WorldPos();
    float multiple = Chunk.chunkSize;
    pos.x = Mathf.FloorToInt(x / multiple) * Chunk.chunkSize;
    pos.y = Mathf.FloorToInt(y / multiple) * Chunk.chunkSize;
    pos.z = Mathf.FloorToInt(z / multiple) * Chunk.chunkSize;

    Chunk containerChunk = null;

    chunks.TryGetValue(pos, out containerChunk);

    return containerChunk;
  }

  public Block GetBlock(int x, int y, int z)
  {
    Chunk containerChunk = GetChunk(x, y, z);

    if (containerChunk != null)
    {
      Block block = containerChunk.GetBlock(
      x - containerChunk.pos.x,
      y - containerChunk.pos.y,
      z - containerChunk.pos.z);

      return block;
    }
    else
    {
      return new BlockAir();
    }

  }

  public void SetBlock(int x, int y, int z, Block block)
  {
    Chunk chunk = GetChunk(x, y, z);

    if (chunk != null)
    {
      chunk.SetBlock(x - chunk.pos.x, y - chunk.pos.y, z - chunk.pos.z, block);
      chunk.update = true;

      UpdateIfEqual(x - chunk.pos.x, 0, new WorldPos(x - 1, y, z));
      UpdateIfEqual(x - chunk.pos.x, Chunk.chunkSize - 1, new WorldPos(x + 1, y, z));
      UpdateIfEqual(y - chunk.pos.y, 0, new WorldPos(x, y - 1, z));
      UpdateIfEqual(y - chunk.pos.y, Chunk.chunkSize - 1, new WorldPos(x, y + 1, z));
      UpdateIfEqual(z - chunk.pos.z, 0, new WorldPos(x, y, z - 1));
      UpdateIfEqual(z - chunk.pos.z, Chunk.chunkSize - 1, new WorldPos(x, y, z + 1));

      UpdateIfBothEqual(x - chunk.pos.x, 0, new WorldPos(x - 1, y, z - 1), y - chunk.pos.y, 0);
      UpdateIfBothEqual(x - chunk.pos.x, Chunk.chunkSize - 1, new WorldPos(x + 1, y, z - 1), y - chunk.pos.y, 0);
      UpdateIfBothEqual(x - chunk.pos.x, 0, new WorldPos(x - 1, y, z + 1), y - chunk.pos.y, Chunk.chunkSize - 1);
      UpdateIfBothEqual(x - chunk.pos.x, Chunk.chunkSize - 1, new WorldPos(x + 1, y, z + 1), y - chunk.pos.y, Chunk.chunkSize - 1);
    }
  }

  void UpdateIfEqual(int value1, int value2, WorldPos pos)
  {
    if (value1 == value2)
    {
      Chunk chunk = GetChunk(pos.x, pos.y, pos.z);
      if (chunk != null)
        chunk.update = true;
    }
  }

  void UpdateIfBothEqual(int value1, int value2, WorldPos pos, int value3, int value4)
  {
    if (value1 == value2 && value3 == value4)
    {
      Chunk chunk = GetChunk(pos.x, pos.y, pos.z);
      if (chunk != null)
        chunk.update = true;
    }
  }

  void OnDestroy() {
    foreach (Chunk chunk in chunks.Values) {
      Serialization.SaveChunk(chunk);
    }
  }
}
