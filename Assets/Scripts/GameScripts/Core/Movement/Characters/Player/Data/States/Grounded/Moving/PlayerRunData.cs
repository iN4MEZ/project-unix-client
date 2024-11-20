using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NMX
{
    [Serializable]
    public class PlayerRunData 
    {
        [field: SerializeField][field: Range(0, 2f)] public float SpeedModifier { get; private set; }
    }
}
