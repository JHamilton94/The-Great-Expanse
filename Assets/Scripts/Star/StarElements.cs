using UnityEngine;
using System.Collections;
using System;

public class StarElements : MonoBehaviour{

    public StarType starType;
    public string starName;

    public StarType StarType
    {
        get
        {
            return starType;
        }

        set
        {
            starType = value;
        }
    }

    public string StarName
    {
        get
        {
            return starName;
        }

        set
        {
            starName = value;
        }
    }

    public string getName()
    {
        return starName;
    }
}
