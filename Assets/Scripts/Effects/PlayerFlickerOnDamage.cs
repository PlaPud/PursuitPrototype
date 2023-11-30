using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerFlickerOnDamage : MonoBehaviour
{

    [SerializeField] int flickerTimes;
    [SerializeField] float flickerSec;
    [SerializeField] Color flickeringOpacityColor;

    private List<SpriteRenderer> _playerSR;
    private Color _playerColor;

    private void Awake()
    {
        _playerSR = GetComponentsInChildren<SpriteRenderer>().ToList();
        _playerColor = _playerSR[0].color;
    }

    void Start()
    {
        PlayerHealth.Instance.OnDamageTaken += TriggerFlicker;
    }

    void Update()
    {
        
    }

    private void TriggerFlicker(int damage) 
    {
        if (damage <= 0) return;

        StartCoroutine(
            _Flicker(
                flickerTimes: flickerTimes, 
                flickerSec: flickerSec
            )
        );
    }

    IEnumerator _Flicker(int flickerTimes, float flickerSec) 
    {
        for (int t = 0; t < flickerTimes; t++) 
        {
            _ChangeOpacity(flickeringOpacityColor);
            yield return new WaitForSeconds(flickerSec);
            _ChangeOpacity(_playerColor);
            yield return new WaitForSeconds(flickerSec);
        }
    }

    private void _ChangeOpacity(Color newColor) 
    {
        _playerSR.ForEach((sr) => { sr.color = newColor; });
    }
}
