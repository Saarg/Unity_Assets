using UnityEngine;
using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

public class Generator : MonoBehaviour {
  protected int _chunkSize;
  public Dictionary<WorldPos, ChunkData> chunkDatas = new Dictionary<WorldPos, ChunkData>();
  public Material[] _materials;

  // Generator thread variables
  private Thread _generatorThread;
  private Queue<WorldPos> _chunkQueue = new Queue<WorldPos>();
  protected bool _continueTh = true;

  void Start() {
    _chunkSize = Chunk.chunkSize;

    _generatorThread = new Thread(new ThreadStart(GeneratorTh));
    _generatorThread.Start();
  }

  void OnDestroy() {
    _continueTh = false;
  }

  public float GetHeight(int x, int y, int z) {
    ChunkData chunkdata = GetChunkData(x, y, z);

    return chunkdata._heightMap[Mathf.Abs(x)%_chunkSize, Mathf.Abs(z)%_chunkSize];
  }

  public ChunkData GetChunkData(int x, int y, int z) {
    WorldPos worldPos = new WorldPos(x/_chunkSize, y/_chunkSize, z/_chunkSize);

    ChunkData chunkdata;
    if(!chunkDatas.ContainsKey(worldPos)) {
      chunkdata = Generate(worldPos);

      if(_continueTh) {
        foreach (WorldPos newPos in chunkPositions) {
          _chunkQueue.Enqueue(new WorldPos(x+newPos.x, y, z+newPos.z));
        }
      }
    } else {
      chunkDatas.TryGetValue(worldPos, out chunkdata);
    }

    return chunkdata;
  }

  public virtual ChunkData Generate(WorldPos pos) {
    ChunkData chunkData;
    if (!chunkDatas.TryGetValue(pos, out chunkData))
    {
      chunkData = new ChunkData(_chunkSize);

      for (int xi = 0; xi < _chunkSize; xi++)
      {
        for (int zi = 0; zi < _chunkSize; zi++)
        {
          float height = Mathf.PerlinNoise((pos.x*_chunkSize + xi)/60.0f, (pos.z*_chunkSize + zi)/60.0f)*20.0f;
          chunkData._heightMap[xi, zi] = height;

          for (int yi = 0; yi < Chunk.chunkSize; yi++)
          {
            chunkData._blocks[xi, yi, zi] = pos.y + yi < height - 2 ? new Block() : (pos.y + yi < height ? new BlockGrass() as Block : new BlockAir() as Block);
          }
        }
      }

      chunkDatas.Add(pos, chunkData);
    }

    return chunkData;
  }

  private void GeneratorTh() {
    Debug.Log("Starting terrain generator thread");
    while(_continueTh) {
      if(_chunkQueue.Count > 0) {
        WorldPos worldPos = _chunkQueue.Dequeue();

        ChunkData chunkData = Generate(worldPos);
      }
    }
    Debug.Log("Ending terrain generator thread");
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
    new WorldPos(-2, 0,  3), new WorldPos( 2, 0, -3), new WorldPos( 2, 0,  3), new WorldPos( 3, 0, -2), new WorldPos( 3, 0,  2)
  };
}
