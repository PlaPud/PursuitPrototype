using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CombatUIController : MonoBehaviour
{
    //[SerializeField] List<GameObject> combatUis = new();
    //[SerializeField] 
    private Image _playerHeadImage;
    private BombAmmoUI _ammoUI;
    private HealthBarUI _healthBarUI;
    private RegenCircle _regenCircleUI;

    private void Awake()
    {
        _playerHeadImage = GetComponentInChildren<Image>();
        _ammoUI = GetComponentInChildren<BombAmmoUI>();
        _healthBarUI = GetComponentInChildren<HealthBarUI>();
        _regenCircleUI = GetComponentInChildren<RegenCircle>();
    }

    void Start()
    {
        //if (combatUis.Count <= 0) return;
        //combatUis.ForEach((img) => _imgs.Add(img.GetComponent<Image>()));
        EnemyAreaController.OnCombatStart += EnableEveryUI;
        EnemyAreaController.OnCombatEnd += DisableEveryUI;
        StartCoroutine(_LateStart());
    }

    private IEnumerator _LateStart() 
    {
        yield return new WaitForSeconds(.005f);
        DisableEveryUI();
    }

    void Update()
    {
        
    }
    private void DisableEveryUI()
    {
        _playerHeadImage.enabled = false;
        _healthBarUI.DisableUI();
        _ammoUI.DisableUI();
        _regenCircleUI.DisableUI();   
    }

    private void EnableEveryUI()
    {
        _playerHeadImage.enabled = true;
        _healthBarUI.EnableUI();
        _ammoUI.EnableUI();
        _regenCircleUI.EnableUI();
    }

    private void OnDestroy()
    {
        EnemyAreaController.OnCombatStart -= EnableEveryUI;
        EnemyAreaController.OnCombatEnd -= DisableEveryUI;
    }

}
