using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ai_Vision_Factory : MonoBehaviour
{
    public Ai_Vision_Tile[,] vision_field;
    


    public void RegenVision(Tile[,] tilemap)
    {
        int width = tilemap.GetLength(0);
        int height = tilemap.GetLength(1);

        vision_field = new Ai_Vision_Tile[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                byte owner = tilemap[x, y].IsOccupied ? GameManager.instance.GetOwneronTile(tilemap[x, y]) : (byte)127;
                vision_field[x, y] = new Ai_Vision_Tile(tilemap[x, y].IsOccupied, 0, x, y, owner);
                     
            }
        }
    }
}
