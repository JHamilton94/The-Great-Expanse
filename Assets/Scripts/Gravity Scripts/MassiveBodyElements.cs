using UnityEngine;
using System.Collections;

public class MassiveBodyElements : MonoBehaviour {
    public double mass;
    public MassiveBodyType massiveBodyType;

    private double sphereOfInfluence;

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
