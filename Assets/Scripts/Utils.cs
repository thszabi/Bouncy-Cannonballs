using UnityEngine;

public static class Utils
{
    public static Vector2 MirrorVector(Vector2 vector, Vector2 mirrorVector)
    {
        mirrorVector = mirrorVector.normalized;
        return vector - 2.0f * Vector2.Dot(vector, mirrorVector) * mirrorVector;
    }
}
