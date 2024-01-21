using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExtraLevelBtn : MonoBehaviour
{
    [field: Range(1, 10)]
    [field: SerializeField] public int levelNumber { get; private set; }

    private Button _btn;

    private void Awake()
    {
        _btn = GetComponent<Button>();
        //_btn.onClick.AddListener(_ChangeLevelPref);
    }

    private void _ChangeLevelPref()
    {
         //PlayerPrefs.SetInt("ExtraModeLevel", levelNumber);
         //PlayerPrefs.Save();
         //ExtraMenuSelections.ExtraModeLevel = levelNumber;
         //_LoadSceneAsync("Loading");
    }

    private async void _LoadSceneAsync(string sceneName, bool isLoadingLevel = false)
    {

        AsyncOperation loadedScene = SceneManager.LoadSceneAsync(sceneName);
        loadedScene.allowSceneActivation = false;

        do { await Task.Delay(100); } while (loadedScene.progress < 0.9f);

        loadedScene.allowSceneActivation = true;
    }

}
