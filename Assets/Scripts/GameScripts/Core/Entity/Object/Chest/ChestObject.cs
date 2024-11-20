using System.Collections;
using System.Collections.Generic;
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
