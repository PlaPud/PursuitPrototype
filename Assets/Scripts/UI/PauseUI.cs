using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TMP_Text pauseText;
    [SerializeField] private List<Button> buttons;

    private Image _pauseBG;

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
            StartCoroutine(_FadeIn());
            pauseText.gameObject.SetActive(true);
            buttons.ForEach((button) => button.gameObject.SetActive(true));

        }
        else if (_pauseBG.enabled && !GameManager.Instance.IsPaused)
        {
            _pauseBG.enabled = false;
            pauseText.gameObject.SetActive(false);
            buttons.ForEach((button) => button.gameObject.SetActive(false));
        }
    }

    private IEnumerator _FadeIn()
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

    public void HandleOnResume()
    {
        GameManager.Instance.IsPaused = false;
    }

    public void HandleOnQuit()
    {
        GameManager.Instance.IsPaused = false;
        SceneManager.LoadScene("MainMenu");
    }
}
