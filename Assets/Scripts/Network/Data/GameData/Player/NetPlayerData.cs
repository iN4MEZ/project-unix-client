using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NMX
{
    [Serializable]
    public class NetPlayerData
    {
        [field: SerializeField] public PlayerPropertiesData PropertiesData { get; private set; }
        [field: SerializeField] public int id { get; private set; }


    }
}
