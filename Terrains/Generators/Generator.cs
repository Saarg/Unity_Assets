using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Generator {
  private int _chunkSize;
  public Dictionary<WorldPos, ChunkData> chunkDatas = new Dictionary<WorldPos, ChunkData>();

  public Generator() {
    _chunkSize = Chunk.chunkSize;
  }

  public float GetHeight(int x, int y, int z) {
    WorldPos worldPos = new WorldPos(x/_chunkSize, y/_chunkSize, z/_chunkSize);

    ChunkData chunkdata;
    if(!chunkDatas.ContainsKey(worldPos)) {
      chunkdata = Generate(worldPos);
      chunkDatas.Add(worldPos, chunkdata);
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
}
