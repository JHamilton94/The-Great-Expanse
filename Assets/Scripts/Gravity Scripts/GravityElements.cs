using UnityEngine;
using System.Collections;

public class GravityElements : MonoBehaviour {
    
    public GravityElements()
    {

    }

    /**
    *   All vectors are relative to 0,0 and then transformed 
    *   based on the location of the celestial body. Velocity 
    *   is always calculated relative to the body it orbits.
    */

    public Vector2 velocity;
    public GameObject massiveBody;
    public GravitationalType gravitationalType;

    private double altitude;
    private double speed;
    private double velocityAngle;

    private OrbitTypes orbitType;
    private double mu;

    private Vector2 position;
    private Vector2 apogee;
    private Vector2 perigee;
    private Vector2 center;
    private double altitudeOfPerigee;
    private double angularMomentum;

    private double semiMajorAxis;
    private double semiLatusRectum;

    private Vector2 eccentricity;
    private double eccentricityMagnitude;
    private double eccentricAnomaly;
    private double trueAnomaly;
    private double meanAnomaly;
    private double anomalyAtEpoch;
    private double mechanicalEnergy;

    private double time;
    private double timeStep;
    private double timeAtEpoch;

    private bool clockwise;
    private bool towardsPerigee;

    private Vector2 globalTransformationVector;
    private double globalRotationAngle;
    
    public double Altitude
    {
        get
        {
            return altitude;
        }

        set
        {
            altitude = value;
        }
    }

    public double Speed
    {
        get
        {
            return speed;
        }

        set
        {
            speed = value;
        }
    }

    public double VelocityAngle
    {
        get
        {
            return velocityAngle;
        }

        set
        {
            velocityAngle = value;
        }
    }

    public OrbitTypes OrbitType
    {
        get
        {
            return orbitType;
        }

        set
        {
            orbitType = value;
        }
    }

    public double Mu
    {
        get
        {
            return mu;
        }

        set
        {
            mu = value;
        }
    }

    public Vector2 Apogee
    {
        get
        {
            return apogee;
        }

        set
        {
            apogee = value;
        }
    }

    public Vector2 Perigee
    {
        get
        {
            return perigee;
        }

        set
        {
            perigee = value;
        }
    }

    public Vector2 Center
    {
        get
        {
            return center;
        }

        set
        {
            center = value;
        }
    }

    public double AltitudeOfPerigee
    {
        get
        {
            return altitudeOfPerigee;
        }

        set
        {
            altitudeOfPerigee = value;
        }
    }

    public double AngularMomentum
    {
        get
        {
            return angularMomentum;
        }

        set
        {
            angularMomentum = value;
        }
    }

    public double SemiMajorAxis
    {
        get
        {
            return semiMajorAxis;
        }

        set
        {
            semiMajorAxis = value;
        }
    }

    public double SemiLatusRectum
    {
        get
        {
            return semiLatusRectum;
        }

        set
        {
            semiLatusRectum = value;
        }
    }

    public Vector2 Eccentricity
    {
        get
        {
            return eccentricity;
        }

        set
        {
            eccentricity = value;
        }
    }

    public double EccentricityMagnitude
    {
        get
        {
            return eccentricityMagnitude;
        }

        set
        {
            eccentricityMagnitude = value;
        }
    }

    public double EccentricAnomaly
    {
        get
        {
            return eccentricAnomaly;
        }

        set
        {
            eccentricAnomaly = value;
        }
    }

    public double TrueAnomaly
    {
        get
        {
            return trueAnomaly;
        }

        set
        {
            trueAnomaly = value;
        }
    }

    public double MeanAnomaly
    {
        get
        {
            return meanAnomaly;
        }

        set
        {
            meanAnomaly = value;
        }
    }

    public double AnomalyAtEpoch
    {
        get
        {
            return anomalyAtEpoch;
        }

        set
        {
            anomalyAtEpoch = value;
        }
    }

    public double MechanicalEnergy
    {
        get
        {
            return mechanicalEnergy;
        }

        set
        {
            mechanicalEnergy = value;
        }
    }

    public double Time
    {
        get
        {
            return time;
        }

        set
        {
            time = value;
        }
    }

    public double TimeStep
    {
        get
        {
            return timeStep;
        }

        set
        {
            timeStep = value;
        }
    }

    public double TimeAtEpoch
    {
        get
        {
            return timeAtEpoch;
        }

        set
        {
            timeAtEpoch = value;
        }
    }

    public bool Clockwise
    {
        get
        {
            return clockwise;
        }

        set
        {
            clockwise = value;
        }
    }

    public bool TowardsPerigee
    {
        get
        {
            return towardsPerigee;
        }

        set
        {
            towardsPerigee = value;
        }
    }

    public Vector2 GlobalTransformationVector
    {
        get
        {
            return globalTransformationVector;
        }

        set
        {
            globalTransformationVector = value;
        }
    }

    public double GlobalRotationAngle
    {
        get
        {
            return globalRotationAngle;
        }

        set
        {
            globalRotationAngle = value;
        }
    }

    public GameObject MassiveBody
    {
        get
        {
            return massiveBody;
        }

        set
        {
            massiveBody = value;
        }
    }

    public Vector2 Position
    {
        get
        {
            return position;
        }

        set
        {
            position = value;
        }
    }
}
