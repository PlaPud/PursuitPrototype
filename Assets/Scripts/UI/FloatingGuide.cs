using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloatingGuide : MonoBehaviour
{
    [Header("Level")]
    [SerializeField] private LevelController levelGuidesIn;

    [Header("Guide")]   
    [SerializeField] private TMP_Text guideText;
    [SerializeField] private SpriteRenderer[] optGuideSprites = { };

    [Header("Guide Toggler")]
    [SerializeField] private OneTimeTrigger fadeInTrigger;
    [SerializeField] private OneTimeTrigger fadeOutTrigger;

    private Collider2D _fadeInCD, _fadeOutCD;

    private void Awake()
    {
        _fadeInCD = fadeInTrigger.GetComponent<Collider2D>();
        _fadeOutCD = fadeOutTrigger.GetComponent<Collider2D>();
        levelGuidesIn.OnLevelStatusLoaded += CheckDisableTriggers;
        fadeInTrigger.OnTriggerDisabled += ShowGuide;
        fadeOutTrigger.OnTriggerDisabled += HideGuide;
    }

    void Start()
    {
        guideText.color = new Color(0, 0, 0, 0);
        for (int i = 0; i < optGuideSprites.Length; i++)
        {
            optGuideSprites[i].color = new Color(255, 255, 255, 0);
        }
    }

    void Update()
    {

    }

    private void CheckDisableTriggers(bool isLevelComplete)
    {
        if (!isLevelComplete) return;
        _fadeInCD.enabled = false;
        _fadeOutCD.enabled = false;
        fadeInTrigger.OnTriggerDisabled -= ShowGuide;
        fadeOutTrigger.OnTriggerDisabled -= HideGuide;
        
    }

    private void ShowGuide() 
    {
        StartCoroutine(_FadeInGuide());
    }

    private void HideGuide() 
    {
        StartCoroutine(_FadeOutGuide());
    }

    private IEnumerator _FadeInGuide()
    {
        float alpha = 0;    
        while (alpha < 1)
        {
            alpha += 0.1f;
            guideText.color = new Color(255, 255, 255, alpha);

            if (optGuideSprites.Length > 0) 
            {
                foreach (SpriteRenderer sprite in optGuideSprites)
                {
                    sprite.color = new Color(255, 255, 255, alpha);
                }
            }
            yield return new WaitForSeconds(0.05f);

        }
    }

    private IEnumerator _FadeOutGuide()
    {
        float alpha = 1;
        while (alpha > 0)
        {
            alpha -= 0.1f;
            guideText.color = new Color(255, 255, 255, alpha);

            if (optGuideSprites.Length > 0) 
            {
                foreach (SpriteRenderer sprite in optGuideSprites)
                {
                    sprite.color = new Color(255, 255, 255, alpha);
                }
            }
            yield return new WaitForSeconds(0.05f);
        }
    }

    private void OnDestroy()
    {
        levelGuidesIn.OnLevelStatusLoaded -= CheckDisableTriggers;
        fadeInTrigger.OnTriggerDisabled -= ShowGuide;
        fadeOutTrigger.OnTriggerDisabled -= HideGuide;
    }
}   
