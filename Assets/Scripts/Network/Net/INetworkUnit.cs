using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace NMX
{
    public interface INetworkUnit : IDisposable
    {
        IPEndPoint RemoteEndPoint { get; }

        ValueTask<int> ReceiveAsync(Memory<byte> buffer, CancellationToken cancellationToken);
        ValueTask SendAsync(Memory<byte> buffer, CancellationToken cancellationToken);
    }
}
