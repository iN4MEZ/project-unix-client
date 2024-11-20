using System;
using UnityEngine;

namespace NMX
{
    [Serializable]
    public class PlayerGroundedData
    {
        [field: SerializeField][field: Range(0f, 25f)] public float BaseSpeed { get; private set; } = 5f;

        [field: SerializeField] public PlayerRotationData BaseRotationData { get; private set; }
        [field: SerializeField] public PlayerWalkData WalkData { get; private set; }
        [field: SerializeField] public PlayerRunData RunData { get; private set; }

        [field: SerializeField] public PlayerSprintData SprintData { get; private set; }

        [field: SerializeField] public PlayerDashedData DashedData { get; private set; }

        [field: SerializeField] public PlayerStopData StopData { get; private set; }

        [field: SerializeField] public AnimationCurve SlopeSpeedAngle { get; private set; }
    }
}
