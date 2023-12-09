using Newtonsoft.Json;
using System;
using UnityEngine;

[System.Serializable]
public class Vector3Serialize 
{
    public float x;
    public float y;
    public float z;

    public Vector3Serialize(Vector3 vector)
    {
        this.x = vector.x;
        this.y = vector.y;
        this.z = vector.z;
    }

    public Vector3 ToUnityVector3() 
    {
        return new Vector3(x,y,z);
    }

}
