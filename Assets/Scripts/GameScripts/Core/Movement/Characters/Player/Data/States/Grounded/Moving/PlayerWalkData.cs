using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NMX
{
    [Serializable]
    public class PlayerWalkData
    {

        [field: SerializeField][field: Range(0, 1f)] public float SpeedModifier { get; private set; } = 0.225f;
    }
}
