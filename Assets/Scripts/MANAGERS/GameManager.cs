using System.Collections;
using UnityEngine.SceneManagement;

using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool IsPaused = false;
    public bool IsSaving = false;
    public bool IsLoaded { get; private set; } = false;

    private PlayerController _playerCat;

    private void Awake()
    {
        if (Instance != null) 
        {
            Debug.LogError("[ERROR] More than one GameManager in Scene.");
            return;
        } 
        
        Instance = this;
        Cursor.visible = false; 
    }

    void Start()
    {
        _playerCat = ControllingManager.Instance.CatController;
        PlayerHealth.Instance.OnPlayerDied += HandlePlayerDeath;

        StartCoroutine(_WaitForLoad());
    }

    void Update()
    {
        if (!IsSaving && IsLoaded && Input.GetKeyDown(KeyCode.Escape))
        {
            IsPaused = !IsPaused;
        }
        
        HandleOnPause();
    }

    private IEnumerator _WaitForLoad()
    {
        yield return new WaitForSeconds(4f);
        IsLoaded = true;
    }

    private void HandlePlayerDeath() 
    {
        SceneManager.LoadScene("Loading");
    }

    private void HandleOnPause() 
    {
        Time.timeScale = IsPaused ? 0 : 1;
        Cursor.visible = IsPaused;
    }
}
