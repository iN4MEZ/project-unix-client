using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NMX
{
    [CreateAssetMenu(fileName = "AttackState", menuName = "Characters/AttackState")]
    public class AttackStateSO : ScriptableObject
    {
        [field: SerializeField] public AttackStatesData AttackStatesData { get; private set; }
    }
}
