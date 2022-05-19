using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Logic : MonoBehaviour
{
    [SerializeField] General_UI_System UI_system;
    [SerializeField] TMP_Text turnIDtext;
    [Space]
    [SerializeField] Button endTurnBtn;
    




    private void Awake()
    {
        UI_system?.onTurnChange.AddListener(UpdateTurnDisplay);
        endTurnBtn.onClick.AddListener(EndTurnRequest);
    }

    private void UpdateTurnDisplay(byte playerID)
    {
        turnIDtext.text = playerID.ToString();
    }

    private void EndTurnRequest()
    {
        UI_system?.onTurnEndRequest.Invoke();
    }
}
