using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


[CreateAssetMenu(fileName = "ItemValue", menuName = "ItemValue")]

public class ItemValue : ScriptableObject
{
    [System.Serializable]
    public struct itemValue
    {
        public int Items_Id;
        public string Items_Name;
        public string Feature;
        public double Duration;
    }

    public List<itemValue> Itemp;
}
