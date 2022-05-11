using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Pawn : MonoBehaviour, IPawnable, IOwnable, IMoveable, IDestination
{
    private byte owner;

    public byte Owner
    {
        get { return owner; }
        set
        {
            owner = value;
            UpdateGraphics();
        }
    }

    public int MoveDistance { get; set; }

    private Tile current;

    public Tile CurrentTile
    {
        get
        {
            return current;
        }

        set
        {

            if (current != null)
            {
                current.IsOccupied = false;
                MoveDistance--;
            }

            current = value;

            current.IsOccupied = true;
            MoveTo(current);
        }
    }

    public int AttacksLeft { get ; set ; }


    /// <summary>
    /// Returns a List of Tiles that the current Pawn can move to, the space is not Occupied
    /// </summary>
    /// <returns></returns>
    public List<Tile> GetAvailableMoves()
    {
        if (MoveDistance <= 0)
        {
           
            return new List<Tile>();
        }

        List<Tile> availableMoves = new List<Tile>();

        foreach (Tile u in CurrentTile.SurroundingTiles())
        {
            if (u.IsWalkable())
            {
                availableMoves.Add(u);
            }

        }
        return availableMoves;

    }

    /// <summary>
    /// Changes the transform of the pawn to the selected Tile destination
    /// </summary>
    /// <param name="destination"></param>
    public void MoveTo(Tile destination)
    {

        this.transform.position = destination.WorldPos;
    }


    public UnityEvent<Pawn> OnPawnDeath = new UnityEvent<Pawn>();
    private void UpdateGraphics()
    {
        if (Owner == 1)
        {
            GetComponent<SpriteRenderer>().color = Color.blue;
        }
        else if (Owner == 2)
        {
            GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    public void Attack()
    {
        OnPawnDeath.Invoke(this);

    }

    public void HandleLeftClick()
    {
        if (GameManager.instance.currentPlayer == Owner)
        {
            GameManager.instance.currentFocus = this;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            HandleRightClick();
        }

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            HandleLeftClick();
        }
    }

    public void HandleRightClick()
    {
        if (GameManager.instance.currentFocus == this || GameManager.instance.currentPlayer == Owner)
        {
            return;
        }

        if (GameManager.instance.currentFocus.GetAvailableMoves()
            .Contains(GridManager.instance.GetTileFromCoord(transform.position)))
        {
            GameManager.instance.currentFocus.AttacksLeft--;
            Attack();
        }
    }
}
