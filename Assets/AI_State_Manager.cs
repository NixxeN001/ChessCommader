using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleNodes;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using Random = UnityEngine.Random;

public class AI_State_Manager : MonoBehaviour
{
    [HideInInspector] public Ai_Vision_Factory factory;
    public Ai_Vision_Tile[,] currentFocus;
    public static AI_State_Manager instance;

    private const float COMMANDER_MAX_WEIGHT = 0.5f;
    const float ENEMY_MAXDIST_WEIGHT = 0.4f;

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
    public async Task TestingLoop()
    {
        await Task.Delay(500);
        int idx = 0;
        Tuple<int, int> retn = factory.GetNextPawn(factory.vision_field, idx);

        while (retn != null)
        {
            Ai_Choice choice = await GetBestMoveForPawn(retn.Item1, retn.Item2, factory.vision_field);
            await DeclareMove(choice, retn.Item1, retn.Item2);
            //Debug.Log(retn.Item1 + " || " + retn.Item2);
            idx++;
            retn = factory.GetNextPawn(factory.vision_field, idx);
            await Task.Delay(1);


        }

        GameManager.instance.CheckAIWin();

    }


    /*Method that gets called recursivly that decides which direction the pawn needs to go to.
     * its options 
     * 
     * 
     * 
     * 
     */
    private async Task<Ai_Choice> GetBestMoveForPawn(int x, int y, Ai_Vision_Tile[,] map)
    {
        // Debug.Log($"Get weights for Pawn {x}:{y}");

        //left, right, down, up
        var HeuristicNodes = new Node<Ai_Vision_Tile[,]>[4];

        for (int i = 0; i < 4; i++)
        {
            HeuristicNodes[i] = new Node<Ai_Vision_Tile[,]>(map, x, y);
        }


        //Prediction loop
        for (int i = 0; i < 5; i++)
        {

            if (x > 0)
            {
                var tempRef = HeuristicNodes[0];
                HeuristicNodes[0] = new Node<Ai_Vision_Tile[,]>(ShiftUnitOnMap(x, y, x - 1, y, map),
                    HeuristicNodes[0].X - 1, HeuristicNodes[0].Y);

                HeuristicNodes[0].weight = await getBoardWeightForUnit(HeuristicNodes[0].X - 1, HeuristicNodes[0].Y, HeuristicNodes[0].Data);
                HeuristicNodes[0].In.Add(tempRef);


            }

            if (x < map.GetLength(0) - 1)
            {
                var tempRef = HeuristicNodes[1];
                HeuristicNodes[1] = new Node<Ai_Vision_Tile[,]>(ShiftUnitOnMap(x, y, x + 1, y, map),
                    HeuristicNodes[1].X + 1, HeuristicNodes[1].Y);
                HeuristicNodes[1].weight = await getBoardWeightForUnit(HeuristicNodes[1].X + 1, HeuristicNodes[1].Y, HeuristicNodes[1].Data);
                HeuristicNodes[1].In.Add(tempRef);


            }

            if (y > 0)
            {
                var tempRef = HeuristicNodes[2];
                HeuristicNodes[2] = new Node<Ai_Vision_Tile[,]>(ShiftUnitOnMap(x, y, x, y - 1, map),
                    HeuristicNodes[2].X, HeuristicNodes[2].Y - 1);
                HeuristicNodes[2].weight = await getBoardWeightForUnit(HeuristicNodes[2].X, HeuristicNodes[2].Y - 1, HeuristicNodes[2].Data);
                HeuristicNodes[2].In.Add(tempRef);


            }

            if (y < map.GetLength(0) - 1)
            {
                var tempRef = HeuristicNodes[3];
                HeuristicNodes[3] = new Node<Ai_Vision_Tile[,]>(ShiftUnitOnMap(x, y, x, y + 1, map),
                    HeuristicNodes[3].X, HeuristicNodes[3].Y + 1);
                HeuristicNodes[3].weight = await getBoardWeightForUnit(HeuristicNodes[3].X, HeuristicNodes[3].Y + 1, HeuristicNodes[3].Data);
                HeuristicNodes[3].In.Add(tempRef);


            }

        }

        /*Add up all weights from loop above 
        *This is what determines the best 

        */


        int bestChoice = -1;
        float bestW = 0;
        for (int i = 0; i < 4; i++)
        {
            float tempW = 0;
            Node<Ai_Vision_Tile[,]> tempRef = HeuristicNodes[i];
            for (int j = 0; j < 3; j++)
            {
                if (tempRef.In.Count > 0)
                {
                    tempRef = tempRef.In[0];
                    tempW += tempRef.weight;
                }
                if (tempW > bestW)
                {
                    bestChoice = i;
                    bestW = tempW;
                }
            }
        }



        switch (bestChoice)
        {
            case 0:
                return Ai_Choice.left;
            case 1:
                return Ai_Choice.right;
            case 2:
                return Ai_Choice.down;
            case 3:
                return Ai_Choice.up;
            default:
                return default(Ai_Choice);
        }

    }

    private Ai_Vision_Tile[,] ShiftUnitOnMap(int cX, int cY, int tX, int tY, Ai_Vision_Tile[,] shiftMap)
    {
        shiftMap[cX, cY].IsOccupied = false;
        shiftMap[tX, tY].IsOccupied = true;

        shiftMap[tX, tY].UnitType = shiftMap[cX, cY].UnitType;
        shiftMap[cX, cY].UnitType = 0;

        shiftMap[tX, tY].Owner = shiftMap[cX, cY].Owner;
        shiftMap[cX, cY].Owner = byte.MaxValue;



        return shiftMap;
    }


    /*
     * 
     * 
     * 
     * 
     * 
     */

    public async Task<float> getBoardWeightForUnit(int x, int y, Ai_Vision_Tile[,] tilemap)
    {
        int width = tilemap.GetLength(0), height = tilemap.GetLength(1);
        float maxPossibleMoveDist = (width - 1) + (height - 1);

        float weight = 0f;

        Tuple<int, int, int> commanderRequest = factory.GetEnemyCommanderDist(x, y, tilemap);

        //Calculates Dist from Commader within maxweight of 1
        float Cweight =
            Mathf.Clamp(COMMANDER_MAX_WEIGHT - ((COMMANDER_MAX_WEIGHT / maxPossibleMoveDist)
            * commanderRequest.Item3), 0, COMMANDER_MAX_WEIGHT);



        Tuple<int, int, int> closestEnemyTile = await factory.GetClosestEnemyPawn(x, y, tilemap);
        bool eatsPawn = closestEnemyTile.Item3 == 0;
        if (closestEnemyTile.Item3 == 0)
        {
            closestEnemyTile = await factory.GetClosestEnemyPawn(x, y, tilemap, false);

        }

        float eatWeight = eatsPawn ? 0.1f : 0f;


        float Eweight = closestEnemyTile.Item3 == 1 ?
                        0 :
                        closestEnemyTile.Item3 % 2 == 0 ?
                        ENEMY_MAXDIST_WEIGHT : 0;

        weight = Cweight + Eweight + Eweight;

        return weight;
    }


    /*
     * 
     * 
     *  
     * 
     */

    private async Task DeclareMove(Ai_Choice _choice, int x, int y)
    {

        Tile target;
        switch (_choice)
        {
            case Ai_Choice.right:
                target = GridManager.instance.TileArray[x + 1, y];
                break;
            case Ai_Choice.left:
                target = GridManager.instance.TileArray[x - 1, y];
                break;
            case Ai_Choice.up:
                target = GridManager.instance.TileArray[x, y + 1];
                break;
            case Ai_Choice.down:
                target = GridManager.instance.TileArray[x, y - 1];
                break;
            default:
                target = GridManager.instance.TileArray[x, y];
                Debug.Log("Default");
                break;

        }
        //
        GameManager.instance.currentFocus = GameManager.instance.pawnsInPlay[GameManager.instance.GetIPawnableOnTile(x, y).Item1]
            [GameManager.instance.GetIPawnableOnTile(x, y).Item2];


        //
        if (GameManager.instance.currentFocus.GetAvailableMoves().Contains(target))
        {
            GameManager.instance.currentFocus.CurrentTile = target;
        }

        await factory.RegenVision(GridManager.instance.TileArray, false);
        var tmp = await factory.GetClosestEnemyPawn(target.X, target.Y, factory.latestGen.Data);
        if (tmp.Item3 == 0)
        {
            GameManager.instance.RemovePawn(GameManager.instance.GetPawnAtCoords(tmp.Item1, tmp.Item2));
            await factory.RegenVision(GridManager.instance.TileArray, false);
        }

    }

    public enum Ai_Choice
    {
        right,
        left,
        up,
        down
    }
}
