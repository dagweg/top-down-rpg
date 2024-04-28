using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public static class VectorExtensions
{
    public static Vector3 OnlyX(this Vector3 vector)
    {
        return new Vector3(vector.x, 0f, 0f);
    }

    public static Vector3 OnlyY(this Vector3 vector)
    {
        return new Vector3(0f, vector.y, 0f);
    }

    public static Vector3 OnlyZ(this Vector3 vector)
    {
        return new Vector3(0f, 0f, vector.z);
    }

    public static Vector3 SetZ(this Vector3 vector, float z=1f){
        return new Vector3(vector.x,vector.y,z);
    }
}
