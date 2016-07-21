using UnityEngine;
using System.Collections;

//Putting all publicly used enums here, because f**k good practice.
//Ok maybe this is good practice... im not sure, look it's here and 
//im not moving it.
public enum OrbitTypes { circular, elliptical, parabolic, hyperbolic, error };
public enum StarType { dwarf, giant };
public enum MassiveBodyType {Planet, Star, Black_Hole }; //The order of these matters, dont change it.
public enum GravitationalType { ship, planet, star, black_hole};
public enum ManeuverHandle { nadir, zenith, prograde, retrograde, noManeuver};