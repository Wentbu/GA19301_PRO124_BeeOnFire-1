using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillActivator : MonoBehaviour
{
    public GameObject expanderPrefab; // Prefab cho ColliderExpander
    public GameObject aiControllerPrefab; // Prefab cho AiController
    public KeyCode activationKey = KeyCode.Q; // Nút nhấn để kích hoạt skill

    private bool canActivate = true; // Biến kiểm tra có thể kích hoạt skill hay không
    private float cooldownDuration = 30f; // Thời gian hồi skill

    private void Update()
    {
        // Kiểm tra nút nhấn và trạng thái có thể kích hoạt
        if (Input.GetKeyDown(activationKey) && canActivate)
        {
            ActivateFunction();
            Debug.Log("Kích hoạt skill");
        }
    }

    public void ActivateFunction()
    {
        if (expanderPrefab == null || aiControllerPrefab == null)
        {
            Debug.LogError("Prefab không được chỉ định.");
            return;
        }

        // Tạo instance của prefab expander tại vị trí của player
        Vector3 playerPosition = transform.position; // Vị trí của player
        GameObject expander = Instantiate(expanderPrefab, playerPosition, Quaternion.identity);
        Debug.Log("Tạo và kích hoạt ColliderExpander tại: " + playerPosition);

        // Tạo instance của prefab AiController tại vị trí của player
        GameObject aiController = Instantiate(aiControllerPrefab, playerPosition, Quaternion.identity);
        Debug.Log("Tạo và kích hoạt AiController tại: " + playerPosition);

        // Gán AiController cho ColliderExpander
        ColliderExpander colliderExpander = expander.GetComponent<ColliderExpander>();
        AiController aiControllerComponent = aiController.GetComponent<AiController>();

        if (colliderExpander != null && aiControllerComponent != null)
        {
            colliderExpander.SetAiController(aiControllerComponent);
            Debug.Log("Gán AiController cho ColliderExpander");
        }
        else
        {
            if (colliderExpander == null)
                Debug.LogError("Không tìm thấy ColliderExpander trong prefab!");

            if (aiControllerComponent == null)
                Debug.LogError("Không tìm thấy AiController trong prefab!");
        }

        // Hủy expander và aiController sau thời gian đã định
        Destroy(expander, cooldownDuration);
        Destroy(aiController, cooldownDuration);

        // Bắt đầu đếm ngược thời gian hồi skill
        Debug.Log("Bắt đầu đếm thời gian hồi skill");
        StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        canActivate = false;
        Debug.Log("Bắt đầu đếm ngược thời gian hồi skill: " + cooldownDuration + " giây");
        yield return new WaitForSeconds(cooldownDuration);
        canActivate = true;
        Debug.Log("Hồi skill hoàn tất, có thể kích hoạt lại");
    }
}
