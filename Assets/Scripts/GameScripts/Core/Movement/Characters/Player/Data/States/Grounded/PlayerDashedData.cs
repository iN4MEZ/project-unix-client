using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NMX
{
    [Serializable]
    public class PlayerDashedData
    {
        [field: SerializeField] [field: Range(2f,5f)] public float SpeedModifier = 2f;

        [field: SerializeField][field: Range(0, 2)] public float TimeToBeConsideredConsecutive { get; private set; } = 1f;
        [field: SerializeField][field: Range(1, 10)] public int ConsecutiveDashesLimitAmout { get; private set; } = 2;
        [field: SerializeField][field: Range(0, 5)] public float DashLimitReachedCooldown { get; private set; } = 1.75f;
    }
}
