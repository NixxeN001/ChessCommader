using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Default GUI Events", menuName = "Custom Events/General GUI")]
public class General_UI_System : ScriptableObject
{
    [HideInInspector] public UnityEvent<byte> onTurnChange = new UnityEvent<byte>();
    [HideInInspector] public UnityEvent onTurnEndRequest = new UnityEvent();

}
