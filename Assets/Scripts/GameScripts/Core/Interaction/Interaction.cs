using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NMX
{
    public abstract class Interaction : Entity
    {

        public Interaction() {
        }

        public virtual void Interact(IInteraction interaction, InteractionType type)
        {
            interaction.InteractTrigger(type);
        }
    }
}
