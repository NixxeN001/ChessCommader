using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Ai_Vision_Tile 
{
    public bool IsOccupied;
    public byte UnitType;
    public int X, Y;
    public byte Owner;
    
    public Ai_Vision_Tile(bool isOccupied, byte unitType,int x,int y, byte owner)
    {
        IsOccupied = isOccupied;
        UnitType = unitType;
        X = x; 
        Y = y; 
        Owner = owner; 
    }

    public Ai_Vision_Tile()
    {

    }
   
}
