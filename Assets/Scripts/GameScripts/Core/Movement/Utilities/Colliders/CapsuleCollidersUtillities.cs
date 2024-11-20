using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NMX
{
    [Serializable]
    public class CapsuleCollidersUtillities
    {
        public CapsuleCollidersData CapsuleCollidersData {  get; private set; }
        [field: SerializeField] public DefaultCollidersData DefaultCollidersData {  get; private set; }
        [field: SerializeField] public SlopeData SlopeData {  get; private set; }


        public void Initialize(GameObject gameObject)
        {
            if(CapsuleCollidersData != null)
            {
                return;
            }

            CapsuleCollidersData = new CapsuleCollidersData();

            CapsuleCollidersData.Initialize(gameObject);
        }

        public void CalculateCapsuleColliderDimension()
        {
            SetCapsuleColliderRadius(DefaultCollidersData.Radius);
            SetCapsuleColliderHeight(DefaultCollidersData.Height * (1f - SlopeData.StepHeightPercentage));

            RecalculateCapsuleColliderCenter();

            float halfColliderHeight = CapsuleCollidersData.Collider.height / 2f;

            if(halfColliderHeight < CapsuleCollidersData.Collider.radius)
            {
                SetCapsuleColliderRadius(halfColliderHeight);
            }
        }

        public void SetCapsuleColliderRadius(float radius)
        {
            CapsuleCollidersData.Collider.radius = radius;
        }
        public void SetCapsuleColliderHeight(float height)
        {
            CapsuleCollidersData.Collider.height = height;
        }
        private void RecalculateCapsuleColliderCenter()
        {
            float colliderHeightDifference = DefaultCollidersData.Height - CapsuleCollidersData.Collider.height;
            Vector3 newColliderCenter = new Vector3(0f,DefaultCollidersData.CenterY + (colliderHeightDifference / 2f),0f);

            CapsuleCollidersData.Collider.center = newColliderCenter;
        }


    }
}
