using NMX.GameCore.Network.Client;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace NMX
{
    public class SinglePacketResult : IResult
    {
        private NetPacket _packet;

        public SinglePacketResult(NetPacket? packet)
        {
            _packet = packet;
        }

        public bool NextPacket([MaybeNullWhen(false)] out NetPacket packet)
        {
            packet = _packet;
            _packet = null;

            return packet != null;
        }
    }
}
