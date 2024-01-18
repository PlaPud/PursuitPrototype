using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    public enum PauseMenuType
    {
        Pause,
        Volume
    }

    [Header("UI Pause Menu")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private TMP_Text pauseText;
    [SerializeField] private List<Button> buttons;

    [Header("UI Volume Menu")]
    [SerializeField] private GameObject volumeMenu;
    [SerializeField] private List<TMP_Text> volumeTexts;
    [SerializeField] private List<Slider> volumeSliders;
    [SerializeField] private Button backToPauseMenu;

    private Image _pauseBG;

    public PauseMenuType CurrentMenuType = PauseMenuType.Pause;

    private void Awake()
    {
        _pauseBG = GetComponent<Image>();
    }
    void Start()
    {
        
    }

    void Update()
    {
        if (!_pauseBG.enabled && GameManager.Instance.IsPaused)
        {
            pauseMenu.SetActive(true);
            StartCoroutine(_FadeInBackground());
            _EnableAllMenuUI();

        }
        else if (_pauseBG.enabled && !GameManager.Instance.IsPaused)
        {
            _pauseBG.enabled = false;
            _DisableAllMenuUI();
            _DisableAllVolumeUI();
            pauseMenu.SetActive(false);
        }
    }

    public void HandleOnResume()
    {
        GameManager.Instance.IsPaused = false;
    }

    public void HandleVolumeSetting()
    {
        _DisableAllMenuUI();
        _EnableAllVolumeUI();
    }

    public void HandleBackToPauseMenu()
    {
        _DisableAllVolumeUI();
        _EnableAllMenuUI();
    }

    private void _EnableAllVolumeUI()
    {
        volumeMenu.SetActive(true);
        volumeTexts.ForEach((text) => text.gameObject.SetActive(true));
        volumeSliders.ForEach((slider) => slider.gameObject.SetActive(true));
        backToPauseMenu.gameObject.SetActive(true);
    }

    private void _DisableAllVolumeUI()
    {
        volumeTexts.ForEach((text) => text.gameObject.SetActive(false));
        volumeSliders.ForEach((slider) => slider.gameObject.SetActive(false));
        volumeMenu.SetActive(false);
        backToPauseMenu.gameObject.SetActive(false);
    }

    private void _DisableAllMenuUI()
    {
        pauseText.gameObject.SetActive(false);
        buttons.ForEach((button) => button.gameObject.SetActive(false));
    }

    private void _EnableAllMenuUI()
    {
        pauseText.gameObject.SetActive(true);
        buttons.ForEach((button) => button.gameObject.SetActive(true));
    }

    private IEnumerator _FadeInBackground()
    {
        _pauseBG.enabled = true;
        _pauseBG.color = new Color(0, 0, 0, 0);
        float alpha = 0;
        while (alpha < 0.7f)
        {
            alpha += 0.1f;
            _pauseBG.color = new Color(0, 0, 0, alpha);
            yield return new WaitForSecondsRealtime(0.025f);
        }
    }

    public void HandleOnQuit()
    {
        GameManager.Instance.IsPaused = false;
        SceneManager.LoadScene("MainMenu");
    }
}
