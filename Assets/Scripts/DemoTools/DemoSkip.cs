using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoSkip : MonoBehaviour
{
    [SerializeField] private List<Transform> _startPosition;

    private int _curIdx= 0;

    void Start()
    {
        
    }

    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Backslash)) return;

        ControllingManager.Instance.CatController.transform.position = _startPosition[_curIdx].position;
        _curIdx = (_curIdx + 1) % _startPosition.Count;
    }
}
