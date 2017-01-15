using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class LoadChunks : MonoBehaviour {
  public World world;
  public int _renderDistance = 256;
  public int _loadingDistance = 6;
  public int _buildSpeed = 64;

  int timer = 0;

  List<WorldPos> updateList = new List<WorldPos>();
  List<WorldPos> buildList = new List<WorldPos>();

  void Update () {
    if (DeleteChunks())
      return;
    FindChunksToLoad();
    LoadAndRenderChunks();
  }

  void FindChunksToLoad()
  {
    //Get the position of this gameobject to generate around
    WorldPos playerPos = new WorldPos(
      Mathf.FloorToInt(transform.position.x / Chunk.chunkSize) * Chunk.chunkSize,
      Mathf.FloorToInt(transform.position.y / Chunk.chunkSize) * Chunk.chunkSize,
      Mathf.FloorToInt(transform.position.z / Chunk.chunkSize) * Chunk.chunkSize
    );
    //If there aren't already chunks to generate
    if (updateList.Count == 0)
    {
      int added = 0;
      //Cycle through the array of positions
      for (int i = 0; i < chunkPositions.Length && i < _loadingDistance*_loadingDistance; i++)
      {
        //translate the player position and array position into chunk position
        WorldPos newChunkPos = new WorldPos(
          chunkPositions[i].x * Chunk.chunkSize + playerPos.x,
          chunkPositions[i].y * Chunk.chunkSize + playerPos.y - Chunk.chunkSize,
          chunkPositions[i].z * Chunk.chunkSize + playerPos.z
        );

        //load a column of chunks in this position
        for (int y = newChunkPos.y - Chunk.chunkSize; y <= newChunkPos.y + 2*Chunk.chunkSize; y += Chunk.chunkSize)
        {
          WorldPos tmpPos = new WorldPos(newChunkPos.x, y, newChunkPos.z);

          Chunk newChunk = world.GetChunk(tmpPos.x, tmpPos.y, tmpPos.z);
          //If the chunk already exists and it's already
          //rendered or in queue to be rendered continue
          if (newChunk != null && (newChunk.rendered || updateList.Contains(newChunkPos))) {
            continue;
          }

          buildList.Add(tmpPos);
          updateList.Add(tmpPos);
          added++;
        }

        if(added > _buildSpeed) {
          return;
        }
      }
    }
  }

  void BuildChunk(WorldPos pos)
  {
    if (world.GetChunk(pos.x,pos.y,pos.z) == null)
      world.CreateChunk(pos.x,pos.y,pos.z);
  }

  void LoadAndRenderChunks()
  {
    if (buildList.Count != 0)
    {
      for (int i = 0; i < buildList.Count && i < _buildSpeed; i++)
      {
        BuildChunk(buildList[0]);
        buildList.RemoveAt(0);
      }
      //If chunks were built return early
      return;
    }
    if (updateList.Count!=0)
    {
      Chunk chunk = world.GetChunk(updateList[0].x, updateList[0].y, updateList[0].z);
      if (chunk != null)
        chunk.update = true;
      updateList.RemoveAt(0);
    }
  }

  bool DeleteChunks()
  {
    if (timer == 10)
    {
      var chunksToDelete = new List<WorldPos>();
      foreach (var chunk in world.chunks)
      {
        float distance = Vector3.Distance(
        new Vector3(chunk.Value.pos.x, 0, chunk.Value.pos.z),
        new Vector3(transform.position.x, 0, transform.position.z));
        if (distance > _renderDistance)
          chunksToDelete.Add(chunk.Key);
      }
      foreach (var chunk in chunksToDelete)
        world.DestroyChunk(chunk.x, chunk.y, chunk.z);
      timer = 0;
      return true;    //Add this line
    }
    timer++;
    return false;    //Add this line
  }

  static  WorldPos[] chunkPositions= {   new WorldPos( 0, 0,  0), new WorldPos(-1, 0,  0), new WorldPos( 0, 0, -1), new WorldPos( 0, 0,  1), new WorldPos( 1, 0,  0),
    new WorldPos(-1, 0, -1), new WorldPos(-1, 0,  1), new WorldPos( 1, 0, -1), new WorldPos( 1, 0,  1), new WorldPos(-2, 0,  0),
    new WorldPos( 0, 0, -2), new WorldPos( 0, 0,  2), new WorldPos( 2, 0,  0), new WorldPos(-2, 0, -1), new WorldPos(-2, 0,  1),
    new WorldPos(-1, 0, -2), new WorldPos(-1, 0,  2), new WorldPos( 1, 0, -2), new WorldPos( 1, 0,  2), new WorldPos( 2, 0, -1),
    new WorldPos( 2, 0,  1), new WorldPos(-2, 0, -2), new WorldPos(-2, 0,  2), new WorldPos( 2, 0, -2), new WorldPos( 2, 0,  2),
    new WorldPos(-3, 0,  0), new WorldPos( 0, 0, -3), new WorldPos( 0, 0,  3), new WorldPos( 3, 0,  0), new WorldPos(-3, 0, -1),
    new WorldPos(-3, 0,  1), new WorldPos(-1, 0, -3), new WorldPos(-1, 0,  3), new WorldPos( 1, 0, -3), new WorldPos( 1, 0,  3),
    new WorldPos( 3, 0, -1), new WorldPos( 3, 0,  1), new WorldPos(-3, 0, -2), new WorldPos(-3, 0,  2), new WorldPos(-2, 0, -3),
    new WorldPos(-2, 0,  3), new WorldPos( 2, 0, -3), new WorldPos( 2, 0,  3), new WorldPos( 3, 0, -2), new WorldPos( 3, 0,  2),
    new WorldPos(-4, 0,  0), new WorldPos( 0, 0, -4), new WorldPos( 0, 0,  4), new WorldPos( 4, 0,  0), new WorldPos(-4, 0, -1),
    new WorldPos(-4, 0,  1), new WorldPos(-1, 0, -4), new WorldPos(-1, 0,  4), new WorldPos( 1, 0, -4), new WorldPos( 1, 0,  4),
    new WorldPos( 4, 0, -1), new WorldPos( 4, 0,  1), new WorldPos(-3, 0, -3), new WorldPos(-3, 0,  3), new WorldPos( 3, 0, -3),
    new WorldPos( 3, 0,  3), new WorldPos(-4, 0, -2), new WorldPos(-4, 0,  2), new WorldPos(-2, 0, -4), new WorldPos(-2, 0,  4),
    new WorldPos( 2, 0, -4), new WorldPos( 2, 0,  4), new WorldPos( 4, 0, -2), new WorldPos( 4, 0,  2), new WorldPos(-5, 0,  0),
    new WorldPos(-4, 0, -3), new WorldPos(-4, 0,  3), new WorldPos(-3, 0, -4), new WorldPos(-3, 0,  4), new WorldPos( 0, 0, -5),
    new WorldPos( 0, 0,  5), new WorldPos( 3, 0, -4), new WorldPos( 3, 0,  4), new WorldPos( 4, 0, -3), new WorldPos( 4, 0,  3),
    new WorldPos( 5, 0,  0), new WorldPos(-5, 0, -1), new WorldPos(-5, 0,  1), new WorldPos(-1, 0, -5), new WorldPos(-1, 0,  5),
    new WorldPos( 1, 0, -5), new WorldPos( 1, 0,  5), new WorldPos( 5, 0, -1), new WorldPos( 5, 0,  1), new WorldPos(-5, 0, -2),
    new WorldPos(-5, 0,  2), new WorldPos(-2, 0, -5), new WorldPos(-2, 0,  5), new WorldPos( 2, 0, -5), new WorldPos( 2, 0,  5),
    new WorldPos( 5, 0, -2), new WorldPos( 5, 0,  2), new WorldPos(-4, 0, -4), new WorldPos(-4, 0,  4), new WorldPos( 4, 0, -4),
    new WorldPos( 4, 0,  4), new WorldPos(-5, 0, -3), new WorldPos(-5, 0,  3), new WorldPos(-3, 0, -5), new WorldPos(-3, 0,  5),
    new WorldPos( 3, 0, -5), new WorldPos( 3, 0,  5), new WorldPos( 5, 0, -3), new WorldPos( 5, 0,  3), new WorldPos(-6, 0,  0),
    new WorldPos( 0, 0, -6), new WorldPos( 0, 0,  6), new WorldPos( 6, 0,  0), new WorldPos(-6, 0, -1), new WorldPos(-6, 0,  1),
    new WorldPos(-1, 0, -6), new WorldPos(-1, 0,  6), new WorldPos( 1, 0, -6), new WorldPos( 1, 0,  6), new WorldPos( 6, 0, -1),
    new WorldPos( 6, 0,  1), new WorldPos(-6, 0, -2), new WorldPos(-6, 0,  2), new WorldPos(-2, 0, -6), new WorldPos(-2, 0,  6),
    new WorldPos( 2, 0, -6), new WorldPos( 2, 0,  6), new WorldPos( 6, 0, -2), new WorldPos( 6, 0,  2), new WorldPos(-5, 0, -4),
    new WorldPos(-5, 0,  4), new WorldPos(-4, 0, -5), new WorldPos(-4, 0,  5), new WorldPos( 4, 0, -5), new WorldPos( 4, 0,  5),
    new WorldPos( 5, 0, -4), new WorldPos( 5, 0,  4), new WorldPos(-6, 0, -3), new WorldPos(-6, 0,  3), new WorldPos(-3, 0, -6),
    new WorldPos(-3, 0,  6), new WorldPos( 3, 0, -6), new WorldPos( 3, 0,  6), new WorldPos( 6, 0, -3), new WorldPos( 6, 0,  3),
    new WorldPos(-7, 0,  0), new WorldPos( 0, 0, -7), new WorldPos( 0, 0,  7), new WorldPos( 7, 0,  0), new WorldPos(-7, 0, -1),
    new WorldPos(-7, 0,  1), new WorldPos(-5, 0, -5), new WorldPos(-5, 0,  5), new WorldPos(-1, 0, -7), new WorldPos(-1, 0,  7),
    new WorldPos( 1, 0, -7), new WorldPos( 1, 0,  7), new WorldPos( 5, 0, -5), new WorldPos( 5, 0,  5), new WorldPos( 7, 0, -1),
    new WorldPos( 7, 0,  1), new WorldPos(-6, 0, -4), new WorldPos(-6, 0,  4), new WorldPos(-4, 0, -6), new WorldPos(-4, 0,  6),
    new WorldPos( 4, 0, -6), new WorldPos( 4, 0,  6), new WorldPos( 6, 0, -4), new WorldPos( 6, 0,  4), new WorldPos(-7, 0, -2),
    new WorldPos(-7, 0,  2), new WorldPos(-2, 0, -7), new WorldPos(-2, 0,  7), new WorldPos( 2, 0, -7), new WorldPos( 2, 0,  7),
    new WorldPos( 7, 0, -2), new WorldPos( 7, 0,  2), new WorldPos(-7, 0, -3), new WorldPos(-7, 0,  3), new WorldPos(-3, 0, -7),
    new WorldPos(-3, 0,  7), new WorldPos( 3, 0, -7), new WorldPos( 3, 0,  7), new WorldPos( 7, 0, -3), new WorldPos( 7, 0,  3),
    new WorldPos(-6, 0, -5), new WorldPos(-6, 0,  5), new WorldPos(-5, 0, -6), new WorldPos(-5, 0,  6), new WorldPos( 5, 0, -6),
    new WorldPos( 5, 0,  6), new WorldPos( 6, 0, -5), new WorldPos( 6, 0,  5) };
  }
