using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultBannerUI : MonoBehaviour
{
    [Header("Result Data")]
    [SerializeField] private Image bannerBG;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text coreLeftText;

    [Header("Navigations")]
    [SerializeField] private List<Button> bannerBtns = new();

    private void Awake()
    {
        _DisableUI();
    }

    private void _DisableUI()
    {
        bannerBG.fillAmount = 0;
        bannerBG.gameObject.SetActive(false);   
        resultText.gameObject.SetActive(false);
        timeText.gameObject.SetActive(false);
        coreLeftText.gameObject.SetActive(false);
        bannerBtns.ForEach((btn) => btn.gameObject.SetActive(false));
    }

    void Start()
    {
        GameManager.Instance.CanPause = true;  
        ExtraModeManager.Instance.OnLevelWin += EnableBanner;
        ExtraModeManager.Instance.OnLevelFailed += EnableBanner;
    }

    public void OnPressRetry() 
    {
        _LoadSceneAsync("Loading");
    }

    public void OnPressReturn()
    {
        Destroy(ExtraLevelSelect.Instance.gameObject);
        _LoadSceneAsync("MainMenu");
    }

    private void EnableBanner() 
    {
        GameManager.Instance.CanPause = false;
        GameManager.Instance.IsFreezeControl = true;

        Cursor.visible = true;
        bannerBG.gameObject.SetActive(true);
        _FillBanner();

        resultText.gameObject.SetActive(true);
        timeText.gameObject.SetActive(true);
        coreLeftText.gameObject.SetActive(true);

        _UpdateResultText();
        _UpdateCoreLeftText();
        _UpdateTimeText();
        _EnableButtons();

        _PlaySound();
    }

    private void _PlaySound() 
    {
        if (!AudioManager.Instance) return;

        if (ExtraModeManager.Instance.IsWin)
        {
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.SavedLevelComplete, transform.position);
            return;
        }

        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.ExtraModeFailed, transform.position);

    }

    private void _FillBanner()
    {
        StartCoroutine(_FillBannerBG());
    }

    private IEnumerator _FillBannerBG()
    {
        while (bannerBG.fillAmount < 1f)
        {
            bannerBG.fillAmount += 0.05f;
            yield return new WaitForSecondsRealtime(0.01f);
        }
    }

    private void _UpdateResultText()
    {
        if (ExtraModeManager.Instance.IsWin)
        {
            resultText.text = "CLEAR!";
            return;
        }

        resultText.text = "FAILED";
    }

    private void _UpdateCoreLeftText()
    {
        if (ExtraModeManager.Instance.IsWin)
        {
            coreLeftText.text = "";
            return;
        }
        coreLeftText.text = $"Core Left: {ExtraModeManager.Instance.CoresRemain}";
    }

    private void _UpdateTimeText()
    {
        if (ExtraModeManager.Instance.IsWin)
        {
            timeText.text = $"Time: {ExtraModeManager.Instance.ElapsedTime.ToString("0#.00")} s";
            return;
        }

        timeText.text = $"Time: TIME'S UP";
    }

    private void _EnableButtons()
    {
        bannerBtns[0].gameObject.SetActive(true);
        bannerBtns[1].gameObject.SetActive(true);
    }

    private async void _LoadSceneAsync(string sceneName)
    {
        AsyncOperation gameScene = SceneManager.LoadSceneAsync(sceneName);
        gameScene.allowSceneActivation = false;

        bannerBtns.ForEach((btn) => btn.interactable = false);

        do { await Task.Delay(100); } while (gameScene.progress < 0.9f);

        gameScene.allowSceneActivation = true;
    }

    private void OnDestroy()
    {
        ExtraModeManager.Instance.OnLevelWin -= EnableBanner;
        ExtraModeManager.Instance.OnLevelFailed -= EnableBanner;
    }
}
