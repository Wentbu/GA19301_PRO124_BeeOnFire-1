using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemValueList", menuName = "ScriptableObjects/ItemValueList")]
public class ItemValueList : ScriptableObject
{
    [System.Serializable]
    public struct itemValueList
    {
        public int Items_Id;
        public string Items_Name;
        public string Feature;
        public double Duration;
    }

    public List<itemValueList> Itemp;
}
