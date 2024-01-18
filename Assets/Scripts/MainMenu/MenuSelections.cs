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

    [Header("Menu Options")]
    [SerializeField] private Button contBtn;
    [SerializeField] private List<Button> menuBtns = new();

    [Header("Confirm Start Options")]
    [SerializeField] private List<TMP_Text> confirmTexts;
    [SerializeField] private List<Button> startConfirmsBtn = new();

    private List<Button> _allBtns = new List<Button>();

    private void Awake()
    {
        GetComponentsInChildren<Button>().ToList().ForEach((btn) => _allBtns.Add(btn));
        Cursor.visible = true;
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
        if (!DataPersistenceManager.Instance.IsSaveDataExist)
        {
            _StartNewGame();
            return;
        }

        _DisplayConfirm();
    }

    public void OnPressContinue() 
    {
        _LoadingScreen();
    }

    public void OnPressQuit() 
    {
        Application.Quit();
    }

    public void OnPressConfirmNewGame()
    {
        _StartNewGame();
    }

    public void OnPressCancel() 
    {
        startConfirmsBtn.ForEach((btn) => btn.interactable = false);

        StartCoroutine(_FadeBtns(
                fadeOut: true, 
                btns: startConfirmsBtn
            ));

        startConfirmsBtn.ForEach((btn) => btn.gameObject.SetActive(false));
        confirmTexts.ForEach((text) => text.gameObject.SetActive(false));

        menuBtns.ForEach((btn) => btn.gameObject.SetActive(true));

        StartCoroutine(_FadeBtns(
                fadeOut: false,
                btns: menuBtns
            ));

        menuBtns.ForEach((btn) => btn.interactable = true);
    }

    private void _StartNewGame()
    {
        DataPersistenceManager.Instance.NewGameData();
        _LoadingScreen();
    }

    private void _DisplayConfirm()
    {
        menuBtns.ForEach((btn) => btn.interactable = false);

        StartCoroutine(_FadeBtns(
                fadeOut: true, btns: menuBtns
            ));

        menuBtns.ForEach((btn) => btn.gameObject.SetActive(false));

        startConfirmsBtn.ForEach((btn) => btn.gameObject.SetActive(true));
        confirmTexts.ForEach((text) => text.gameObject.SetActive(true));

        StartCoroutine(_FadeBtns(
                fadeOut: false, 
                btns: startConfirmsBtn, 
                confirmTexts: confirmTexts
            ));

        startConfirmsBtn.ForEach((btn) => btn.interactable = true);
    }

    private async void _LoadingScreen()
    {
        AsyncOperation gameScene = SceneManager.LoadSceneAsync("Loading");
        gameScene.allowSceneActivation = false;

        _allBtns.ForEach((btn) => btn.interactable = false);

        do { await Task.Delay(100); } while (gameScene.progress < 0.9f);

        gameScene.allowSceneActivation = true;
    }

    private IEnumerator _FadeBtns(bool fadeOut, List<Button> btns, List<TMP_Text> confirmTexts = null)
    {
        float fadeTime = 0.5f;
        float fadeSpeed = 1 / fadeTime;
        float fadeStart = fadeOut ? 1 : 0;
        float fadeEnd = fadeOut ? 0 : 1;

        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * fadeSpeed;

            // lerp text color in btn
            btns.ForEach(
                (btn) => {
                    TMP_Text btn_text = btn.GetComponentInChildren<TMP_Text>();
                    btn_text.color = new Color(
                        btn_text.color.r,
                        btn_text.color.g,
                        btn_text.color.b,
                        Mathf.Lerp(fadeStart, fadeEnd, t
                    ));
                }
            );

            confirmTexts?.ForEach(
                (text) => text.color = new Color(
                    text.color.r, 
                    text.color.g, 
                    text.color.b, 
                    Mathf.Lerp(fadeStart, fadeEnd, t * 2)
            ));

            yield return null;
        }
    }

}
