using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public float baseSpeed = 5.0f;

    [SerializeField] private Coroutine speedBoostCoroutine;
    [SerializeField] public int maxHealth = 100;
    [SerializeField] private int currentHealth;
    [SerializeField] private InventoryManager inventoryManager;

    [SerializeField] public float currentSpeed;
    [SerializeField] public Door nearbyDoor;
    [SerializeField] private GameObject shadowPrefab; // Prefab Shadow  

    private void Start()
    {
        inventoryManager = FindObjectOfType<InventoryManager>();
        currentSpeed = baseSpeed;
        currentHealth = maxHealth;
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.E))
        {
            InventoryManager.Instance.ToggleInventory();
        }

        if (nearbyDoor != null && Input.GetKeyDown(KeyCode.G))
        {
            nearbyDoor.TryOpenDoor();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            Item selectedItem = inventoryManager.GetSelectedItem(false);
            if (selectedItem != null)
            {
                if (!(selectedItem is Key))  // Only use items that are not KeyItem
                {
                    selectedItem.Use(this);
                    inventoryManager.GetSelectedItem(true); // Consume the item
                }
            }
        }
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(moveHorizontal, moveVertical);
        transform.Translate(movement * currentSpeed * Time.deltaTime);
    }

    public IEnumerator ApplySpeedBoost(float boostAmount, float boostDuration)
    {
        currentSpeed += boostAmount;
        yield return new WaitForSeconds(boostDuration);
        currentSpeed -= boostAmount;
    }

    private IEnumerator CreateShadows(float duration, float shadowAlpha = 0.5f, float shadowExistTime = 0.25f, float shadowSpawnInterval = 0.1f, Vector3 shadowOffset = default(Vector3))
    {
        float endTime = Time.time + duration;

        // Color Set
        Color[] shadowColors = new Color[]
        {
        Color.red,
        Color.green,
        Color.blue,
        Color.yellow,
        Color.magenta
        };

        while (Time.time < endTime)
        {
            GameObject shadow = Instantiate(shadowPrefab, transform.position + shadowOffset, transform.rotation);
            SpriteRenderer shadowSprite = shadow.GetComponent<SpriteRenderer>();
            shadowSprite.sprite = GetComponent<SpriteRenderer>().sprite;

            // Random Color generator
            Color shadowColor = shadowColors[Random.Range(0, shadowColors.Length)];
            shadowColor.a = shadowAlpha; // Tranparency
            shadowSprite.color = shadowColor;

            Destroy(shadow, shadowExistTime); // Shadow Duration

            yield return new WaitForSeconds(shadowSpawnInterval); // create shadow every one second
        }

    }
}
