using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] Image panelImg;
    [SerializeField] TMP_Text loadingText;
    [SerializeField] Image loadingLogoImg;

    public string StageToLoad { get; private set; }

    void Awake()
    {
        Cursor.visible = false; 
    }

    void Start()
    {
        StageToLoad = PlayerPrefs.GetString("StageToLoad", "MainMenu");
        _LoadSavedGameScene();
    }

    void Update()
    {
        
    }

    private async void _LoadSavedGameScene()
    {
        AsyncOperation stageScene = SceneManager.LoadSceneAsync(StageToLoad, mode: LoadSceneMode.Additive);
        stageScene.allowSceneActivation = false;

        do
        {
            await Task.Delay(100);

        } while (stageScene.progress < 0.9f);

        stageScene.allowSceneActivation = true;

        await Task.Delay(3000);

        await _FadeOutLoadingScreen();

        SceneManager.UnloadSceneAsync("Loading");
    }

    private async Task _FadeOutLoadingScreen()
    {
        for (float a = 1; a >= 0; a -= 0.05f)
        {
            panelImg.color = new Color(0, 0, 0, a);
            loadingText.color = new Color(255, 255, 255, a);
            loadingLogoImg.color = new Color(255, 255, 255, a);
            await Task.Delay(10);
        }
    }
}
