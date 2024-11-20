using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NMX
{
    [Serializable]
    public class PlayerStopData
    {
        [field: SerializeField] [field:Range(0f,15f)] public float LightDecelerationForce =5f;
        [field: SerializeField] [field:Range(0f,15f)] public float MediumDecelerationForce = 6.5f;
        [field: SerializeField] [field:Range(0f,15f)] public float HardDecelerationForce = 5f;
    }
}
