using UnityEngine;
using System.Collections;

public class BlackHoleElements : MonoBehaviour {

    public string galaxyName;

    public string GalaxyName
    {
        get
        {
            return galaxyName;
        }

        set
        {
            galaxyName = value;
        }
    }
}
