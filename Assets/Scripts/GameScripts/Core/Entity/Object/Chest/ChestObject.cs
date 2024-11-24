using NMX.Protocal;
using UnityEngine;

namespace NMX
{
    public class ChestObject : Entity
    {
        public override void Load(Transform transform)
        {
            base.Load(transform);

            transform.position = new Vector3(9, 1, 9);

        }
    }
}
