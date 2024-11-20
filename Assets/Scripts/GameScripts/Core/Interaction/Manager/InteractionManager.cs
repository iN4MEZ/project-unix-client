using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NMX
{
    public class InteractionManager : MonoBehaviour
    {
        private PlayerInteraction interaction;

        private void Start()
        {
            interaction = new PlayerInteraction();
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("NPC")) {
                var npcCompo = other.GetComponent<NPCManager>();

                if(npcCompo.EntityType != EntityType.NPC) { return; }

                interaction.Interact(new NPCInteraction(npcCompo.EntityId),InteractionType.TALK);
            }
            if (other.CompareTag("SceneObject"))
            {
                var chestCompo = other.GetComponent<ChestManager>();

                if (chestCompo.EntityType != EntityType.CHESTOBJECT) { return; }

                interaction.Interact(new ChestInteraction(1), InteractionType.OPEN);
            }
        }
        private void OnTriggerExit(Collider other)
        {
            
        }
    }
}
