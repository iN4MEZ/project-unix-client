using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NMX
{
    public class MonsterManager : MonsterEntity
    {
        private Rigidbody rigidbody;

        private void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (rigidbody != null)
            {
                rigidbody.AddForce(Vector3.forward * 300 * Time.deltaTime);
            }
        }


    }
}
