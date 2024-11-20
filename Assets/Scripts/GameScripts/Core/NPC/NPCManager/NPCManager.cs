using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NMX
{
    public class NPCManager : NPCEntity
    {
        [field: SerializeField] public NPCInteraction npcInteract {  get; private set; }

        private void Awake()
        {
            npcInteract = new NPCInteraction(EntityId);
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.tag == "Player")
            {
                Debug.Log(EntityId + " MSG: Player!!!");
            }
        }


    }
}
