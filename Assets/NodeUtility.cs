using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NodeUtility : MonoBehaviour
{
    public static string ROOT_NODES_PATH;

    private static ulong highestID = 0;
    public static ulong GetUniqueNodeID()
    {
        highestID++;
        return highestID;
    }

    [SerializeField] private bool readFromDiskOnLoad;

    public static List<string> LastDiskRead = new List<string>();

    private void Awake()
    {
        ROOT_NODES_PATH = Application.dataPath + "/Nodes";
        Debug.Log($"Base node export path : {ROOT_NODES_PATH}");
        if (!Directory.Exists(ROOT_NODES_PATH))
        {
            Directory.CreateDirectory(ROOT_NODES_PATH);
        }

        if (!readFromDiskOnLoad) return;

        string[] files = Directory.GetFiles(ROOT_NODES_PATH, "*Node.json", SearchOption.TopDirectoryOnly);
        foreach (string file in files)
        {
            LastDiskRead.Add(String.Concat(File.ReadAllLines(file)));
            highestID++;
        }
    }
}