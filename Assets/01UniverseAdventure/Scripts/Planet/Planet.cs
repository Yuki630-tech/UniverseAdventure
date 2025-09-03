using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    [Tooltip("˜f¯‚Ìƒ^ƒCƒv"), SerializeField] private PlanetType planetType;

    public PlanetType PlanetTypeParam { get => planetType; }

    public enum PlanetType
    {
        Sphere,
        Box,
        SmallBox,
    }
}
