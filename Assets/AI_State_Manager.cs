using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleNodes;
using System;
using System.Threading.Tasks;

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

        //  GetBestMoveForPawn(retn.Item1, retn.Item2, factory.vision_field);

       /* Task.Run(async () =>
        {*/
            await Task.Delay(500);
            int idx = 0;
            Tuple<int, int> retn = factory.GetNextPawn(factory.vision_field, idx);

            while (retn != null)
            {
                Ai_Choice choice = GetBestMoveForPawn(retn.Item1, retn.Item2, factory.vision_field);
                await DeclareMove(choice, retn.Item1, retn.Item2);
                // Debug.Log(retn.Item1 + " || " + retn.Item2);
                idx++;
                retn = factory.GetNextPawn(factory.vision_field, idx);
                await Task.Delay(1);


            }
        //});

        // Debug.Log(GetBestMoveForPawn(retn.Item1, retn.Item2, factory.vision_field));
    }

    private Ai_Choice GetBestMoveForPawn(int x, int y, Ai_Vision_Tile[,] map)
    {
        /*Distance from the Commandar after the move
         * Distance from closest enemy Pawn after move
         * Distance from second closest enemy Pawn after move 
         */

        int width = map.GetLength(0), height = map.GetLength(1);

        float rightMoveWeight = 0;
        float leftMoveWeight = 0;
        float upMoveWeight = 0;
        float downMoveWeight = 0;

        Node<Ai_Vision_Tile[,]>[] HeuristicOutputs = null;
        Node<Ai_Vision_Tile[,]> latestGen = new Node<Ai_Vision_Tile[,]>(map);

        int genX = x, genY = y;

        for (int i = 0; i < 10; i++)
        {
            HeuristicOutputs = new Node<Ai_Vision_Tile[,]>[4];
            float[] weights = new float[] { rightMoveWeight, leftMoveWeight, upMoveWeight, downMoveWeight };
            int tempID = 0;

            #region if Stuff
            //Checks if you can move to the Right
            if (genX < width - 1)
            {

                Tuple<Ai_Vision_Tile[,], int, int> rightMove = ShiftPawnTo(genX, genY, genX + 1, genY, latestGen.Data);
                rightMoveWeight = getBoardWeightForUnit(rightMove.Item2, rightMove.Item3, latestGen.Data);
                Node<Ai_Vision_Tile[,]> temp = new Node<Ai_Vision_Tile[,]>(rightMove.Item1);
                temp.weight = rightMoveWeight;
                temp.In.Add(HeuristicOutputs[0]);
                HeuristicOutputs[0] = temp;
            }
            //Checks if you can move to the Left
            if (genX > 0)
            {
                Tuple<Ai_Vision_Tile[,], int, int> leftmove = ShiftPawnTo(genX, genY, genX - 1, genY, latestGen.Data);
                leftMoveWeight = getBoardWeightForUnit(leftmove.Item2, leftmove.Item3, latestGen.Data);
                Node<Ai_Vision_Tile[,]> temp = new Node<Ai_Vision_Tile[,]>(leftmove.Item1);
                temp.weight = leftMoveWeight;
                temp.In.Add(HeuristicOutputs[1]);
                HeuristicOutputs[1] = temp;
            }
            //Checks of you can move Up
            if (genY < height - 1)
            {
                Tuple<Ai_Vision_Tile[,], int, int> upMove = ShiftPawnTo(genX, genY, genX, genY + 1, latestGen.Data);
                upMoveWeight = getBoardWeightForUnit(upMove.Item2, upMove.Item3, latestGen.Data);
                Node<Ai_Vision_Tile[,]> temp = new Node<Ai_Vision_Tile[,]>(upMove.Item1);
                temp.weight = upMoveWeight;
                temp.In.Add(HeuristicOutputs[2]);
                HeuristicOutputs[2] = temp;
            }

            //Checks of you can move Down
            if (genY > 0)
            {
                Tuple<Ai_Vision_Tile[,], int, int> downMove = ShiftPawnTo(genX, genY, genX, genY - 1, latestGen.Data);
                downMoveWeight = getBoardWeightForUnit(downMove.Item2, downMove.Item3, latestGen.Data);
                Node<Ai_Vision_Tile[,]> temp = new Node<Ai_Vision_Tile[,]>(downMove.Item1);
                temp.weight = downMoveWeight;
                temp.In.Add(HeuristicOutputs[3]);
                HeuristicOutputs[3] = temp;
            }
            #endregion

            for (int j = 0; j < weights.Length; j++)
            {
                if (weights[j] > weights[tempID])
                {
                    tempID = j;
                }

            }
            latestGen = new Node<Ai_Vision_Tile[,]>(HeuristicOutputs[tempID]);

        }

        float highestWeightPath = 0f;
        int choiceIndex = 0;
        Node<Ai_Vision_Tile[,]> currentNode;
        for (int i = 0; i < HeuristicOutputs.Length; i++)
        {
            float tmpweightPath = 0;
            currentNode = HeuristicOutputs[i];
            while (currentNode?.In.Count > 0)
            {
                tmpweightPath += currentNode.weight;
                currentNode = HeuristicOutputs[i].In[0];
            }

            if (tmpweightPath > highestWeightPath)
            {
                highestWeightPath = tmpweightPath;
                choiceIndex = i;
            }

        }

        switch (choiceIndex)
        {
            case 0:
                return Ai_Choice.right;
            case 1:
                return Ai_Choice.left;
            case 2:
                return Ai_Choice.up;
            case 3:
                return Ai_Choice.down;

            default:
                throw new Exception();

        }


        Tuple<Ai_Vision_Tile[,], int, int> ShiftPawnTo(int x, int y, int targetX, int targetY, Ai_Vision_Tile[,] map)
        {
            if (map == null)
            {
                throw new Exception();

            }
            else if (map[targetX, targetY].IsOccupied)
            {
                return new Tuple<Ai_Vision_Tile[,], int, int>(map, x, y);
            }
            Ai_Vision_Tile[,] newMap = (Ai_Vision_Tile[,])map.Clone();

            newMap[targetX, targetY].UnitType = newMap[x, y].UnitType;
            newMap[targetX, targetY].IsOccupied = true;

            newMap[x, y].UnitType = byte.MaxValue;
            newMap[x, y].IsOccupied = false;

            return new Tuple<Ai_Vision_Tile[,], int, int>(newMap, targetX, targetY);
        }
    }


    public float getBoardWeightForUnit(int x, int y, Ai_Vision_Tile[,] tilemap)
    {
        int width = tilemap.GetLength(0), height = tilemap.GetLength(1);
        float maxPossibleMoveDist = (width - 1) + (height - 1);

        float weight = 0f;

        Tuple<int, int, int> commanderRequest = factory.GetEnemyCommanderDist(x, y, tilemap);

        //Calculates Dist from Commader within maxweight of 1
        float Cweight =
            Mathf.Clamp(COMMANDER_MAX_WEIGHT - ((COMMANDER_MAX_WEIGHT / maxPossibleMoveDist)
            * commanderRequest.Item3), 0, COMMANDER_MAX_WEIGHT);



        Tuple<int, int, int> closestEnemyTile = factory.GetClosestEnemyPawn(x, y, tilemap);
        bool eatsPawn = closestEnemyTile.Item3 == 0;
        if (closestEnemyTile.Item3 == 0)
        {
            closestEnemyTile = factory.GetClosestEnemyPawn(x, y, tilemap, false);

        }

        float eatWeight = eatsPawn ? 0.1f : 0f;


        float Eweight = closestEnemyTile.Item3 == 1 ?
                        0 :
                        closestEnemyTile.Item3 % 2 == 0 ?
                        ENEMY_MAXDIST_WEIGHT : 0;

        weight = Cweight + Eweight + Eweight;

        return weight;
    }

    //
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
        GameManager.instance.currentFocus = GameManager.instance.pawnsInPlay[GameManager.instance.GetIPawnableOnTile(x, y).Item1]
            [GameManager.instance.GetIPawnableOnTile(x, y).Item2];


        if (GameManager.instance.currentFocus.GetAvailableMoves().Contains(target))
        {
            Debug.Log($"Moved [{GameManager.instance.currentFocus.CurrentTile.X}:{GameManager.instance.currentFocus.CurrentTile.Y}] " +
                $"to [{target.X} : {target.Y}] ");
            GameManager.instance.currentFocus.CurrentTile = target;
        }

        //await factory.RegenVision(GridManager.instance.TileArray, false);
    }

    public enum Ai_Choice
    {
        right,
        left,
        up,
        down
    }
}
