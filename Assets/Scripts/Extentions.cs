using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Author: Microsoft
 * 
 * 
 * URL: https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/extension-methods 
 */

public static class Extentions
{
    public static Vector2Int GetPointAround(this int[,] arr, Vector2Int index_)
    {
        int x_dim = arr.GetLength(0);
        int y_dim = arr.GetLength(1);

        Vector2Int newPos = index_;
        int fix = 0;
        while (newPos == index_ && fix < 15)
        {
            newPos.x = Random.Range(index_.x - 1, index_.x + 1);
            newPos.x = Mathf.Clamp(newPos.x, 0, x_dim);

            newPos.y = Random.Range(index_.y - 1, index_.y + 1);
            newPos.y = Mathf.Clamp(newPos.y, 0, y_dim);
            fix++;
        }

        return newPos;
    }
}
