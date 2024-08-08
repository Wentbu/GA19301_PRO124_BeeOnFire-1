using UnityEngine;

public class ItemCollectible : MonoBehaviour
{
    [SerializeField] public Item item;  // Reference to the item scriptable object
    [SerializeField] public int quantity = 1;  // Quantity of the item

    [SerializeField] private InventoryManager inventoryManager;

    private void Start()
    {
        // Find the InventoryManager in the scene (assuming there's only one)
        inventoryManager = FindObjectOfType<InventoryManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Player"))
        {

            if (inventoryManager != null)
            {
                bool wasAdded = inventoryManager.AddItem(item, quantity);
                if (wasAdded)
                {
                    Destroy(gameObject);  // Remove the item from the scene
                }
            }
        }
    }
}
