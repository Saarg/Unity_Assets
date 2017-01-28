using UnityEngine;
using System;

[Serializable]
public struct SVector3{
  public float x;
  public float y;
  public float z;

  public SVector3(float x = 0, float y = 0, float z = 0) {
    this.x = x;
    this.y = y;
    this.z = z;
  }

  public Vector3 toVector() {
    return new Vector3(x, y, z);
  }

  public static SVector3 operator +(SVector3 a, SVector3 b) {
    return new SVector3(a.x+b.x, a.y+b.y, a.z+b.z);
  }

  public static SVector3 operator -(SVector3 a, SVector3 b) {
    return new SVector3(a.x-b.x, a.y-b.y, a.z-b.z);
  }

  public static SVector3 operator *(SVector3 a, SVector3 b) {
    return new SVector3(a.x*b.x, a.y*b.y, a.z*b.z);
  }

  public static SVector3 operator *(SVector3 a, float b) {
    return new SVector3(a.x*b, a.y*b, a.z*b);
  }

  public static SVector3 operator /(SVector3 a, SVector3 b) {
    return new SVector3(a.x/b.x, a.y/b.y, a.z/b.z);
  }

  public static SVector3 operator /(SVector3 a, float b) {
    return new SVector3(a.x/b, a.y/b, a.z/b);
  }

  public static bool operator ==(SVector3 a, SVector3 b) {
    return (a.x == b.x && a.y == b.y && a.z == b.z);
  }

  public static bool operator !=(SVector3 a, SVector3 b) {
    return (a.x != b.x || a.y != b.y || a.z != b.z);
  }

  public override bool Equals(object obj)
  {
    if (GetHashCode() == obj.GetHashCode())
    return true;
    return false;
  }

  public override int GetHashCode()
  {
    unchecked
    {
      int hash = 47;
      hash = hash * 227 + x.GetHashCode();
      hash = hash * 227 + y.GetHashCode();
      hash = hash * 227 + z.GetHashCode();
      return hash;
    }
  }
}
