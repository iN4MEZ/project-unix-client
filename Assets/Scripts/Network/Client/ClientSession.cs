using System;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;
using NMX.GameCore.Network.Protocol;
using NMX.Protocal;
using static NMX.GameCore.Network.Client.Client;

namespace NMX.GameCore.Network.Client
{
    public class ClientSession : IDisposable
    {

        private const int MaxPacketSize = 32768;
        private readonly byte[] _recvBuffer;
        private readonly byte[] _sendBuffer;

        private const int ReadTimeout = 10;
        private const int WriteTimeout = 10;

        private bool active;

        private INetworkUnit networkUnit;

        private ServerCommandHandler serverCommandHandler;

        public ClientSession(INetworkUnit networkUnit)
        {
            this.networkUnit = networkUnit;

            serverCommandHandler = new ServerCommandHandler();
            serverCommandHandler.InitPacket();

            _recvBuffer = new byte[MaxPacketSize];
            _sendBuffer = new byte[MaxPacketSize];
        }

        public async ValueTask RunAsync()
        {
            Debug.Log("Client Network Activated!! Don't Destroy!");

            active = true;

            Client.instance.ClientConnectionState = CONNECTION_STATE.CONNECTED;

            Memory<byte> buffer = _recvBuffer.AsMemory();


            await Client.NET.SendPacketAsync(CmdType.PlayerTokenCs,new PlayerTokenCs { Token = Client.instance.ClientToken});

            await Client.NET.SendPacketAsync(CmdType.EnterGameReq);


            while(active)
            {
                int readAmount = await ReadWithTimeoutAsync(buffer,ReadTimeout);

                if (readAmount < 0)
                {
                    break;
                }

                int consumedBytes = await PacketHandlerAsync(buffer[..readAmount]);

                if (consumedBytes == -1)
                {
                    break;
                }

            }
        }

        public void Stop()
        {
            active = false;
        }

        private async ValueTask<int> PacketHandlerAsync(Memory<byte> buffer)
        {
            if(buffer.Length < 12)
            {
                return 0;
            }
            int consumed = 0;

            do
            {
                (NetPacket? packet, int bytesConsumed) = NetPacket.DecodeFrom(buffer[consumed..]);
                consumed += bytesConsumed;

                if (packet == null)
                    return consumed;

                await serverCommandHandler.packetHandlers[packet.CmdType](packet);

            } while (buffer.Length - consumed >= 12);
            return consumed;
        }

        public async ValueTask SendAsync(NetPacket packet)
        {
            Memory<byte> buffer = _sendBuffer.AsMemory();

            int length = packet.EncodeTo(buffer);

            await WriteWithTimeoutAsync(buffer[..length], WriteTimeout);
        }

        protected async ValueTask<int> ReadWithTimeoutAsync(Memory<byte> buffer, int timeoutSeconds)
        {
            using CancellationTokenSource cancellationTokenSource = new(TimeSpan.FromSeconds(timeoutSeconds));
            return await networkUnit!.ReceiveAsync(buffer, cancellationTokenSource.Token);
        }

        protected async ValueTask WriteWithTimeoutAsync(Memory<byte> buffer, int timeoutSeconds)
        {
            using CancellationTokenSource cancellationTokenSource = new(TimeSpan.FromSeconds(timeoutSeconds));
            await networkUnit!.SendAsync(buffer, cancellationTokenSource.Token);
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
