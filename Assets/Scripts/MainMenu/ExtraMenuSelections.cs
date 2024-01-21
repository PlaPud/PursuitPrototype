using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExtraMenuSelections : MonoBehaviour
{
    [SerializeField] private TMP_Text menuTitle;
    [SerializeField] private TMP_Text menuDescription;
    [SerializeField] private List<Button> levels = new List<Button>();
    [SerializeField] private Button backToMenuBtn;

    [SerializeField] private Image bgImg;

    public static int ExtraModeLevel;

    void Start()
    {
        if (DataPersistenceManager.Instance) Destroy(DataPersistenceManager.Instance.gameObject);

        _FadeInBackground();
    }

    private void _FadeInBackground()
    {
        StartCoroutine(FadeInBackground());
        ChangeNonInteractBtnColor();
    }

    private IEnumerator FadeInBackground()
    {
        Color tmpColor = bgImg.color;
        bgImg.color = tmpColor;

        while (tmpColor.r < 0.6f)
        {
            tmpColor += new Color(0.02f, 0.02f, 0.02f);
            bgImg.color = tmpColor;
            yield return new WaitForSeconds(0.02f);
        }
    }

    private void ChangeNonInteractBtnColor()
    {
        foreach (var level in levels)
        {
            if (!level.interactable)
            {
                TMP_Text levelText = level.GetComponentInChildren<TMP_Text>();
                levelText.color = new Color(0.5f, 0.5f, 0.5f);
            }
        }
    }
    void Update()
    {
        
    }

    public void OnPressBackToMenu() 
    {
        Destroy(ExtraLevelSelect.Instance.gameObject);
        _LoadSceneAsync("MainMenu");
    }

    public void OnPressExtraLevel()
    {
        ExtraLevelBtn selectedBtn = EventSystem.current.currentSelectedGameObject.GetComponent<ExtraLevelBtn>();
        ExtraLevelSelect.Instance.SelectedLevel = selectedBtn.levelNumber;  
        _LoadSceneAsync("Loading");
    }

    private async void _LoadSceneAsync(string sceneName, bool isLoadingLevel = false)
    {

        AsyncOperation loadedScene = SceneManager.LoadSceneAsync(sceneName);
        loadedScene.allowSceneActivation = false;

        backToMenuBtn.interactable = false;
        levels.ForEach((btn) => btn.interactable = false);

        await Task.Delay(1000);

        do { await Task.Delay(100); } while (loadedScene.progress < 0.9f);

        loadedScene.allowSceneActivation = true;
    }
}
