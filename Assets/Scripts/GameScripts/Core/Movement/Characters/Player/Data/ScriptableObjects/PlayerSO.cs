using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NMX
{
    [CreateAssetMenu(fileName = "Player", menuName = "Custom/Characters/Player")]
    public class PlayerSO : ScriptableObject
    {
        [field: SerializeField] public PlayerGroundedData GroundedData { get; private set; }

        [field: SerializeField] public PlayerPropertiesData PropertiesData { get; private set; }
    }
}
