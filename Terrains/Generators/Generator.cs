using UnityEngine;
using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

public class Generator {
  protected int _chunkSize;
  public Dictionary<WorldPos, ChunkData> chunkDatas = new Dictionary<WorldPos, ChunkData>();

  private Thread _generatorThread;
  private Queue<WorldPos> _chunkQueue = new Queue<WorldPos>();

  public Generator() {

  }

  public void Init() {
    _chunkSize = Chunk.chunkSize;

    _generatorThread = new Thread(new ThreadStart(GeneratorTh));
    _generatorThread.Start();
  }

  public float GetHeight(int x, int y, int z) {
    WorldPos worldPos = new WorldPos(x/_chunkSize, y/_chunkSize, z/_chunkSize);

    ChunkData chunkdata;
    if(!chunkDatas.ContainsKey(worldPos)) {
      chunkdata = Generate(worldPos);
      chunkDatas.Add(worldPos, chunkdata);

      _chunkQueue.Enqueue(new WorldPos(x/_chunkSize - 1, y/_chunkSize, z/_chunkSize - 1));
      _chunkQueue.Enqueue(new WorldPos(x/_chunkSize - 1, y/_chunkSize, z/_chunkSize));
      _chunkQueue.Enqueue(new WorldPos(x/_chunkSize - 1, y/_chunkSize, z/_chunkSize + 1));
      _chunkQueue.Enqueue(new WorldPos(x/_chunkSize, y/_chunkSize, z/_chunkSize - 1));
      _chunkQueue.Enqueue(new WorldPos(x/_chunkSize, y/_chunkSize, z/_chunkSize + 1));
      _chunkQueue.Enqueue(new WorldPos(x/_chunkSize + 1, y/_chunkSize, z/_chunkSize - 1));
      _chunkQueue.Enqueue(new WorldPos(x/_chunkSize + 1, y/_chunkSize, z/_chunkSize));
      _chunkQueue.Enqueue(new WorldPos(x/_chunkSize + 1, y/_chunkSize, z/_chunkSize + 1));

    } else {
      chunkDatas.TryGetValue(worldPos, out chunkdata);
    }

    return chunkdata._heightMap[x%_chunkSize, z%_chunkSize];
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
          chunkData._heightMap[xi, zi] = Mathf.PerlinNoise((pos.x*_chunkSize + xi)/60.0f, (pos.z*_chunkSize + zi)/60.0f)*20.0f;
        }
      }
    }

    return chunkData;
  }

  private void GeneratorTh() {
    Debug.Log("Starting terrain generator thread");
    while(true) {
      if(_chunkQueue.Count > 0) {
        WorldPos worldPos = _chunkQueue.Dequeue();

        Generate(worldPos);
      }
    }
  }
}
