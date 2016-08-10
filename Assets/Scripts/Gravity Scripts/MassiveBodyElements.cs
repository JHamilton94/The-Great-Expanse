using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MassiveBodyElements : MonoBehaviour {
    public double mass;
    public MassiveBodyType massiveBodyType;

    private double sphereOfInfluence;

    public List<GameObject> satelites;

    public double SphereOfInfluence
    {
        get
        {
            return sphereOfInfluence;
        }

        set
        {
            sphereOfInfluence = value;
        }
    }
}
