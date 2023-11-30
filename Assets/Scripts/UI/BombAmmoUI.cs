using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BombAmmoUI : MonoBehaviour
{
    [Header("Reference Script")]
    [SerializeField] PlayerCombatController playerCombat;
    [Header("Bomb UI")]
    [SerializeField] GameObject bombPrefab;
    [SerializeField] Sprite bombFull;
    [SerializeField] Sprite bombEmpty;

    private List<GameObject> _bombDisplay = new List<GameObject>();
    private int _bombEmptyCount = 0;

    void Start()
    {
        playerCombat.OnPlayerShootBomb += RemoveAmmoSlot;
        playerCombat.OnCheckReload += ReloadAmmoSlot;
        _RenderAmmoSlot();
    }

    void _RenderAmmoSlot() 
    {
        _bombDisplay.Clear();

        for (int i = 0; i < playerCombat.MaxInField; i++) 
        {
            GameObject newObj = Instantiate(bombPrefab, transform);
            _bombDisplay.Add(newObj);
        }

        _bombDisplay.ForEach(
                (bombGO) => { bombGO.transform.SetParent(transform); }
            );
    }

    void Update()
    {
        
    }

    private void RemoveAmmoSlot() 
    {
        if (playerCombat.MaxInField == playerCombat.CurrentInField.Count) return;
        
        int oldInField = playerCombat.CurrentInField.Count;
        int changeSpriteIdx = playerCombat.MaxInField - oldInField - 1;

        Image ammoImg = _bombDisplay[changeSpriteIdx].GetComponent<Image>();
        ammoImg.sprite = bombEmpty;
        _bombEmptyCount += 1;
    }

    private void ReloadAmmoSlot() 
    {
        if (_bombEmptyCount < playerCombat.CurrentInField.Count) return;

        int afterReloadInField = playerCombat.CurrentInField.Count;
        int changeSpriteIdx = playerCombat.MaxInField - afterReloadInField - 1;

        Image ammoImg = _bombDisplay[changeSpriteIdx].GetComponent<Image>();
        ammoImg.sprite = bombFull;
        _bombEmptyCount -= 1;
    }
}
