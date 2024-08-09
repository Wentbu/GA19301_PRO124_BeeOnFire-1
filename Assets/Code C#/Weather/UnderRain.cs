using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UnderRain : MonoBehaviour
{
    [SerializeField] private float inRainDuration = 10.0f; // Thời gian cần thiết để bị bệnh khi đứng trong mưa
    private float timeInRain = 0.0f;
    private bool isInRain = false;
    public bool isSick = false;
    private bool hasUmbrella = false;  // Kiểm tra trạng thái có dù
    private Coroutine checkRainEffectCoroutine;// tham chiếu đến coroutine CheckRainEffect
    public Image uiFill;
    public GameObject sickIcon;
    public GameObject underRainBar;
    public PlayerControl playerControl;
    public PlayerHealth ReduceHT;

    void Awake()
    {
        uiFill.fillAmount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // Hiển thị thanh tiến trình khi đứng trong mưa và chưa bị bệnh, không có dù
        if (isInRain == true && isSick == false && hasUmbrella == false)
        {
            underRainBar.SetActive(true);
        }
        if (isInRain == false && isSick == false)//add
        {
            timeInRain -= Time.deltaTime;
            uiFill.fillAmount = timeInRain / 10;
            if (timeInRain <= 0)
            {
                timeInRain = 0;
                underRainBar.SetActive(false);
            }
        }
        if (isSick == true)
        {

            underRainBar.SetActive(false);
            timeInRain = 0;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player") // Đảm bảo rằng đối tượng là nhân vật chính
        {
            isInRain = true;
            // Chỉ bắt đầu kiểm tra hiệu ứng mưa nếu không có dù và chưa bị bệnh
            if (hasUmbrella == false && isSick == false)
            {
                checkRainEffectCoroutine = StartCoroutine(CheckRainEffect());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            isInRain = false;

        }
    }
    //Coroutine kiểm tra thời gian nhân vật đứng trong mưa
    private IEnumerator CheckRainEffect()
    {
        while (isInRain)
        {
            timeInRain += Time.deltaTime;
            uiFill.fillAmount = timeInRain / 10;
            if (timeInRain >= inRainDuration)
            {
                StartCoroutine(ApplySicknessEffect());
                timeInRain = 0.0f; // Đặt lại thời gian sau khi áp dụng hiệu ứng bệnh
            }
            yield return null;
        }
    }

    //Áp dụng hiệu ứng bệnh cho nhân vật
    private IEnumerator ApplySicknessEffect()
    {
        isSick = true;
        sickIcon.SetActive(true);
        ReduceHT.ApplyDamage();
        playerControl.ApplySick();
        yield return new WaitForSeconds(8f);
        StartCoroutine(BlinkIcon());
        yield return new WaitForSeconds(2f);
        isSick = false;
        sickIcon.SetActive(false);
    }
    private IEnumerator BlinkIcon()
    {
        float endTime = Time.time + 2;
        while (Time.time < endTime)
        {
            sickIcon.SetActive(!sickIcon.activeSelf); // Chuyển đổi trạng thái hiện/ẩn
            yield return new WaitForSeconds(0.2f); // Thời gian nhấp nháy
        }
        sickIcon.SetActive(false); // Ẩn icon khi kết thúc
    }

    //Khi player nhặt được dù
    public void SetUmbrellaState(bool state)
    {
        hasUmbrella = state;
        if (state)
        {
            if (isSick == false)
            {
                StopAllCoroutines();
                underRainBar.SetActive(true);

            }
            if (isSick == true)
            {
                if (checkRainEffectCoroutine != null)//add
                {
                    StopCoroutine(checkRainEffectCoroutine);
                }
            }

        }
        else if (isInRain = true && isSick == false)
        {
            checkRainEffectCoroutine = StartCoroutine(CheckRainEffect());
        }

    }
}

