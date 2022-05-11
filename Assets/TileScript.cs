using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileScript : MonoBehaviour, IDestination
{
    public void HandleRightClick()
    {
       if (GameManager.instance.currentFocus != null)
        {
            Tile current = GridManager.instance.GetTileFromCoord(transform.position);

            if (GameManager.instance.currentFocus.GetAvailableMoves().Contains(current))
            {
                GameManager.instance.currentFocus.CurrentTile = current;
            }
           
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
       if (eventData.button == PointerEventData.InputButton.Right)
        {
            HandleRightClick();
        }
    }
}
