using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TempLoadGame : MonoBehaviour
{

    private bool _isLoading = false;

    void Start()
    {
        
    }

    void Update()
    {
        if (!_isLoading && Input.GetKeyDown(KeyCode.Mouse0)) 
        {
            _LoadingScreen();
        }    
    }

    private async void _LoadingScreen() 
    {
        _isLoading = true;
        AsyncOperation gameScene = SceneManager.LoadSceneAsync("Loading");
        gameScene.allowSceneActivation = false;

        do
        {
            Debug.Log(gameScene.progress);
            await Task.Delay(100);

        } while (gameScene.progress < 0.9f);

        gameScene.allowSceneActivation = true;
        _isLoading = false;
    }
}
