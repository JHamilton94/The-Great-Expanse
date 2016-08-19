using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class StarGravityBehavior : GravityBehavior
{
    void Start()
    {
        base.Start();
        base.gravityElements.massiveBody.GetComponent<MassiveBodyElements>().satelites.Add(gameObject);
    }
}
