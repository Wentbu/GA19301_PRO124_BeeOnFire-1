using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering.Universal;

public class VisionReductionItemManager : MonoBehaviour
{
    private static VisionReductionItemManager instance;
    public static VisionReductionItemManager Instance { get { return instance; } }

    public float reductionDuration = 1f;           // Thời gian giảm tầm nhìn
    public float restorationDuration = 1f;         // Thời gian để ánh sáng tăng trở lại

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void ReduceLightGradually(Light2D playerLight, float reducedInnerRadius, float reducedOuterRadius)
    {
        StartCoroutine(ReduceLightCoroutine(playerLight, reducedInnerRadius, reducedOuterRadius));
    }

    private IEnumerator ReduceLightCoroutine(Light2D playerLight, float reducedInnerRadius, float reducedOuterRadius)
    {
        // Lưu giá trị ban đầu của inner và outer radius
        float originalInnerRadius = playerLight.pointLightInnerRadius;
        float originalOuterRadius = playerLight.pointLightOuterRadius;

        // Giảm inner và outer radius của ánh sáng từ từ
        float elapsedTime = 0f;
        while (elapsedTime < reductionDuration)
        {
            elapsedTime += Time.deltaTime;
            playerLight.pointLightInnerRadius = Mathf.Lerp(originalInnerRadius, reducedInnerRadius, elapsedTime / reductionDuration);
            playerLight.pointLightOuterRadius = Mathf.Lerp(originalOuterRadius, reducedOuterRadius, elapsedTime / reductionDuration);
            yield return null;
        }

        // Đảm bảo rằng giá trị cuối cùng là giá trị giảm tối đa
        playerLight.pointLightInnerRadius = reducedInnerRadius;
        playerLight.pointLightOuterRadius = reducedOuterRadius;

        // Sau khi giảm xong, gọi coroutine để tăng lại ánh sáng
        StartCoroutine(RestoreLightCoroutine(playerLight, originalInnerRadius, originalOuterRadius));
    }

    private IEnumerator RestoreLightCoroutine(Light2D playerLight, float originalInnerRadius, float originalOuterRadius)
    {
        // Chờ trong thời gian giảm tầm nhìn
        yield return new WaitForSeconds(restorationDuration);

        // Tăng dần inner và outer radius của ánh sáng trở lại giá trị ban đầu
        float elapsedTime = 0f;
        while (elapsedTime < restorationDuration)
        {
            elapsedTime += Time.deltaTime;
            playerLight.pointLightInnerRadius = Mathf.Lerp(playerLight.pointLightInnerRadius, originalInnerRadius, elapsedTime / restorationDuration);
            playerLight.pointLightOuterRadius = Mathf.Lerp(playerLight.pointLightOuterRadius, originalOuterRadius, elapsedTime / restorationDuration);
            yield return null;
        }

        // Đảm bảo rằng giá trị cuối cùng là giá trị ban đầu
        playerLight.pointLightInnerRadius = originalInnerRadius;
        playerLight.pointLightOuterRadius = originalOuterRadius;
    }
}