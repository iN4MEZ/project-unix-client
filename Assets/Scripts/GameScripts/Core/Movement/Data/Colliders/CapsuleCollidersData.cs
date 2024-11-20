using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NMX
{
    public class CapsuleCollidersData
    {
        public CapsuleCollider Collider { get; private set; }

        public Vector3 ColliderCenterInLocalSpace { get; private set; }

        public void Initialize(GameObject gameObject)
        {
            if (Collider != null)
            {
                return;
            }

            Collider = gameObject.GetComponent<CapsuleCollider>();

            UpdateColliderData();
        }

        public void UpdateColliderData()
        {
            ColliderCenterInLocalSpace = Collider.center;
        }
    }
}
