using UnityEngine;
using System.Collections;

public struct ChunkData {
  public int _chunkSize;
  public float[, ] _heightMap;

  public ChunkData(int chunkSize) {
    _chunkSize = chunkSize;
    _heightMap = new float[_chunkSize, _chunkSize];
  }
}
