using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRuleManager : MonoBehaviour
{
    [SerializeField] GameObject InfoGO;
    // Start is called before the first frame update
    void Start()
    {
        InfoGO.SetActive(false);
    }

   public void ToggleInfo()
    {
        InfoGO.SetActive(!InfoGO.active);
    }
}
