using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NMX
{
    public class NPCInteraction : IInteraction
    {
        public uint Id { get; private set; }

        public NPCInteraction(uint id)
        {
            Id = id;
        }

        public void InteractTrigger(InteractionType interactionType)
        {
            Debug.Log("Npc Id " + Id + " Get interact Type" + interactionType);
        }
    }
}
