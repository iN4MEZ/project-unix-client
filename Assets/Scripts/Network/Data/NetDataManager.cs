using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NMX
{
    public class NetDataManager : MonoBehaviour
    {
        [field: Header("Network Profile")]
        [field: SerializeField] public NetData NetData { get; private set; }

        [field: SerializeField] public PlayerBasicInfoData PlayerGameData;

        public static NetDataManager Instance;

        private void Awake()
        {
            Instance = this;
        }
    }
}
