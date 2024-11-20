using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NMX
{

    [Serializable]
    public class SlopeData
    {
        [field: SerializeField][field: Range(0, 1)] public float StepHeightPercentage { get; private set; } = 0.25f;
        [field: SerializeField][field: Range(0, 5)] public float FloatRayDistance { get; private set; } = 2f;
        [field: SerializeField][field: Range(0, 50)] public float StepReachForce { get; private set; } = 25f;
    }
}
