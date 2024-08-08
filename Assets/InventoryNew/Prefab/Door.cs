using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] public string doorCode;
    [SerializeField] public bool canPassThrough;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerControl player = other.GetComponent<PlayerControl>();
            if (player != null)
            {
                player.nearbyDoor = this;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerControl player = other.GetComponent<PlayerControl>();
            if (player != null)
            {
                player.nearbyDoor = null;
            }
        }
    }

    public void TryOpenDoor()
    {
        Item selectedItem = InventoryManager.Instance.GetSelectedItem(false);
        if (selectedItem is Key key && key.keyCode == doorCode)
        {
            canPassThrough = true;
            InventoryManager.Instance.UseSelectedItem();
            Destroy(gameObject);
            Debug.Log("Door opened!");
        }
        else
        {
            canPassThrough = false;
            Debug.Log("You need the correct key to open this door.");
        }
    }
}
