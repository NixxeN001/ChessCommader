using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] General_UI_System UI_system;


    [SerializeField] private byte pawnsPerPlayer;
    [SerializeField] private GameObject pawnPrefab;
    [SerializeField] private GameObject commanderPrefab;
    [SerializeField] private GameObject ObjectSelectedMsg;

    private GridManager gridManager;
    public static GameManager instance;

    [HideInInspector] public IPawnable currentFocus;

    public List<Pawn>[] pawnsInPlay;

    public byte currentPlayer = 1;

    private Commander[] commanders;
    public Commander[] Commanders => commanders;

    public byte GetOwneronTile(Tile tile)
    {
        foreach (Pawn pm in pawnsInPlay[0])
        {
            if (pm.CurrentTile == tile)
            {
                return pm.Owner;
            }
        }

        foreach (Pawn pm in pawnsInPlay[1])
        {
            if (pm.CurrentTile == tile)
            {
                return pm.Owner;
            }
        }

        return byte.MaxValue;
    }

    public Pawn GetPawnAtCoords(int x, int y)
    {

        for (int i = 0; i < pawnsInPlay.Length; i++)
        {
            foreach (Pawn pawn in pawnsInPlay[i])
            {
                if (pawn.CurrentTile.X==x && pawn.CurrentTile.Y ==y)
                {
                    return pawn;
                }
            }
        }

        return null;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }


        UI_system?.onTurnEndRequest.AddListener(EndTurn);
    }

    public void ShowSelectedMsg()
    {
        ObjectSelectedMsg.SetActive(true);
    }

    public void HideSelectedMsg()
    {
        ObjectSelectedMsg.SetActive(false);
    }
    public void Start()
    {
        gridManager = GridManager.instance;
        GridManager.instance.OnGenerated.AddListener(Init);
        HideSelectedMsg();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EndTurn();
        }
    }

    /// <summary>
    /// spawn in pawns per player
    /// </summary>
    private void Init()
    {
        pawnsInPlay = new List<Pawn>[2];
        pawnsInPlay[0] = new List<Pawn>();
        pawnsInPlay[1] = new List<Pawn>();
        commanders = new Commander[2];

        for (int p = 1; p <= 2; p++)
        {
            for (int pw = 1; pw <= pawnsPerPlayer; pw++)
            {
                GameObject obj = Instantiate(pawnPrefab);
                Pawn pawn = obj.GetComponent<Pawn>();


                pawn.CurrentTile = gridManager.RandomTile(true);
                pawn.Owner = (byte)p;
                pawn.OnPawnDeath.AddListener(RemovePawn);
                pawnsInPlay[p - 1].Add(pawn);



            }
        }
        GameObject c1 = Instantiate(commanderPrefab);
        Commander tpm1 = c1.GetComponent<Commander>();
        tpm1.CurrentTile = gridManager.RandomTile(true);
        tpm1.Owner = 1;
        tpm1.OnCommaderDeath.AddListener(EndGame);
        commanders[0] = tpm1;



        GameObject c2 = Instantiate(commanderPrefab);
        tpm1 = c2.GetComponent<Commander>();
        tpm1.CurrentTile = gridManager.RandomTile(true);
        tpm1.Owner = 2;
        tpm1.OnCommaderDeath.AddListener(EndGame);
        commanders[1] = tpm1;

        commanders[0].MoveDistance = 1;
        commanders[1].MoveDistance = 1;
        EndTurn();
    }

    public void RemovePawn(Pawn pawn)
    {
        pawnsInPlay[pawn.Owner - 1].Remove(pawn);
        Destroy(pawn.gameObject);

        if (pawnsInPlay[0].Count == 0 || pawnsInPlay[1].Count == 0)
        {
            Debug.Log("GAME OVER!");
        }
    }

    public void EndGame(Commander loser)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void CheckAIWin()
    {
        foreach (Pawn item in pawnsInPlay[1])
        {
            if (item.CurrentTile == commanders[0].CurrentTile)
            {
                EndGame(Commanders[0]);
            }
        }
    }

    public async void EndTurn()
    {

        try
        {
            currentPlayer = currentPlayer == 1 ? (byte)2 : (byte)1;
            currentFocus = null;

            foreach (Pawn po in pawnsInPlay[currentPlayer - 1])
            {
                po.MoveDistance = 1;
                po.AttacksLeft = 1;
            }

            commanders[currentPlayer - 1].AttacksLeft = 0;


            UI_system?.onTurnChange.Invoke(currentPlayer);

            if (Game_Settings.instance.PvCPU)
            {
                await Ai_Vision_Factory.instance.RegenVision(gridManager.TileArray);
                await AI_State_Manager.instance.TestingLoop();
            }
           
        }
        catch 
        {

           
        }
       

    }

    public Tuple<int, int> GetIPawnableOnTile(int x, int y)
    {
        for (int i = 0; i < pawnsInPlay.Length; i++)
        {
            for (int j = 0; j < pawnsInPlay[i].Count; j++)
            {
                if (pawnsInPlay[i][j].CurrentTile.X == x && pawnsInPlay[i][j].CurrentTile.Y == y)
                {
                    return new Tuple<int, int>(i, j);
                }
            }
        }
        /*foreach (var pawn in pawnsInPlay)
        {
            foreach (var p in pawn)
            {
                if (p.CurrentTile.X == x && p.CurrentTile.Y == y)
                {
                    return p;
                }
            }
        }*/

        return null;
    }
}
