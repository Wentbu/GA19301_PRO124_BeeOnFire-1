using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] private Image UiFill;
    [SerializeField] private Text UiText;

    public int Duration;
    private int remainingDuration;

    private bool isRunning;

    // Start is called before the first frame update
    void Start()
    {
        UiText.gameObject.SetActive(false);
        UiFill.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !isRunning)
        {
            Begin(Duration);
        }
    }

    private void Begin(int seconds)
    {
        remainingDuration = seconds;
        if (!isRunning)
        {
            UiText.gameObject.SetActive(true);
            UiFill.gameObject.SetActive(true);
            StartCoroutine(UpdateTimer());
        }
    }

    private IEnumerator UpdateTimer()
    {
        isRunning = true;
        while (remainingDuration >= 0)
        {
            UiText.text = $"{remainingDuration:00}";
            UiFill.fillAmount = Mathf.InverseLerp(0, Duration, remainingDuration);
            remainingDuration--;
            yield return new WaitForSeconds(1f);
        }
        OnEnd();
        isRunning = false;
    }

    private void OnEnd()
    {
        print("End");
        UiText.gameObject.SetActive(false);
        UiFill.gameObject.SetActive(false);
    }
}
