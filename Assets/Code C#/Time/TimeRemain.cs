using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeRemain : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI DemTG;
    [SerializeField] private float TGConLai_Giay;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        TGConLai_Giay -= Time.deltaTime;
        int Phut = Mathf.FloorToInt(TGConLai_Giay / 60);
        int Giay = Mathf.FloorToInt(TGConLai_Giay % 60);
        DemTG.text = string.Format("{0:00}:{1:00}", Phut, Giay);

        if (TGConLai_Giay <= 0f)
        {
            SceneManager.LoadScene("TimeOutEnd");
        }
    }
}
