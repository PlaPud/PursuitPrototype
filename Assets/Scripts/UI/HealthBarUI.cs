using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{

    [SerializeField] GameObject healthPiecePrefab;
    [SerializeField] Sprite healthPieceEmpty;
    [SerializeField] Sprite healthPieceFull;

    private List<GameObject> _healthBarDisplay = new List<GameObject>();
    void Start()
    {
        PlayerHealth.Instance.OnDamageTaken += DecreaseHealthBar;
        PlayerHealth.Instance.OnHealthRegen += RegenHealthBar;

        //EnemySpawnPoint.OnEnterCombat += EnableUI;
        //EnemyAreaController.OnCombatEnd += DisableUI;
        

        DisableUI();
        _RenderHealthBar();
    }

    private void _RenderHealthBar() 
    {
        int currentHealth = PlayerHealth.Instance.CurrentHealth;
        int maxHealth = PlayerHealth.Instance.MaxHealth;

        _healthBarDisplay.Clear();
        
        for (int i = 0; i < maxHealth; i++) 
        {
            GameObject newObj = Instantiate(healthPiecePrefab, transform);
            _healthBarDisplay.Add(newObj);
        }

        _decreaseHealthFromTo(
                fromIndex: maxHealth - 1,
                toIndexExclude: currentHealth
            );

        _healthBarDisplay.ForEach(
                (healthGO) => { healthGO.transform.SetParent(transform); }
            );

    }

    void Update()
    {
         
    }

    private void DecreaseHealthBar(int amount) 
    {
        int currentHealth = PlayerHealth.Instance.CurrentHealth;

        _decreaseHealthFromTo(
                fromIndex: currentHealth - 1, 
                toIndexExclude: currentHealth - amount - 1
            );
    }

    private void RegenHealthBar(int amount) 
    {
        int currentHealth = PlayerHealth.Instance.CurrentHealth;
        int maxHealth = PlayerHealth.Instance.MaxHealth;

        if (currentHealth == maxHealth) return;

        int regenHealth = currentHealth + amount <= maxHealth ? currentHealth + amount : maxHealth;

        for (int i = currentHealth; i < regenHealth; i++) 
        {
            Image healthImg = _healthBarDisplay[i].GetComponent<Image>();
            if (!healthImg) return;
            healthImg.sprite = healthPieceFull;
        }
    }

    private void _decreaseHealthFromTo(int fromIndex, int toIndexExclude) 
    {
        if (fromIndex < toIndexExclude) return;

        for (int i = fromIndex; i > toIndexExclude; i--) 
        {
            Image healthImg = _healthBarDisplay[i].GetComponent<Image>();
            if (!healthImg) return;
            healthImg.sprite = healthPieceEmpty;
        }
    }

    public void EnableUI() 
    {
        foreach (GameObject health in  _healthBarDisplay) 
        {
            Image healthImg = health.GetComponent<Image>();
            if (healthImg.enabled) return;
            healthImg.enabled = true;
        }
    }

    public void DisableUI() 
    {
        foreach (GameObject health in _healthBarDisplay)
        {
            Image healthImg = health.GetComponent<Image>();
            healthImg.enabled = false;
        }
    }
}
