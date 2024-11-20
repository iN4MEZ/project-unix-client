using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NMX
{
    public class PlayerInteraction : Interaction
    {
        public PlayerInteraction()
        {
            Debug.Log("Created Instance Player Interaction! asshole");
        }

        public override void Interact(IInteraction interaction,InteractionType interactionType)
        {
            base.Interact(interaction, interactionType);
        }

    }
}
