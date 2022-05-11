using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileType
{
    public string Name;
    public string Asset;
    public bool IsWalkable;
    public TileType(string _name, string _assetPath, bool _isWalkable)
    {
        Name = _name;
        Asset = _assetPath;
        IsWalkable = _isWalkable;
    }

}

public class Gravel : TileType
{
    public Gravel() : base("Gravel", "Gravel", true)
    {

    }
}

public class Wall : TileType
{
    public Wall() : base("Wall", "Wall", false)
    {

    }
}


