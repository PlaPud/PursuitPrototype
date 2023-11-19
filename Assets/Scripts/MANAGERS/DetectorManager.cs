using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectorManager : MonoBehaviour
{
    public static DetectorManager instance;

    [SerializeField] private List<ItemDetectorController> detectors = new List<ItemDetectorController>();

    private void Awake() => instance = this;

    private void Start()
    {
        
    }
    private void Update()
    {
        
    }

    public ItemDetectorController findDetector(string detectorGuid) 
    {
        foreach (ItemDetectorController detector in detectors) 
        {
            if (detector.guid == detectorGuid) return detector;
        }
        return null;
    } 
}
