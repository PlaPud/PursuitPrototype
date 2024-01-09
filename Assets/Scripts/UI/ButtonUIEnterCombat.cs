using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonUIEnterCombat : MonoBehaviour
{
    [SerializeField] Vector2 buttonOffset;
    private PlayerController playerCat;

    private void Awake()
    {
        playerCat = GameObject
            .FindGameObjectWithTag("PlayerCat")
            .GetComponent<PlayerController>();
    }

    private void Start()
    {
        EnemyAreaController.OnCombatStart += EnableUI;
        gameObject.SetActive(false);
    }

    private void Update()
    {

    }

    private void LateUpdate()
    {
        transform.position = playerCat.transform.position + (Vector3) buttonOffset;
    }

    private void EnableUI()
    {
        gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        EnemyAreaController.OnCombatStart -= EnableUI;
    }
}
