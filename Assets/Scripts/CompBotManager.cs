using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CompBotManager : MonoBehaviour
{

    public static CompBotManager Instance;

    private List<CompBotPanelController> _compBotPanels;

    public bool IsControlCompBot { get; private set; }

    private void Awake()
    {
        Instance = this;
        _compBotPanels = FindObjectsOfType<CompBotPanelController>().ToList();
    }

    void Start()
    {
        
    }

    void Update()
    {
        IsControlCompBot = ControllingManager.Instance.IsControllingCompBot;
        AssignCompbot();
    }

    private void AssignCompbot() 
    {
        _compBotPanels.ForEach((panel) => { 
            panel.IsControllingThis = (panel.PlayerCD != null); 
        });
    }

}
