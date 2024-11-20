using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NMX
{
    [Serializable]
    public class NetData
    {
        [field: SerializeField] public NetPlayerData NetPlayerData {  get; private set; }
    }
}
