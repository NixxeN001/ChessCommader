using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleNodes;
using System;

public class AI_State_Manager : MonoBehaviour
{
    public Ai_Vision_Factory factory;
    public Ai_Vision_Tile[,] currentFocus;
    public static AI_State_Manager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        factory = Ai_Vision_Factory.instance;
    }
    public void TestingLoop()
    {
        Tuple<int, int> retn = factory.GetNextPawn();
        GetBestMoveForPawn(retn.Item1, retn.Item2, factory.vision_field);
    }

    private Ai_Vision_Tile[,] GetBestMoveForPawn(int x, int y, Ai_Vision_Tile[,] map)
    {
        /*Distance from the Commandar after the move
         * Distance from closest enemy Pawn after move
         * Distance from second closest enemy Pawn after move 
         */
        float weight = 0f;


        Tuple<int, int, int> closestEnemyTile = factory.GetClosestEnemyPawn(x, y, map);
        return null;
    }


}
