using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ExtraModeUI : MonoBehaviour
{
    [SerializeField] private TMP_Text finishLineNotif;
    [SerializeField] private TMP_Text coreRemainsText;
    [SerializeField] private TMP_Text timerText;

    void Start()
    {
        StartCoroutine(LateStart());
    }

    IEnumerator LateStart() 
    {
        yield return new WaitForSeconds(0.1f);
        coreRemainsText.text = ExtraModeManager.Instance.CoresRemain.ToString();
        timerText.text = ExtraModeManager.Instance.Timer.ToString("0#.00");
    }

    void Update()
    {
        UpdateText();
        UpdateColor();
    }

    private void UpdateText()
    {
        _UpdateCoreRemains();
        _UpdateTimer();
    }

    private void UpdateColor() 
    {
        if (ExtraModeManager.Instance.Timer <= 11f)
        {
            timerText.color = Color.red;
            return;
        }
        timerText.color = Color.white;  
    }

    private void _UpdateCoreRemains()
    {
        coreRemainsText.text = ExtraModeManager.Instance.CoresRemain.ToString();
        if (ExtraModeManager.Instance.IsCollectedAllCores) finishLineNotif.gameObject.SetActive(true);
    }

    private void _UpdateTimer()
    {
        timerText.text = ExtraModeManager.Instance.Timer.ToString("0#.00");
    }

}
