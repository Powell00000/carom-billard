using UnityEngine;

public static class Vector3Ex
{
    public static Vector3 WithX(this Vector3 vec, float X = 0)
    {
        vec.x = X;
        return vec;
    }

    public static Vector3 WithY(this Vector3 vec, float Y = 0)
    {
        vec.y = Y;
        return vec;
    }

    public static Vector3 WithZ(this Vector3 vec, float Z = 0)
    {
        vec.z = Z;
        return vec;
    }
}
