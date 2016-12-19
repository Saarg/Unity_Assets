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
    WorldPos pos = new WorldPos();
    pos.x = Mathf.FloorToInt(x / Chunk.chunkSize) * Chunk.chunkSize;
    pos.y = Mathf.FloorToInt(y / Chunk.chunkSize) * Chunk.chunkSize;
    pos.z = Mathf.FloorToInt(z / Chunk.chunkSize) * Chunk.chunkSize;

    ChunkData chunkData;
    if (!chunkDatas.TryGetValue(pos, out chunkData))
    {
      chunkData = Generate(pos);
    }

    return chunkData._heightMap[x%_chunkSize, z%_chunkSize];
  }

  public virtual ChunkData Generate(WorldPos pos) {
    ChunkData chunkData;
    if (!chunkDatas.TryGetValue(pos, out chunkData))
    {
      chunkData = new ChunkData(_chunkSize);

      for (int xi = 0; xi < Chunk.chunkSize; xi++)
      {
        for (int zi = 0; zi < Chunk.chunkSize; zi++)
        {
          chunkData._heightMap[xi, zi] = Mathf.PerlinNoise((pos.x + xi)/60.0f, (pos.z + zi)/60.0f)*20.0f;
        }
      }
    }

    return chunkData;
  }
}
