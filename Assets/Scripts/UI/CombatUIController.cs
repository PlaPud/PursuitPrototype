using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CombatUIController : MonoBehaviour
{
    [SerializeField] List<GameObject> combatUis = new();

    private List<Image> _imgs = new();

    void Start()
    {
        if (combatUis.Count <= 0) return;
        combatUis.ForEach((img) => _imgs.Add(img.GetComponent<Image>()));
    }

    void Update()
    {
        
    }
}
