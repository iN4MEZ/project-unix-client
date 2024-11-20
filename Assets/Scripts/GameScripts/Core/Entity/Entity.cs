using NMX.Protocal;
using System;
using UnityEngine;

namespace NMX
{
    [Serializable]

    public abstract class Entity : MonoBehaviour, IEntity
    {
        [field: Header("Entity Info")]
        [field:SerializeField] public uint EntityId {  get; protected set; }

        [field:SerializeField] public EntityType EntityType { get; protected set; }

        public GameObject OnLoadResourceGameObject { get; protected set; }

        public EntityInfo EntityInfo { get; protected set; }


        public Vector3 Position { get { return Position; }  set { transform.position = value; } }

        public Rigidbody Rigidbody { get; }


        public virtual void Create()
        {
            throw new System.NotImplementedException();
        }

        public virtual void Load(Transform transform)
        {
            Debug.Log("Entity ID: " + EntityId + " Type: " + EntityType + " Has Loaded!");
        }

    }
}
