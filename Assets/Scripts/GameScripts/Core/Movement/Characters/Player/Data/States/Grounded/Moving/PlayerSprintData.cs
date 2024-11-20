using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NMX
{
    [Serializable]
    public class PlayerSprintData
    {
        [field: SerializeField][field: Range(1, 3)] public float SpeedModifier { get; private set; } = 1.7f;
        [field: SerializeField][field: Range(0, 5)] public float SprintToRunTime { get; private set; } = 1f;

        [field: SerializeField][field: Range(0, 2)] public float RunToWalkTime { get; private set; } = 0.5f;
    }
}
