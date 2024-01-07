using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuSelections : MonoBehaviour
{

    [SerializeField] private Button contBtn;
    
    private List<Button> btns = new List<Button>();


    private void Awake()
    {
        GetComponentsInChildren<Button>().ToList().ForEach((btn) => btns.Add(btn));
    }

    void Start()
    {
        contBtn.interactable = DataPersistenceManager.Instance.IsSaveDataExist;
        contBtn.GetComponentInChildren<TMP_Text>().color = contBtn.interactable ? Color.white : Color.gray;
    }

    void Update()
    {
        
    }

    public void OnPressNewGame() 
    {
        DataPersistenceManager.Instance.NewGameData();
        _LoadingScreen();
    }

    public void OnPressContinue() 
    {
        _LoadingScreen();
    }

    public void OnPressQuit() 
    {
        Application.Quit();
    }

    private async void _LoadingScreen()
    {
        AsyncOperation gameScene = SceneManager.LoadSceneAsync("Loading");
        gameScene.allowSceneActivation = false;

        btns.ForEach((btn) => btn.interactable = false);

        do { await Task.Delay(100); } while (gameScene.progress < 0.9f);

        gameScene.allowSceneActivation = true;
    }
}
