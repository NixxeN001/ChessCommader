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

    private GridManager gridManager;
    public static GameManager instance;

    [HideInInspector] public IPawnable currentFocus;

    List<Pawn>[] pawnsInPlay;

    public byte currentPlayer = 1;

    private Commander[] commanders;

    public byte GetOwneronTile(Tile tile)
    {
        foreach (Pawn pm in pawnsInPlay[0])
        {
            if (pm.CurrentTile==tile)
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

        throw new System.Exception();
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }


        UI_system?.onTurnEndRequest.AddListener(EndTurn);
    }

    public void Start()
    {
        gridManager = GridManager.instance;
        GridManager.instance.OnGenerated.AddListener(Init);

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
        tpm1.CurrentTile= gridManager.RandomTile(true);
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

    public void EndTurn()
    {


        currentPlayer = currentPlayer == 1 ? (byte)2 : (byte)1;
        currentFocus = null;

        foreach (Pawn po in pawnsInPlay[currentPlayer-1])
        {
            po.MoveDistance = 1;
            po.AttacksLeft = 1;
        }

        commanders[currentPlayer-1].AttacksLeft = 0;


        UI_system?.onTurnChange.Invoke(currentPlayer);
        AI_State_Manager.instance.CurrentStateToNode(GridManager.instance.TileArray);

    }
}
