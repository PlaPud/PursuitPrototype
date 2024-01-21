using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraLvlData : MonoBehaviour
{
    [field: Header("Level Data")]
    [field: SerializeField] public int SecondsToComplete { get; private set; }
    [field: SerializeField] public Transform StartPoint { get; private set; }
    [field: SerializeField] public Transform FinishPoint { get; private set; }

    [field: SerializeField] public List<Transform> CoresPos { get; private set; }
    public int AmountOfCores => CoresPos.Count;

    void Start()
    {
        
    }

    void Update()
    {
         
    }
}
