using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoSpawning : MonoBehaviour
{
    // Start is called before the first frame update
    public InventoryManager inventoryManager;
    public Item[] itemToPickup;

    public void PickupItem(int id)
    {
        Debug.Log("add");
        bool result = inventoryManager.AddItem(itemToPickup[id]);
        if (result == true)
        {
            Debug.Log("Item added");
        }
        else
        {
            Debug.Log("Item Not Added");
        }
    }
    public void GetSelectedItem()
    {
        Item recievedItem = inventoryManager.GetSelectedItem(false);
        if (recievedItem != null)
        {
            Debug.Log("Item Recieved In Slot");
        }
        else
        {
            Debug.Log("No Item Recieved In Slot");
        }
    }
    public void UseSelectedItem()
    {
        Item recievedItem = inventoryManager.GetSelectedItem(true);
        if (recievedItem != null)
        {
            Debug.Log("Used Item Recieved In Slot");
        }
        else
        {
            Debug.Log("No Item Recieved In Slot");
        }
    }
}
