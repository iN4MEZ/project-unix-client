using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NMX
{
    [Serializable]
    public class AttackStatesData
    {
        [field: SerializeField] public string StateOwner { get; private set; }

        [field: SerializeField] public PlayerAttackState Logic { get; private set; }
    }
}
