using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegenCircle : MonoBehaviour
{

    private Image circleImg;

    private void Awake()
    {
        circleImg = GetComponent<Image>();
    }

    void Start()
    {
        circleImg.fillAmount = 0;
        PlayerHealth.Instance.OnCountDownRegen += FillDownCircle;
    }

    void Update()
    {
        
    }

    private void FillDownCircle(float timeLeftRatio) 
    {
        circleImg.fillAmount = timeLeftRatio;
    }
}
