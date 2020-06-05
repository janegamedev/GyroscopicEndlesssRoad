using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helper 
{
    public static Vector2 Rotate(this Vector2 v, float degrees) 
    {
        float radians = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);
         
        float tx = v.x;
        float ty = v.y;
 
        return new Vector2(cos * tx - sin * ty, sin * tx + cos * ty);
    }

    public static Vector2 GetNextRandomPosition(this Vector2 v, Vector2 previous, float a, float r)
    {
        Vector2 dir = (v - previous).normalized;
        return v + dir.Rotate(a) * r;
    }
}


