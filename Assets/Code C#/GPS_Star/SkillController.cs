using System.Collections;
using UnityEngine;

public class SkillActivator : MonoBehaviour
{
    public GameObject expanderPrefab; // Prefab cho NewBehaviourScript
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
        // Tạo instance của prefab expander và gắn nó theo player
        GameObject expander = Instantiate(expanderPrefab, transform.position, Quaternion.identity);
        expander.transform.SetParent(transform);
        expander.SetActive(true);
        Debug.Log("Tạo và kích hoạt NewBehaviourScript: " + expander.name);

        // Tạo instance của prefab AiController
        GameObject aiController = Instantiate(aiControllerPrefab, transform.position, Quaternion.identity);
        aiController.SetActive(true);
        Debug.Log("Tạo và kích hoạt AiController: " + aiController.name);

        // Gán AiController cho NewBehaviourScript nếu cần
        NewBehaviourScript newBehaviourScript = expander.GetComponent<NewBehaviourScript>();
        AiController aiControllerComponent = aiController.GetComponent<AiController>();
        if (newBehaviourScript != null && aiControllerComponent != null)
        {
            newBehaviourScript.SetAiController(aiControllerComponent);
            Debug.Log("Gán AiController cho NewBehaviourScript");
        }
        else
        {
            Debug.LogError("Không tìm thấy NewBehaviourScript hoặc AiController");
        }

        // Hủy expander và aiController sau 10 giây
        Destroy(expander, 10.0f);
        Destroy(aiController, 10.0f);

        // Bắt đầu đếm ngược thời gian hồi skill
        Debug.Log("Bắt đầu đếm thời gian");
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
