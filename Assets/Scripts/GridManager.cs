using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class GridManager : MonoBehaviour
{
    private const int POINT_LOOP = 2;
    private const int EXPAND_VALUE = 9;
    public static GridManager instance;
    private Tile[,] tileArray;
    public Tile[,] TileArray
    {
        get
        {
            return tileArray;
        }
    }
    int[,] map_Types;
    [SerializeField] private Vector2Int gridSize;
    [SerializeField] private Transform worldParent;
    [SerializeField] private GameObject prefabTile;
    [SerializeField] private Vector2 tileSize;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        GenerateMap();
    }



    /// <summary>
    /// Returns empty tile that is "walkable" and unoccupied
    /// </summary>
    /// <returns></returns>
    public Tile RandomTile(bool occupieOnGet = false)
    {
        Tile currentSelction = tileArray[UnityEngine.Random.Range(0, gridSize.x), UnityEngine.Random.Range(0, gridSize.y)];
        while (currentSelction.IsOccupied || currentSelction.type is Wall)
        {
            currentSelction = tileArray[UnityEngine.Random.Range(0, gridSize.x), UnityEngine.Random.Range(0, gridSize.y)];
        }
        if (occupieOnGet)
        {
            currentSelction.IsOccupied = true;
        }
        return currentSelction;

        //  return tileArray[UnityEngine.Random.Range(0, gridSize.x), UnityEngine.Random.Range(0, gridSize.y)];

    }

    [HideInInspector] public UnityEvent OnGenerated = new UnityEvent();

    public async void GenerateMap()
    {
        tileArray = new Tile[gridSize.x, gridSize.y];

        await GenerateTileTypes();
        await PlaceTiles();

        await ProcessTiles();

        OnGenerated.Invoke();

        //Debug.Log("Done");
    }

    private async Task GenerateTileTypes()
    {
        map_Types = new int[gridSize.x, gridSize.y];
        for (int i = 0; i < gridSize.y; i++)
        {
            for (int h = 0; h < gridSize.x; h++)
            {
                map_Types[h, i] = 0;
            }
        }

        for (int i = 0; i < POINT_LOOP; i++)
        {
            Vector2Int grid_point = new Vector2Int(Random.Range(0, gridSize.x), Random.Range(0, gridSize.y));
            map_Types[grid_point.x, grid_point.y] = 1;

            for (int j = 0; j < EXPAND_VALUE; j++)
            {
                Vector2Int newPoint = map_Types.GetPointAround(grid_point);
                grid_point = newPoint;
                map_Types[grid_point.x, grid_point.y] = 1;
            }
        }
    }
    private async Task PlaceTiles()
    {
        for (uint y = 0; y < gridSize.y; y++)
        {
            for (uint x = 0; x < gridSize.x; x++)
            {
                TileType tileType = new Gravel();

                if (map_Types[x,y]!=0)
                {
                    tileType = new Wall();
                }
                tileArray[x, y] = new Tile(tileType, x, y, tileSize);
                await Task.Delay(10);

            }
        }
    }

    private async Task ProcessTiles()
    {
        foreach (Tile item in tileArray)
        {
            item.ReadSurrounding(tileArray, gridSize);
            await Task.Delay(1);
        }
    }

    public Tile GetTileFromCoord(Vector2 pos)
    {
        int xPos = (int)(pos.x / tileSize.x);
        xPos = Mathf.Clamp(xPos, 0, (int)gridSize.x);

        int yPos = (int)(pos.y / tileSize.y);
        yPos = Mathf.Clamp(yPos, 0, (int)gridSize.y);

        return tileArray[xPos, yPos];
    }


}



[Serializable]
public class Tile
{

    [NonSerialized] private GameObject obj;
    public TileType type;
    uint x, y;
    public int X => (int)x;
    public int Y => (int)y;

    Vector2 worldPos;
    public Tile Top, Bottom, Right, Left;
    public bool IsOccupied = false;


    public Vector2 WorldPos
    {
        get { return worldPos; }

        set
        {
            worldPos = value;
            obj.transform.position = worldPos;
        }
    }

    public Tile(TileType _type, uint _x, uint _y, Vector2 tileSize)
    {
        type = _type;
        x = _x;
        y = _y;


        // Debug.Log(type.Asset);
        obj = GameObject.Instantiate(Resources.Load(type.Asset) as GameObject);

        worldPos = new Vector3(tileSize.x / 2 + tileSize.x * x, tileSize.y / 2 + tileSize.y * y, 0);
        obj.transform.position = worldPos;
    }

    public void ReadSurrounding(Tile[,] tiles, Vector2 mapsize)
    {
        if (x > 0)
        {
            Left = tiles[x - 1, y];

        }
        if (x < mapsize.x - 1)
        {
            Right = tiles[x + 1, y];
        }

        if (y > 0)
        {
            Bottom = tiles[x, y - 1];
        }
        if (y < mapsize.y - 1)
        {
            Top = tiles[x, y + 1];
        }

    }

    public List<Tile> SurroundingTiles()
    {
        List<Tile> list = new List<Tile>();
        if (Top != null)
        {
            list.Add(Top);
        }

        if (Bottom != null)
        {
            list.Add(Bottom);
        }

        if (Left != null)
        {
            list.Add(Left);
        }

        if (Right != null)
        {
            list.Add(Right);
        }

        return list;

    }
    public bool IsWalkable()
    {
        return type.IsWalkable;
    }

    public Type GetTileType()
    {
        return type.GetType();
    }
}
