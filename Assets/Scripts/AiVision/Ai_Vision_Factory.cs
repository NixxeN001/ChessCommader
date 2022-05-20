using SimpleNodes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Ai_Vision_Factory : MonoBehaviour
{
    public static Ai_Vision_Factory instance;

    public Ai_Vision_Tile[,] vision_field;
    public Node<Ai_Vision_Tile[,]> latestGen;
    private int lastPawnX = 0, lastPawnY = 0;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public Tuple<int, int> GetNextPawn()
    {
        for (int y = lastPawnY; y < vision_field.GetLength(1); y++)
        {
            for (int x = lastPawnX; x < vision_field.GetLength(0); x++)
            {
                if (vision_field[x, y].IsOccupied && vision_field[x, y].Owner == 2)
                {
                    return new Tuple<int, int>(x, y);
                }
            }
        }

        return new Tuple<int, int>(int.MaxValue, int.MaxValue);
    }

    public Tuple<int, int, int> GetClosestEnemyPawn(int x, int y, Ai_Vision_Tile[,] tilemap)
    {
        //Get a List of tiles that have an enemy Pawn on it
        List<Ai_Vision_Tile> enemyTiles = new List<Ai_Vision_Tile>();
        foreach (Ai_Vision_Tile tile in tilemap)
        {
            if (tile.IsOccupied && tile.Owner == 1)
            {
                enemyTiles.Add(tile);
            }
        }

        Ai_Vision_Tile closestEnemyTile = null;
        int closetDist = int.MaxValue;

        foreach (Ai_Vision_Tile eTile in enemyTiles)
        {
            int heuristicDistance =
                Mathf.Abs(eTile.X - x) + Mathf.Abs(eTile.Y - y);

            // Debug.Log(heuristicDistance);

            //checks which pawn is closests and sets it to closestEnemyTile
            if (heuristicDistance < closetDist)
            {
                closestEnemyTile = eTile;
            }
        }

        return new Tuple<int, int, int>(closestEnemyTile.X, closestEnemyTile.Y, closetDist);

    }

    public async void RegenVision(Tile[,] tilemap)
    {
        lastPawnX = 0;
        lastPawnY = 0;





        int width = tilemap.GetLength(0);
        int height = tilemap.GetLength(1);

        vision_field = new Ai_Vision_Tile[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                byte owner = tilemap[x, y].IsOccupied ? GameManager.instance.GetOwneronTile(tilemap[x, y]) : byte.MaxValue;
                vision_field[x, y] = new Ai_Vision_Tile(tilemap[x, y].IsOccupied, 0, x, y, owner);

            }
        }

        latestGen = new Node<Ai_Vision_Tile[,]>(vision_field);
        await latestGen.AppendCurrentNodeStateToDiskFileAsync();
    }
}
