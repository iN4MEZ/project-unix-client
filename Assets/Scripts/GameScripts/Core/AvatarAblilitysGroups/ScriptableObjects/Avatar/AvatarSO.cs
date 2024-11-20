using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NMX
{
    [Serializable]
    [CreateAssetMenu(fileName = "Avatar", menuName = "Characters/AvatarData")]
    public class AvatarSO : ScriptableObject
    {
        [field: SerializeField] public AvatarData Data { get; private set; }

        [field: SerializeField] public AvatarResourceData ResourceData { get; private set; }
    }
}
