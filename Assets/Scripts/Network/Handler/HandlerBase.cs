using Google.Protobuf;
using NMX.GameCore.Network.Client;
using NMX.GameCore.Network.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NMX
{
    public abstract class HandlerBase
    {
        public NetPacket? Packet { get; set; }

        protected IResult Ok()
        {
            return new SinglePacketResult(null);
        }

        protected IResult Response<TMessage>(CmdType cmdType, TMessage message) where TMessage : IMessage
        {
            return new SinglePacketResult(new()
            {
                CmdType = cmdType,
                Head = Memory<byte>.Empty,
                Body = message.ToByteArray()
            });
        }
    }
}
