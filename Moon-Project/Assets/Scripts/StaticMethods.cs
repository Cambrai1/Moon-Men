using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticMethods
{
    public static Vector2 TopDown(Vector3 _vec)
    {
        Vector2 result = Vector2.zero;

        result.x = _vec.x;
        result.y = _vec.y;

        return result;
    }
}