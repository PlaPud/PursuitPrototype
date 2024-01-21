using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreditList : MonoBehaviour
{

    [SerializeField] private List<TMP_Text> creditRoles = new();
    [SerializeField] private List<TMP_Text> creditNames = new();
    [SerializeField] private Button backbtn;

    [SerializeField] private GameObject mainMenu;

    void Start()
    {
        backbtn.interactable = false;
    }

    private void OnEnable()
    {
        backbtn.interactable = false;

        foreach (var role in creditRoles)
        {
            role.CrossFadeAlpha(0, 0, true);
        }

        foreach (var name in creditNames)
        {
            name.CrossFadeAlpha(0, 0, true);
        }

        backbtn.GetComponentInChildren<TMP_Text>().CrossFadeAlpha(0, 0, true);

        StartCoroutine(_FadeInCreditList());
    }

    private void OnDisable()
    {
        mainMenu.SetActive(true);
        mainMenu.GetComponent<MenuSelections>().FadeInMenu();
    }

    private IEnumerator _FadeInCreditList()
    {
        yield return new WaitForSeconds(0.5f);

        foreach (var role in creditRoles)
        {
            role.CrossFadeAlpha(1f, 1f, false);
        }

        yield return new WaitForSeconds(1f);

        foreach (var name in creditNames)
        {
            name.CrossFadeAlpha(1f, 1f, false);
        }

        yield return new WaitForSeconds(1f);

        backbtn.GetComponentInChildren<TMP_Text>().CrossFadeAlpha(1f, 1f, false);
        backbtn.interactable = true;
    }

    public void OnPressBack() 
    {
        gameObject.SetActive(false);
    }

    void Update()
    {

    }
}
