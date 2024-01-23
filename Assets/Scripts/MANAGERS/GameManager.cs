using System.Collections;
using UnityEngine.SceneManagement;

using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool IsPaused = false;
    public bool IsFreezeControl = false;
    public bool IsSaving = false;
    public bool CanPause = true;
    public bool IsLoaded { get; private set; } = false;

    private PlayerController _playerCat;
    private PlayerCombatController _catCombat;

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
        _catCombat = _playerCat.GetComponent<PlayerCombatController>();
        
        if (PlayerHealth.Instance) 
        {
            PlayerHealth.Instance.OnPlayerDied += HandlePlayerDeath;
        }

        StartCoroutine(_WaitForLoad());
    }

    void Update()
    {
        if (!CanPause) return;

        if (!IsSaving && IsLoaded && Input.GetKeyDown(KeyCode.Escape))
        {
            IsPaused = !IsPaused;
        }
        
        HandleOnPause();
    }

    private IEnumerator _WaitForLoad()
    {
        IsFreezeControl = true;
        yield return new WaitForSeconds(3f);
        IsFreezeControl = false;
        IsLoaded = true;
    }

    private void HandlePlayerDeath() 
    {
        SceneManager.LoadScene("Loading");
    }

    private void HandleOnPause() 
    {
        Time.timeScale = IsPaused ? 0 : 1;

        if (IsPaused) 
        {
            Cursor.visible = true;
            return;
        }

        if (ControllingManager.Instance.IsControllingCompBot) 
        {
            Cursor.visible = true;
            return;
        }

        if (ControllingManager.Instance.IsControllingCat && _catCombat && _catCombat.IsInCombat) 
        {
            Cursor.visible = true;
            return;
        }

        Cursor.visible = false;

    }
}
