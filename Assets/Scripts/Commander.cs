using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Commander : MonoBehaviour, IOwnable, IDestination, IMoveable, IPawnable
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
    public int AttacksLeft { get; set; }

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

    public UnityEvent<Commander> OnCommaderDeath = new UnityEvent<Commander>();

    public void Attack()
    {
        OnCommaderDeath.Invoke(this);
    }

    public List<Tile> GetAvailableMoves()
    {
        if (MoveDistance<=0)
        {
            return new List<Tile>();
        }

        List<Tile> tiles =  new List<Tile>();
        Debug.Log($"Tile Count = {GridManager.instance.TileArray.Length}");
        foreach (Tile item in GridManager.instance.TileArray)
        {
            if (!item.IsOccupied && item.IsWalkable())
            {
                tiles.Add(item);
            }
        }

        return tiles;
    }

    public void HandleLeftClick()
    {
        Debug.Log("Commader Selected");

        if (GameManager.instance.currentPlayer == Owner)
        {
            GameManager.instance.currentFocus = this;

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

    public void MoveTo(Tile destination)
    {
        this.transform.position = destination.WorldPos;
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
}
