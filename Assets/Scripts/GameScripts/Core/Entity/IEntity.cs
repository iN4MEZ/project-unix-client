using NMX.Protocal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NMX
{
    public interface IEntity
    {
        public void Load(Transform transform);

        public void RegisterServerEntityData(EntityInfo entityInfo);
    }
}
