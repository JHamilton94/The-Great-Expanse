using UnityEngine;
using System.Collections;
using System;

public static class OrbitalHelper {
    public static double advanceTime(double timeAtEpoch, double timeStep, bool clockwise, OrbitTypes orbitType)
    {
        switch (orbitType)
        {
            case OrbitTypes.parabolic:
                if (clockwise)
                {
                    return timeAtEpoch - timeStep;
                }
                else
                {
                    return timeAtEpoch + timeStep;
                }
            case OrbitTypes.hyperbolic:
                if (clockwise)
                {
                    return timeAtEpoch - timeStep;
                }
                else
                {
                    return timeAtEpoch + timeStep;
                }
            default:
                if (clockwise)
                {
                    return timeAtEpoch - timeStep;
                }
                else
                {
                    return timeAtEpoch + timeStep;
                }
        }
    }

    public static Vector2 assembleVelocityVector(double velocityAngle, double speed)
    {
        return (new Vector2((float)Math.Cos(velocityAngle), (float)Math.Sin(velocityAngle)).normalized * (float)speed);
    }

    public static double calculateVelocityAngle(Vector2 position, Vector2 eccentricity, double semiMajorAxis,
        double trueAnomaly, double globalRotationAngle, bool clockwise, bool towardsPerigee, OrbitTypes orbitType)
    {
        double returnVelocityAngle = 0;
        double trueAnomalyModifier = 0;
        double alpha = 0;
        double k = 0;

        switch (orbitType)
        {
            case OrbitTypes.circular:
                //find new velocity vector
                k = position.magnitude / (semiMajorAxis);
                alpha = Math.Acos(((2 - (2 * Vector2.SqrMagnitude(eccentricity))) /
                    (k * (2 - k))) - 1);
                trueAnomalyModifier = (Math.PI - alpha) / 2;
                if (clockwise)
                {
                    if (towardsPerigee)
                    {
                        returnVelocityAngle = trueAnomaly + trueAnomalyModifier + globalRotationAngle;
                        returnVelocityAngle -= Math.PI;
                    }
                    else
                    {
                        returnVelocityAngle = trueAnomaly - trueAnomalyModifier + globalRotationAngle;

                    }
                }
                else
                {
                    if (towardsPerigee)
                    {
                        returnVelocityAngle = trueAnomaly - trueAnomalyModifier + globalRotationAngle;
                        returnVelocityAngle -= Math.PI;
                    }
                    else
                    {
                        returnVelocityAngle = trueAnomaly + trueAnomalyModifier + globalRotationAngle;
                    }
                }
                break;
            case OrbitTypes.elliptical:
                //find new velocity vector
                k = position.magnitude / (semiMajorAxis);
                alpha = Math.Acos(((2 - (2 * Vector2.SqrMagnitude(eccentricity))) /
                    (k * (2 - k))) - 1);

                trueAnomalyModifier = (Math.PI - alpha) / 2;

                if (clockwise)
                {
                    if (towardsPerigee)
                    {
                        returnVelocityAngle = trueAnomaly + trueAnomalyModifier + globalRotationAngle;
                        returnVelocityAngle -= Math.PI;
                    }
                    else
                    {
                        returnVelocityAngle = trueAnomaly - trueAnomalyModifier + globalRotationAngle;

                    }
                }
                else
                {
                    if (towardsPerigee)
                    {
                        returnVelocityAngle = trueAnomaly - trueAnomalyModifier + globalRotationAngle;
                        returnVelocityAngle -= Math.PI;
                    }
                    else
                    {
                        returnVelocityAngle = trueAnomaly + trueAnomalyModifier + globalRotationAngle;
                    }
                }
                break;
            case OrbitTypes.parabolic:
                break;
            case OrbitTypes.hyperbolic:
                //find new velocity vector
                k = position.magnitude / (semiMajorAxis);
                alpha = Math.Acos(((2 - (2 * Vector2.SqrMagnitude(eccentricity))) /
                    (k * (2 - k))) - 1);

                trueAnomalyModifier = (Math.PI - alpha) / 2;

                if (clockwise)
                {
                    if (towardsPerigee)
                    {
                        returnVelocityAngle = trueAnomaly + trueAnomalyModifier + globalRotationAngle;
                        returnVelocityAngle -= Math.PI;
                    }
                    else
                    {
                        returnVelocityAngle = trueAnomaly - trueAnomalyModifier + globalRotationAngle;
                    }
                }
                else
                {
                    if (towardsPerigee)
                    {
                        returnVelocityAngle = trueAnomaly - trueAnomalyModifier + globalRotationAngle;
                        returnVelocityAngle -= Math.PI;
                    }
                    else
                    {
                        returnVelocityAngle = trueAnomaly + trueAnomalyModifier + globalRotationAngle;
                    }
                }
                break;
        }
        return returnVelocityAngle;
    }

    public static bool towardsPerigeeOrbit(double meanAnomaly, bool clockwise, double timeAtEpoch, OrbitTypes orbitType)
    {
        return (clockwise && meanAnomaly > 0) || (!clockwise && meanAnomaly < 0);
    }

    public static Vector2 calculatePosition(Vector2 perigee, double trueAnomaly, double globalRotationAngle, double altitude, OrbitTypes orbitType)
    {
        Vector2 returnPosition = Vector2.right * float.PositiveInfinity;
        returnPosition = new Vector2((float)Math.Cos(trueAnomaly + globalRotationAngle), (float)Math.Sin(trueAnomaly + globalRotationAngle));
        returnPosition = returnPosition.normalized * (float)altitude;
        return returnPosition;
    }

    public static double calculateTrueAnomaly(Vector2 eccentricity, double eccentricAnomaly, double meanAnomaly, OrbitTypes orbitType)
    {
        double returnTrueAnomaly = double.PositiveInfinity;
        switch (orbitType)
        {
            case OrbitTypes.circular:
                //calculate true anomaly
                returnTrueAnomaly = Math.Acos(
                    (Math.Cos(eccentricAnomaly) - eccentricity.magnitude) /
                    (1 - (eccentricity.magnitude * Math.Cos(eccentricAnomaly)))
                    );

                if (meanAnomaly < 0)
                {
                    returnTrueAnomaly = -Math.Abs(returnTrueAnomaly);
                }
                break;
            case OrbitTypes.elliptical:
                //calculate true anomaly
                returnTrueAnomaly = Math.Acos(
                    (Math.Cos(eccentricAnomaly) - eccentricity.magnitude) /
                    (1 - (eccentricity.magnitude * Math.Cos(eccentricAnomaly)))
                    );
                if (meanAnomaly < 0)
                {
                    returnTrueAnomaly = -Math.Abs(returnTrueAnomaly);
                }
                break;
            case OrbitTypes.parabolic:
                returnTrueAnomaly = Math.Atan(
                    (Math.Sqrt((eccentricity.magnitude + 1) /
                    (eccentricity.magnitude - 1))) *
                    Math.Tanh(eccentricAnomaly / 2)
                    );
                if (meanAnomaly < 0)
                {
                    returnTrueAnomaly = -returnTrueAnomaly;
                }
                break;
            case OrbitTypes.hyperbolic:
                returnTrueAnomaly = Math.Atan(Math.Tanh(eccentricAnomaly / 2) / Math.Sqrt((eccentricity.magnitude - 1) / (eccentricity.magnitude + 1))) * 2;

                if (eccentricAnomaly < 0)
                {
                    returnTrueAnomaly = -Math.Abs(returnTrueAnomaly);
                }
                else
                {
                    returnTrueAnomaly = Math.Abs(returnTrueAnomaly);
                }


                break;
        }
        return returnTrueAnomaly;
    }

    public static double calculateEccentricAnomaly(Vector2 eccentricity, double semiMajorAxis, double GRAV_CONST,
        double timeStep, double timeAtEpoch, double meanAnomaly, double eccentricAnomaly, double mu, bool clockwise, OrbitTypes orbitType)
    {
        double returnEccentricAnomaly = 0.0d;
        switch (orbitType)
        {
            case OrbitTypes.circular:
                returnEccentricAnomaly = meanAnomaly + (2 * eccentricity.magnitude * Math.Sin(meanAnomaly)) + (1.25 * Vector2.SqrMagnitude(eccentricity) * Math.Sin(2 * meanAnomaly));
                break;
            case OrbitTypes.elliptical:
                while (returnEccentricAnomaly - eccentricity.magnitude * Math.Sin(returnEccentricAnomaly) < meanAnomaly)
                {
                    returnEccentricAnomaly += 1.0f;
                }

                while (returnEccentricAnomaly - eccentricity.magnitude * Math.Sin(returnEccentricAnomaly) > meanAnomaly)
                {
                    returnEccentricAnomaly -= 0.1f;
                }

                while (returnEccentricAnomaly - eccentricity.magnitude * Math.Sin(returnEccentricAnomaly) < meanAnomaly)
                {
                    returnEccentricAnomaly += 0.01f;
                }

                while (returnEccentricAnomaly - eccentricity.magnitude * Math.Sin(returnEccentricAnomaly) > meanAnomaly)
                {
                    returnEccentricAnomaly -= 0.001f;
                }

                while (returnEccentricAnomaly - eccentricity.magnitude * Math.Sin(returnEccentricAnomaly) < meanAnomaly)
                {
                    returnEccentricAnomaly += 0.0001f;
                }
                while (returnEccentricAnomaly - eccentricity.magnitude * Math.Sin(returnEccentricAnomaly) > meanAnomaly)
                {
                    returnEccentricAnomaly -= 0.00001f;
                }
                break;
            case OrbitTypes.parabolic:
                break;
            case OrbitTypes.hyperbolic:
                while (Math.Sqrt(Math.Pow(-semiMajorAxis, 3) / mu) * (eccentricity.magnitude * Math.Sinh(returnEccentricAnomaly) - returnEccentricAnomaly) < timeAtEpoch)
                {
                    returnEccentricAnomaly += 1.0f;
                }

                while (Math.Sqrt(Math.Pow(-semiMajorAxis, 3) / mu) * (eccentricity.magnitude * Math.Sinh(returnEccentricAnomaly) - returnEccentricAnomaly) > timeAtEpoch)
                {
                    returnEccentricAnomaly -= 0.1f;
                }

                while (Math.Sqrt(Math.Pow(-semiMajorAxis, 3) / mu) * (eccentricity.magnitude * Math.Sinh(returnEccentricAnomaly) - returnEccentricAnomaly) < timeAtEpoch)
                {
                    returnEccentricAnomaly += 0.01f;
                }

                while (Math.Sqrt(Math.Pow(-semiMajorAxis, 3) / mu) * (eccentricity.magnitude * Math.Sinh(returnEccentricAnomaly) - returnEccentricAnomaly) > timeAtEpoch)
                {
                    returnEccentricAnomaly -= 0.001f;
                }

                while (Math.Sqrt(Math.Pow(-semiMajorAxis, 3) / mu) * (eccentricity.magnitude * Math.Sinh(returnEccentricAnomaly) - returnEccentricAnomaly) < timeAtEpoch)
                {
                    returnEccentricAnomaly += 0.0001f;
                }
                while (Math.Sqrt(Math.Pow(-semiMajorAxis, 3) / mu) * (eccentricity.magnitude * Math.Sinh(returnEccentricAnomaly) - returnEccentricAnomaly) > timeAtEpoch)
                {
                    returnEccentricAnomaly -= 0.00001f;
                }
                break;
        }



        return returnEccentricAnomaly;
    }

    public static double calculateMeanAnomaly(Vector2 eccentricity, double semiMajorAxis,
        double anomalyAtEpoch, double timeStep, double timeAtEpoch, bool clockwise, double mu, OrbitTypes orbitType)
    {
        double orbitalSpeed;
        double returnMeanAnomaly = double.PositiveInfinity;

        //Calculate percentage of orbit being crossed
        orbitalSpeed = Math.Sqrt((mu) / Math.Pow(Math.Abs(semiMajorAxis), 3));
        if (clockwise)
        {
            returnMeanAnomaly = anomalyAtEpoch - (orbitalSpeed * timeStep);
        }
        else
        {
            returnMeanAnomaly = anomalyAtEpoch + (orbitalSpeed * timeStep);
        }

        if (orbitType != OrbitTypes.hyperbolic && orbitType != OrbitTypes.parabolic)
        {
            returnMeanAnomaly = MiscHelperFuncs.wrapAngle(returnMeanAnomaly);
        }
        return returnMeanAnomaly;
    }

    public static Vector2 calculateGlobalTranformationVector(GameObject celestialBody)
    {
        return celestialBody.transform.position;
    }

    public static double convertToWorldAngle(double localAngle, double globalRotationAngle, bool towardsPerigee)
    {
        if (towardsPerigee)
        {
            if (globalRotationAngle < 0)
            {
                localAngle = Math.Abs(localAngle) - Math.Abs(globalRotationAngle);
            }
            else
            {
                localAngle = Math.Abs(globalRotationAngle) - Math.Abs(localAngle);
            }
        }
        else
        {
            if (globalRotationAngle < 0)
            {
                localAngle = Math.Abs((2 * Math.PI) - Math.Abs(globalRotationAngle) - Math.Abs(localAngle));
            }
            else
            {
                localAngle = -Math.Abs((2 * Math.PI) - globalRotationAngle - localAngle);
            }
        }
        localAngle = MiscHelperFuncs.wrapAngle(localAngle);
        return localAngle;
    }

    public static double calculateTimeAtEpoch(Vector2 eccentricity, double eccentricAnomaly, double semiMajorAxis, double mu, bool clockwise, bool towardsPerigee, OrbitTypes orbitType)
    {
        double returnTime = double.PositiveInfinity;
        switch (orbitType)
        {
            case OrbitTypes.circular:
                returnTime = Math.Pow((Math.Pow(semiMajorAxis, 3) / mu), 0.5) * ((eccentricity.magnitude * Math.Sinh(eccentricAnomaly)) - eccentricAnomaly);
                break;
            case OrbitTypes.elliptical:
                returnTime = Math.Pow((Math.Pow(semiMajorAxis, 3) / mu), 0.5) * ((eccentricity.magnitude * Math.Sinh(eccentricAnomaly)) - eccentricAnomaly);
                break;
            case OrbitTypes.parabolic:
                returnTime = Math.Pow((Math.Pow(-semiMajorAxis, 3) / mu), 0.5) * ((eccentricity.magnitude * Math.Sinh(eccentricAnomaly)) - eccentricAnomaly);
                break;
            case OrbitTypes.hyperbolic:
                returnTime = Math.Pow((Math.Pow(-semiMajorAxis, 3) / mu), 0.5) * ((eccentricity.magnitude * Math.Sinh(eccentricAnomaly)) - eccentricAnomaly);
                break;
        }
        if (clockwise)
        {
            if (towardsPerigee)
            {
                returnTime = Math.Abs(returnTime);
            }
            else
            {
                returnTime = -Math.Abs(returnTime);
            }
        }
        else
        {
            if (towardsPerigee)
            {
                returnTime = -Math.Abs(returnTime);
            }
            else
            {
                returnTime = Math.Abs(returnTime);
            }
        }

        return returnTime;
    }

    public static double calculateAngularMomentum(Vector2 eccentricity, Vector2 perigee, double semiMajorAxis, double semiLatusRectum, double mu, OrbitTypes orbitType)
    {
        return calculateSpeed(perigee, semiMajorAxis, mu, orbitType) * calculateAltitude(eccentricity, semiMajorAxis, semiLatusRectum, 0, orbitType);
    }

    public static double calculateAltitude(Vector2 eccentricity, double semiMajorAxis, double semiLatusRectum, double trueAnomaly, OrbitTypes orbitType)
    {
        double returnAltitude = double.PositiveInfinity;
        switch (orbitType)
        {
            case OrbitTypes.circular:
                returnAltitude = (semiMajorAxis * (1 - (eccentricity.magnitude * eccentricity.magnitude))) /
                    (1 + (eccentricity.magnitude * Math.Cos(trueAnomaly)));
                break;
            case OrbitTypes.elliptical:
                returnAltitude = (semiMajorAxis * (1 - (eccentricity.magnitude * eccentricity.magnitude))) /
                    (1 + (eccentricity.magnitude * Math.Cos(trueAnomaly)));
                break;
            case OrbitTypes.parabolic:
                returnAltitude = (semiMajorAxis * (1 - (eccentricity.magnitude * eccentricity.magnitude))) /
                    (1 + (eccentricity.magnitude * Math.Cos(trueAnomaly)));
                break;
            case OrbitTypes.hyperbolic:
                returnAltitude = semiLatusRectum / (1 + (eccentricity.magnitude * Math.Cos(trueAnomaly)));
                break;
        }
        return returnAltitude;
    }

    public static double calculateSpeed(Vector2 position, double semiMajorAxis, double mu, OrbitTypes orbitType)
    {
        return Math.Sqrt((mu) * ((2 / position.magnitude) - (1 / semiMajorAxis)));
    }

    public static double calculateAnomalyAtCurrentEpoch(Vector2 eccentricity, double eccentricAnomaly, bool clockwise, OrbitTypes orbitType)
    {
        double returnAnomaly = 0;
        switch (orbitType)
        {
            case OrbitTypes.circular:
                returnAnomaly = eccentricAnomaly - eccentricity.magnitude * Math.Sin(eccentricAnomaly);
                //flip sign if clockwise
                if (clockwise)
                {
                    returnAnomaly = -returnAnomaly;
                }
                break;
            case OrbitTypes.elliptical:
                returnAnomaly = eccentricAnomaly - eccentricity.magnitude * Math.Sin(eccentricAnomaly);
                //flip sign if clockwise
                if (clockwise)
                {
                    returnAnomaly = -returnAnomaly;
                }
                break;
            case OrbitTypes.parabolic:
                returnAnomaly = eccentricAnomaly + (Math.Pow(eccentricAnomaly, 3) / 3);
                //flip sign if clockwise
                if (clockwise)
                {
                    returnAnomaly = -returnAnomaly;
                }
                break;
            case OrbitTypes.hyperbolic:
                returnAnomaly = (eccentricity.magnitude * Math.Sinh(eccentricAnomaly)) - eccentricAnomaly;
                break;
        }


        return returnAnomaly;
    }

    public static double calculateEccentricAnomaly(Vector2 eccentricity, double trueAnomaly, bool towardsPerigee, OrbitTypes orbitType)
    {
        double returnEccentricAnomaly = 0;
        switch (orbitType)
        {
            case OrbitTypes.circular:
                returnEccentricAnomaly = Math.Acos((eccentricity.magnitude + Math.Cos(trueAnomaly)) /
                (1 + (eccentricity.magnitude * Math.Cos(trueAnomaly))));
                if (towardsPerigee)
                {
                    returnEccentricAnomaly = -returnEccentricAnomaly;
                }
                break;
            case OrbitTypes.elliptical:
                returnEccentricAnomaly = Math.Acos((eccentricity.magnitude + Math.Cos(trueAnomaly)) /
                        (1 + (eccentricity.magnitude * Math.Cos(trueAnomaly))));
                if (towardsPerigee)
                {
                    returnEccentricAnomaly = -returnEccentricAnomaly;
                }
                break;
            case OrbitTypes.parabolic:
                break;
            case OrbitTypes.hyperbolic:
                double x = (eccentricity.magnitude + Math.Cos(trueAnomaly)) / (1 + (eccentricity.magnitude * Math.Cos(trueAnomaly)));
                returnEccentricAnomaly = Math.Log(x + Math.Sqrt((x * x) - 1));
                if (trueAnomaly < 0)
                {
                    returnEccentricAnomaly = -Math.Abs(returnEccentricAnomaly);
                }
                else
                {
                    returnEccentricAnomaly = Math.Abs(returnEccentricAnomaly);
                }
                break;
        }



        return returnEccentricAnomaly;
    }

    public static double calculateTrueAnomaly(Vector2 eccentricity, Vector2 position, bool towardsPerigee, bool clockwise, OrbitTypes orbitType)
    {
        double returnTrueAnomaly = double.PositiveInfinity;
        switch (orbitType)
        {
            case OrbitTypes.circular:
                returnTrueAnomaly = Vector2.Angle(position, Vector2.right);
                returnTrueAnomaly = MiscHelperFuncs.convertToRadians(returnTrueAnomaly);
                break;
            case OrbitTypes.elliptical:
                returnTrueAnomaly = Vector2.Angle(eccentricity, position);
                returnTrueAnomaly = MiscHelperFuncs.convertToRadians(returnTrueAnomaly);
                if (towardsPerigee)
                {
                    returnTrueAnomaly = -Math.Abs(returnTrueAnomaly);
                }
                else
                {
                    returnTrueAnomaly = Math.Abs(returnTrueAnomaly);
                }

                break;
            case OrbitTypes.parabolic:
                break;
            case OrbitTypes.hyperbolic:
                returnTrueAnomaly = Vector2.Angle(eccentricity, position);
                returnTrueAnomaly = MiscHelperFuncs.convertToRadians(returnTrueAnomaly);
                if (clockwise)
                {
                    if (towardsPerigee)
                    {
                        returnTrueAnomaly = Math.Abs(returnTrueAnomaly);
                    }
                    else
                    {
                        returnTrueAnomaly = -Math.Abs(returnTrueAnomaly);
                    }
                }
                else
                {
                    if (towardsPerigee)
                    {
                        returnTrueAnomaly = -Math.Abs(returnTrueAnomaly);
                    }
                    else
                    {
                        returnTrueAnomaly = Math.Abs(returnTrueAnomaly);
                    }
                }
                break;
        }

        return returnTrueAnomaly;
    }

    public static bool towardsPerigeeOrbit(Vector2 velocity, Vector2 eccentricity, OrbitTypes orbitType)
    {
        switch (orbitType)
        {
            case OrbitTypes.circular:
                return (MiscHelperFuncs.convertToRadians(Vector2.Angle(Vector2.right, velocity)) < Math.PI / 2);
            case OrbitTypes.elliptical:
                return (MiscHelperFuncs.convertToRadians(Vector2.Angle(eccentricity, velocity)) < Math.PI / 2);
            case OrbitTypes.parabolic:
                return true;
            case OrbitTypes.hyperbolic:
                return (MiscHelperFuncs.convertToRadians(Vector2.Angle(eccentricity, velocity)) < Math.PI / 2);
        }
        return true;

    }

    public static bool clockwiseOrbit(Vector2 position, Vector2 velocity)
    {
        Vector3 crossProduct;
        crossProduct = Vector3.Cross(MiscHelperFuncs.convertToVec3(position), MiscHelperFuncs.convertToVec3(velocity));
        if (crossProduct.z > 0)
        {
            return false;
        }
        return true;
    }

    public static double calculateGlobalRotationAngle(Vector2 eccentricity, OrbitTypes orbitType)
    {
        double returnGlobalRotationAngle = 0;
        switch (orbitType)
        {
            case OrbitTypes.circular:
                returnGlobalRotationAngle = 0;
                break;
            case OrbitTypes.elliptical:
                returnGlobalRotationAngle = Math.Atan2(eccentricity.y, eccentricity.x);
                break;
            case OrbitTypes.parabolic:
                returnGlobalRotationAngle = Math.Atan2(eccentricity.y, eccentricity.x);
                break;
            case OrbitTypes.hyperbolic:
                returnGlobalRotationAngle = Math.Atan2(eccentricity.y, eccentricity.x);
                break;
        }
        //conver to positive angle
        if (returnGlobalRotationAngle < 0)
        {
            returnGlobalRotationAngle = (Math.PI - Math.Abs(returnGlobalRotationAngle)) + Math.PI;
        }
        return returnGlobalRotationAngle;
    }

    public static Vector2 calculateCenter(double semiMajorAxis, Vector2 perigee, OrbitTypes orbitType)
    {
        return -perigee.normalized * (float)semiMajorAxis + perigee;
    }

    public static Vector2 calculatePerigee(double semiMajorAxis, Vector2 eccentricity, OrbitTypes orbitType)
    {
        double altitudeOfPerigee = 0;
        Vector2 returnPerigee = Vector2.right;
        switch (orbitType)
        {
            case OrbitTypes.circular:
                altitudeOfPerigee = semiMajorAxis;
                returnPerigee = Vector2.right;
                returnPerigee = returnPerigee * (float)altitudeOfPerigee;
                break;
            case OrbitTypes.elliptical:
                altitudeOfPerigee = semiMajorAxis * (1 - eccentricity.magnitude);
                returnPerigee = eccentricity.normalized;
                returnPerigee = returnPerigee * (float)altitudeOfPerigee;
                break;
            case OrbitTypes.parabolic:
                altitudeOfPerigee = -semiMajorAxis * (eccentricity.magnitude - 1);
                returnPerigee = eccentricity.normalized * (float)altitudeOfPerigee;
                break;
            case OrbitTypes.hyperbolic:
                altitudeOfPerigee = -semiMajorAxis * (eccentricity.magnitude - 1);
                returnPerigee = eccentricity.normalized * (float)altitudeOfPerigee;
                break;
        }
        return returnPerigee;
    }

    public static Vector2 calculateApogee(double semiMajorAxis, Vector2 eccentricity, OrbitTypes orbitType)
    {
        double altitudeOfApogee = 0;
        Vector2 returnApogee = Vector2.right;
        switch (orbitType)
        {
            case OrbitTypes.circular:
                altitudeOfApogee = semiMajorAxis;
                returnApogee = Vector2.right;
                returnApogee = -returnApogee * (float)altitudeOfApogee;
                break;
            case OrbitTypes.elliptical:
                altitudeOfApogee = semiMajorAxis * (1 + eccentricity.magnitude);
                returnApogee = -eccentricity.normalized;
                returnApogee = returnApogee * (float)altitudeOfApogee;
                break;
            case OrbitTypes.parabolic:
                altitudeOfApogee = double.PositiveInfinity;
                returnApogee = -Vector2.right * float.PositiveInfinity;
                break;
            case OrbitTypes.hyperbolic:
                altitudeOfApogee = double.PositiveInfinity;
                returnApogee = -Vector2.right * float.PositiveInfinity;
                break;
        }
        return returnApogee;
    }

    public static double calculateSemiLatusRectum(double semiMajorAxis, Vector2 eccentricity, Vector2 perigee, OrbitTypes orbitType)
    {
        if (orbitType != OrbitTypes.parabolic)
        {
            return semiMajorAxis * (1 - Vector2.SqrMagnitude(eccentricity));
        }
        else
        {
            return 2 * perigee.magnitude;
        }
    }

    public static double calculateSemiMajorAxis(double mechanicalEnergy, double mu, OrbitTypes orbitType)
    {
        switch (orbitType)
        {
            case OrbitTypes.circular:
                return -((mu) / (2 * mechanicalEnergy));
            case OrbitTypes.elliptical:
                return -((mu) / (2 * mechanicalEnergy));
            case OrbitTypes.parabolic:
                return double.PositiveInfinity;
            case OrbitTypes.hyperbolic:
                return -((mu) / (2 * mechanicalEnergy));
        }
        return double.NegativeInfinity;
    }

    public static double calculateMechanicalEnergy(Vector2 position, Vector2 velocity, double mu, OrbitTypes orbitType)
    {
        return (Vector2.SqrMagnitude(velocity) / 2) - ((mu) / position.magnitude);
    }

    public static OrbitTypes determineOrbitType(Vector2 eccentricity)
    {
        if (eccentricity.magnitude == 0)
        {
            return OrbitTypes.circular;
        }
        else if (eccentricity.magnitude < 1
          && eccentricity.magnitude > 0)
        {
            return OrbitTypes.elliptical;
        }
        else if (eccentricity.magnitude == 1.0f)
        {
            return OrbitTypes.parabolic;
        }
        else if (eccentricity.magnitude > 1)
        {
            return OrbitTypes.hyperbolic;
        }

        //Shouled never reach this
        return OrbitTypes.error;
    }

    public static Vector2 calculateEccentricity(Vector2 inPosition, Vector2 inVelocity, double inMu)
    {
        return ((inPosition * (float)(Vector2.SqrMagnitude(inVelocity) - ((inMu) / inPosition.magnitude))) - ((float)(Vector2.Dot(inPosition, inVelocity)) * inVelocity)) *
            (float)(1 / (inMu));
    }
}
