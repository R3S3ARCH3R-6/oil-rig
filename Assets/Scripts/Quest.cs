using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest
{ 
    //  public string name = "";
    //  public string desc = "";
    //  public int status = 0;
    public float x = 0.0f;
    public float y = 0.0f;
    public float z = 0.0f;
    public float t = 0.0f;

    public Quest()
    {

    }

    public Quest(float x, float y, float z, float t)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.t = t;
    }

}