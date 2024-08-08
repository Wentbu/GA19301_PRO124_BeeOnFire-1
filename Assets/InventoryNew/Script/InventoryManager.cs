using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] public static InventoryManager Instance { get; private set; }

    [SerializeField] public int maxStackAmount = 5;
    [SerializeField] public GameObject inventory;
    [SerializeField] public InventorySlot[] inventorySlots;
    [SerializeField] public GameObject inventoryPanel;
    [SerializeField] public GameObject inventoryButton;
    [SerializeField] public GameObject inventoryItemPrefab;
    [SerializeField] public int slotAmount;
    int selectedSlot = -1;

    bool inventoryVisible;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ChangeSelectedSlot(0);
        inventoryButton.SetActive(inventoryVisible);
        inventoryPanel.SetActive(!inventoryVisible);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
            inventoryButton.SetActive(!inventoryButton);
        }
        if (Input.inputString != null)
        {
            bool isNumber = int.TryParse(Input.inputString, out int number);
            if (isNumber && number > 0 && number < slotAmount)
            {
                ChangeSelectedSlot(number - 1);
            }
        }
    }

    void ChangeSelectedSlot(int newValue)
    {
        if (selectedSlot >= 0)
        {
            inventorySlots[selectedSlot].DeSelect();
        }
        inventorySlots[newValue].Select();
        selectedSlot = newValue;
    }
    public void ToggleInventory()
    {
        inventoryVisible = !inventoryVisible;
        inventoryButton.SetActive(inventoryVisible);
        inventoryPanel.SetActive(!inventoryVisible); 
    }
    public Item GetSelectedItem(bool use)
    {
        InventorySlot slot = inventorySlots[selectedSlot];
        InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
        if (itemInSlot != null)
        {
            Item item = itemInSlot.item;
            if (use)
            {
                itemInSlot.count--;
                if (itemInSlot.count <= 0)
                {
                    Destroy(itemInSlot.gameObject);
                }
                else
                {
                    itemInSlot.RefreshCount();
                }
            }
            return item;
        }
        return null;
    }
    
    public void UseSelectedItem()
    {
        InventorySlot slot = inventorySlots[selectedSlot];
        InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();

        if (itemInSlot != null)
        {
            PlayerControl playerControl = FindObjectOfType<PlayerControl>();
            itemInSlot.item.Use(playerControl);

            itemInSlot.count--;
            if (itemInSlot.count <= 0)
            {
                Destroy(itemInSlot.gameObject);
            }
            else
            {
                itemInSlot.RefreshCount();
            }
        }
    }

    public bool AddItem(Item item, int quantity = 1)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null &&
                itemInSlot.item == item &&
                itemInSlot.count < maxStackAmount &&
                itemInSlot.item.stackable)
            {
                itemInSlot.count += quantity;
                itemInSlot.RefreshCount();
                return true;
            }
        }

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot == null)
            {
                SpawnNewItem(item, slot, quantity);
                return true;
            }
        }
        return false;
    }

    public void SpawnNewItem(Item item, InventorySlot slot, int quantity = 1)
    {
        GameObject newItemGo = Instantiate(inventoryItemPrefab, slot.transform);
        InventoryItem inventoryItem = newItemGo.GetComponent<InventoryItem>();
        inventoryItem.InitialiseItem(item);
        inventoryItem.count = quantity;
        inventoryItem.RefreshCount();
    }

    public bool HasKey(string keyCode)
    {
        foreach (var slot in inventorySlots)
        {
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null && itemInSlot.item is Key key && key.keyCode == keyCode)
            {
                return true;
            }
        }
        return false;
    }

    public void AddItem(Key key)
    {
        AddItem((Item)key);
    }
}
