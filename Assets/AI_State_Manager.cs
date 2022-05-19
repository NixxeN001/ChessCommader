using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleNodes;


public class AI_State_Manager : MonoBehaviour
{
    public Node<Tile[,]> currentState;
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

    }

    public async void CurrentStateToNode(Tile[,] tiles)
    {
        currentState = new Node<Tile[,]>(tiles);
       await currentState.AppendCurrentNodeStateToDiskFileAsync() ;
        

    }

}
