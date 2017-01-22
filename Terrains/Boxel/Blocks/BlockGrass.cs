using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class BlockGrass : Block
{
  public BlockGrass() : base()
  {
  }

  public override int GetSubMesh(Direction direction) {
    switch (direction) {
      case Direction.up:
        return 1;
    }
    return 2;
  }
}
