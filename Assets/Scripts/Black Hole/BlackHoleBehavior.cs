using UnityEngine;
using System.Collections;
using System;

public class BlackHoleBehavior : MonoBehaviour {

    private BlackHoleElements blackHoleElements;
    private MassiveBodyElements massiveBodyElements;
    
    // Use this for initialization
    void Start () {
        blackHoleElements = GetComponent<BlackHoleElements>();
        massiveBodyElements = GetComponent<MassiveBodyElements>();
	}

    public string getName()
    {
        return blackHoleElements.GalaxyName;
    }
}
