using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegenCircle : MonoBehaviour
{

    private Image _circleImg;

    private void Awake()
    {
        _circleImg = GetComponent<Image>();
    }

    void Start()
    {
        _circleImg.fillAmount = 0;
        PlayerHealth.Instance.OnCountDownRegen += FillDownCircle;
    }

    void Update()
    {
 
    }

    private void FillDownCircle(float timeLeftRatio) 
    {
        if (timeLeftRatio <= 0 || timeLeftRatio == 1f) 
        {
            _circleImg.fillAmount = 0f;
            return;
        };

        _circleImg.fillAmount = timeLeftRatio;
    }
}
