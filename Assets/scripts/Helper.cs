using System;
using UnityEngine;

public class Helper{
  public static Vector3 Normalize(Vector3 vec, Vector3 min, Vector3 max)
  {
      Vector3 range = max - min;

      float xNormalized = (vec.x - min.x) / range.x;
      float yNormalized = (vec.y - min.y) / range.y;
      float zNormalized = (vec.z - min.z) / range.z;

      return new Vector3(xNormalized, yNormalized, zNormalized);
  }

}