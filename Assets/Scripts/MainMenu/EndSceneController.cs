using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndSceneController : MonoBehaviour
{
    [SerializeField] private Image bgImg;
    [SerializeField] private TMP_Text endText;
    [SerializeField] private TMP_Text subText;

    void Start()
    {
        endText.color = new Color(255, 255, 255, 0);
        subText.color = new Color(255, 255, 255, 0);
    }

    void Update()
    {
        if (endText.color.a < 1)
        {
            endText.color = new Color(255, 255, 255, endText.color.a + 0.01f);
            subText.color = new Color(255, 255, 255, subText.color.a + 0.01f);
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            _LoadMainMenu();
        }
    }

    private void _LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
