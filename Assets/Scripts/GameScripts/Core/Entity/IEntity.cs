using NMX.Protocal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NMX
{
    public interface IEntity
    {
        public void Load(Transform transform,EntityInfo entityInfo);

        public void Create();
    }
}
