using UnityEngine;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

public class LoadChunks : MonoBehaviour {
  public World world;
  public int _renderDistance = 256;

  int timer = 0;

  List<WorldPos> updateList = new List<WorldPos>();
  List<WorldPos> buildList = new List<WorldPos>();

  private WorldPos playerPos;

  private Thread _chunkWorker;
  protected bool _continueTh = true;

  void Start() {
    _chunkWorker = new Thread(new ThreadStart(FindChunksToLoad));
    _chunkWorker.Start();

    StartCoroutine("LoadAndRenderChunks");
  }

  void OnDestroy() {
    _continueTh = false;
  }

  void Update () {
    //Get the position of this gameobject to generate around
    playerPos = new WorldPos(
      Mathf.FloorToInt(transform.position.x / Chunk.chunkSize) * Chunk.chunkSize,
      Mathf.FloorToInt(transform.position.y / Chunk.chunkSize) * Chunk.chunkSize,
      Mathf.FloorToInt(transform.position.z / Chunk.chunkSize) * Chunk.chunkSize
    );

    DeleteChunks();
  }

  void FindChunksToLoad()
  {
    Debug.Log("Starting Chunk thread");
    while(_continueTh) {
      //If there aren't already chunks to generate
      if (updateList.Count == 0)
      {
        int added = 0;
        //Cycle through the array of positions
        for (int i = 0; i < chunkPositions.Length; i++)
        {
          int posy;
          try {
            float height = world.terrain.GetHeight(playerPos.x + chunkPositions[i].x * Chunk.chunkSize, 0, playerPos.z + chunkPositions[i].z * Chunk.chunkSize);
            posy = Mathf.FloorToInt(height / Chunk.chunkSize) * Chunk.chunkSize;
          } catch {
            break;
          }

          //translate the player position and array position into chunk position
          WorldPos newChunkPos = new WorldPos(
            chunkPositions[i].x * Chunk.chunkSize + playerPos.x,
            chunkPositions[i].y * Chunk.chunkSize + posy,
            chunkPositions[i].z * Chunk.chunkSize + playerPos.z
          );

          //load a column of chunks in this position
          for (int y = newChunkPos.y - Chunk.chunkSize; y <= newChunkPos.y + Chunk.chunkSize; y += Chunk.chunkSize)
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

          if(added > 0) {
            break;
          }
        }
      }
    }
    Debug.Log("Ending Chunk thread");
  }

  void BuildChunk(WorldPos pos)
  {
    if (world.GetChunk(pos.x,pos.y,pos.z) == null)
      world.CreateChunk(pos.x,pos.y,pos.z);
  }

  IEnumerator LoadAndRenderChunks()
  {
    while (true) {
      float startTime = Time.realtimeSinceStartup;
      if (buildList.Count != 0)
      {
        for (int i = 0; i < buildList.Count; i++)
        {
          BuildChunk(buildList[0]);
          buildList.RemoveAt(0);

          yield return null;
        }
      }
      if (updateList.Count!=0)
      {
        Chunk chunk = world.GetChunk(updateList[0].x, updateList[0].y, updateList[0].z);
        if (chunk != null && chunk.generated) {
          chunk.update = true;
          updateList.RemoveAt(0);
        }
      }
      yield return null;
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

  static  WorldPos[] chunkPositions= {
    new WorldPos( 0, 0,  0), new WorldPos(-1, 0,  0), new WorldPos( 0, 0, -1), new WorldPos( 0, 0,  1), new WorldPos( 1, 0,  0),
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
    new WorldPos( 5, 0,  6), new WorldPos( 6, 0, -5), new WorldPos( 6, 0,  5)
  };
  }
