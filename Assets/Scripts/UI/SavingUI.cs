using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SavingUI : MonoBehaviour
{

    private TMP_Text _tmpText;

    private Animator _anim;
    private const string SAVING = "Saving";

    private void Awake()
    {
        _tmpText = GetComponentInChildren<TMP_Text>();    
        _anim = GetComponent<Animator>();   
    }

    void Start()
    {
        CheckPoint.OnSave += PlaySavingAnimation;
        _anim.enabled = false;
    }

    private void PlaySavingAnimation() 
    {
        _tmpText.enabled = true;
        StartCoroutine(_PlayAnimation());
    }

    IEnumerator _PlayAnimation()
    {
        _anim.enabled = true;
        _anim.Play(SAVING);
        yield return new WaitForSeconds(3f);
        _anim.enabled = false;
        _tmpText.enabled = false;
    }
}
