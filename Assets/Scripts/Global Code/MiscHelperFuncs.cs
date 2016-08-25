using UnityEngine;
using System.Collections;
using System;

public static class MiscHelperFuncs {
    public static double convertToRadians(double degrees)
    {
        return (degrees * Math.PI) / 180;
    }

    public static Vector3 convertToVec3(Vector2 inVec)
    {
        return new Vector3(inVec.x, inVec.y, 0.0f);
    }

    public static Vector2 convertToVec2(Vector3 inVec)
    {
        return new Vector2(inVec.x, inVec.y);
    }

    public static double wrapAngle(double angleToWrap)
    {
        if (angleToWrap > Math.PI)
        {
            angleToWrap -= Math.PI;
            angleToWrap = -Math.PI + angleToWrap;
        }

        if (angleToWrap < -Math.PI)
        {
            angleToWrap += Math.PI;
            angleToWrap = Math.Abs(angleToWrap);
            angleToWrap = Math.PI - angleToWrap;
        }

        return angleToWrap;
    } 

    public static double convertTo360Angle(double angle)
    {

        //Debug.Log("In angle: " + angle);
        if(angle < 0)
        {
            angle = Math.PI - Math.Abs(angle);
            angle += Math.PI;
        }

        //Debug.Log("Return angle: " + angle);
        return angle;
    }

    public static double AngleBetweenVector2(Vector2 source, Vector2 target)
    {
        return (Mathf.DeltaAngle(Mathf.Atan2(source.y, source.x) * Mathf.Rad2Deg,
                                Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg)) * (Mathf.PI/180);
    }
}
