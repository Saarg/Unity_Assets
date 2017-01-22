using UnityEngine;
using System.Collections;

public struct ChunkData {
  public int _chunkSize;
  public float[, ] _heightMap;
  public Block[, ,] _blocks;

  public ChunkData(int chunkSize) {
    _chunkSize = chunkSize;
    _heightMap = new float[_chunkSize, _chunkSize];
    _blocks = new Block[_chunkSize, _chunkSize, _chunkSize];
  }
}
