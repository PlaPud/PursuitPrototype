using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] RectTransform panel;
    [SerializeField] RectTransform loadingTMP;
    //[SerializeField] RectTransform loadingIcon;
    private bool _isLoading = false;

    void Start()
    {
        _LoadSavedGameScene();
    }

    void Update()
    {
        
    }

    private async void _LoadSavedGameScene() 
    {
        _isLoading = true;
        AsyncOperation gameScene = SceneManager.LoadSceneAsync("Stage1", mode: LoadSceneMode.Additive);
        gameScene.allowSceneActivation = false;

        do
        {
            Debug.Log(gameScene.progress);
            await Task.Delay(100);
        } while (gameScene.progress < 0.9f);

        gameScene.allowSceneActivation = true;
        await Task.Delay(3000);
        panel.gameObject.SetActive(false);
        loadingTMP.gameObject.SetActive(false);
        _isLoading = false;
        SceneManager.UnloadSceneAsync("Loading");
    }
}
