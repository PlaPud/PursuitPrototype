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
    [Header("Background")]
    [SerializeField] private Image backgroundImg;

    [Header("Menu Options")]
    [SerializeField] private Button contBtn;
    [SerializeField] private Button extraMode;
    [SerializeField] private List<Button> menuBtns = new();

    [Header("Confirm Start Options")]
    [SerializeField] private List<TMP_Text> confirmTexts;
    [SerializeField] private List<Button> startConfirmsBtn = new();

    [Header("Credit")]
    [SerializeField] private GameObject creditList;

    private List<Button> _allBtns = new List<Button>();


    private void Awake()
    {
        GetComponentsInChildren<Button>().ToList().ForEach((btn) => _allBtns.Add(btn));
        Cursor.visible = true;
        Time.timeScale = 1;
    }

    void Start()
    {
        _SetInteractable();
        StartCoroutine(_FadeInBackground());
    }

    private void _SetInteractable()
    {
        contBtn.interactable = DataPersistenceManager.Instance.IsSaveDataExist;
        extraMode.interactable = DataPersistenceManager.Instance.IsAnyStageCleared;
        contBtn.GetComponentInChildren<TMP_Text>().color = contBtn.interactable ? Color.white : Color.gray;
        extraMode.GetComponentInChildren<TMP_Text>().color = extraMode.interactable ? Color.white : Color.gray;
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
        _LoadSceneAsync("Loading");
    }

    public void OnPressExtra() 
    {
        _LoadSceneAsync("ExtraModeMenu");
    }

    public void OnPressCredit() 
    {
        FadeOutMenu();
        creditList.SetActive(true);
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

        FadeInMenu();
    }

    private void _StartNewGame()
    {
        DataPersistenceManager.Instance.NewGameData();
        _LoadSceneAsync("Loading");
    }
    private void _DisplayConfirm()
    {
        FadeOutMenu();

        startConfirmsBtn.ForEach((btn) => btn.gameObject.SetActive(true));
        confirmTexts.ForEach((text) => text.gameObject.SetActive(true));

        StartCoroutine(_FadeBtns(
                fadeOut: false,
                btns: startConfirmsBtn,
                confirmTexts: confirmTexts
            ));

        startConfirmsBtn.ForEach((btn) => btn.interactable = true);
    }
    public void FadeInMenu()
    {
        menuBtns.ForEach((btn) => btn.gameObject.SetActive(true));

        StartCoroutine(_FadeBtns(
                fadeOut: false,
                btns: menuBtns
            ));

        menuBtns.ForEach((btn) => btn.interactable = true);
        contBtn.interactable = DataPersistenceManager.Instance.IsSaveDataExist;
        extraMode.interactable = DataPersistenceManager.Instance.IsAnyStageCleared;

    }
    public void FadeOutMenu()
    {
        menuBtns.ForEach((btn) => btn.interactable = false);

        StartCoroutine(_FadeBtns(
                fadeOut: true, btns: menuBtns
            ));

        menuBtns.ForEach((btn) => btn.gameObject.SetActive(false));
    }

    private async void _LoadSceneAsync(string sceneName)
    {
        AsyncOperation gameScene = SceneManager.LoadSceneAsync(sceneName);
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

    private IEnumerator _FadeInBackground()
    {
        Debug.Log("Fade In");
        Color tmpColor = backgroundImg.color;
        backgroundImg.color = tmpColor;

        while (tmpColor.b < 0.4f)
        {
            tmpColor += new Color(0.01f, 0.01f, 0.01f);
            backgroundImg.color = tmpColor;
            yield return new WaitForSeconds(0.01f);
        }
    }

}
