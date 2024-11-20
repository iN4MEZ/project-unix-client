using NMX;
using NMX.Kcp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace NMX
{
    public class NetworkUnit : INetworkUnit
    {
        public IPEndPoint RemoteEndPoint { get; private set; }

        private KcpConversation _conversation;

        public NetworkUnit(KcpConversation conversation, IPEndPoint remoteEndPoint)
        {
            _conversation = conversation;
            RemoteEndPoint = remoteEndPoint;

        }

        public void Dispose()
        {
            _conversation.Dispose();
        }

        public async ValueTask<int> ReceiveAsync(Memory<byte> buffer, CancellationToken cancellationToken)
        {
            KcpConversationReceiveResult result = await _conversation.ReceiveAsync(buffer, cancellationToken);
            if (result.TransportClosed)
                return -1;

            return result.BytesReceived;
        }


        public async ValueTask SendAsync(Memory<byte> buffer, CancellationToken cancellationToken)
        {
            await _conversation.SendAsync(buffer, cancellationToken);
            
        }
    }
}
