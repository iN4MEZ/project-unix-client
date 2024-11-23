using NMX.Protocal;
using UnityEngine;

namespace NMX
{
    public class ChestObject : Entity
    {
        public override void Load(Transform transform, EntityInfo entityInfo)
        {
            base.Load(transform,entityInfo);

            transform.position = new Vector3(9, 1, 9);

        }
    }
}
