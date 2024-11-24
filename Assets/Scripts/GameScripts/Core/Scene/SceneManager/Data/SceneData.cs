using NMX.Protocal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NMX
{
    [Serializable]
    public class SceneData
    {
        [field: SerializeField] public Dictionary<EntityInfo,GameObject> entities { get; private set;  }
        [field: SerializeField] public uint Id { get; set;  }

        [field: SerializeField] public List<GameObject> chestObjects { get; private set; }

        public void Initialize()
        {
            entities = new Dictionary<EntityInfo, GameObject>();
            chestObjects = new List<GameObject>();
        }


    }
}
