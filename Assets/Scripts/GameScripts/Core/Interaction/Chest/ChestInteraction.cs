using NMX.GameCore.Network.Client;
using NMX.Protocal;
using UnityEngine;

namespace NMX
{
    public class ChestInteraction : IInteraction
    {
        private uint id;

        public ChestInteraction(uint id) { 
            this.id = id;
        }

        public async void InteractTrigger(InteractionType interactionType)
        {
            Debug.Log("Chest Interaction! Id" + id);

            await Client.NET.SendPacketAsync(GameCore.Network.Protocol.CmdType.ChestInteractionReq, new ChestInteractionReq { ChestId = 1});
        }
    }
}
