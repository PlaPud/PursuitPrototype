using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] RectTransform panel;
    [SerializeField] RectTransform loadingTMP;


    void Start()
    {
        _LoadSavedGameScene();
    }

    void Update()
    {
        
    }

    private async void _LoadSavedGameScene() 
    {
        AsyncOperation gameScene = SceneManager.LoadSceneAsync("Stage1", mode: LoadSceneMode.Additive);
        gameScene.allowSceneActivation = false;

        do
        {
            await Task.Delay(100);

        } while (gameScene.progress < 0.9f);

        gameScene.allowSceneActivation = true;
        
        await Task.Delay(3000);
        
        panel.gameObject.SetActive(false);
        loadingTMP.gameObject.SetActive(false);

        SceneManager.UnloadSceneAsync("Loading");
    }
}
