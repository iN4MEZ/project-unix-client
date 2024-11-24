using Google.Protobuf;
using NMX.GameCore.Network.Protocol;
using NMX.Kcp;
using NMX.Protocal;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

using QFSW.QC;

namespace NMX.GameCore.Network.Client
{
    [Serializable]
    public class Client : MonoBehaviour
    {
        public UDP udp { get; private set; }

        public static Client instance;

        public static UDP NET { get { return instance.udp; } }


        public Player Player { get; private set; }

        private GameObject uiLoadingGate;

        private Canvas cvLoad;

        public enum CONNECTION_STATE { READY_TO_CONNECT,DISCONNECTED, CONNECTING,CONNECTED, CONNECT_FAIL }

        public CONNECTION_STATE ClientConnectionState;

        public string ClientToken { get; set; }

        public bool Connected { get
            {
                return udp.isConnected;
            }
        }

        private bool isSendBreath;

        private void Awake()
        {
            instance = this;

            if(instance == null)
            {
                instance = this;
            }


            if(instance != this)
            {
                Destroy(this);
                instance = this;
            }

            DontDestroyOnLoad(gameObject);
        }


        void Start () {

            uiLoadingGate = GameObject.FindGameObjectWithTag("LoadingUI");

            cvLoad = uiLoadingGate.GetComponentInChildren<Canvas>();

            cvLoad.enabled = false;

            ClientConnectionState = CONNECTION_STATE.READY_TO_CONNECT;

            Debug.Log("Initialize Client Instance Success! Ready To Uplink");
        }

        public async void Connect(IPEndPoint ip)
        {
            udp = new UDP(ip);

            Debug.Log("Created Protocol Instance Success! Prepare To Connect UDP SOCKET");

            LoginUI.Instance.gameObject.SetActive(false);

            //UIManager.instance.MainUICanvas.enabled = true;

            cvLoad.enabled = true;

            await udp.Connect();
        }

        private void Update()
        {

            if (!isSendBreath)
            {
                if(udp != null)
                {
                    if(udp.isConnected)
                    {
                        StartCoroutine(SendBreathingEnumerator());
                    }
                }
            }
        }

        private IEnumerator SendBreathingEnumerator()
        {
            isSendBreath = true;
            yield return new WaitForSeconds(1f);
            SendBreathingToServer(); // ส่งข้อมูลไปยังเซิร์ฟเวอร์ทุกๆ 1 วินาที

            isSendBreath = false;
        }

        private async void SendBreathingToServer()
        {
            await udp.SendPacketAsync(CmdType.PlayerBreathingReq);
        }

        public void OnConnectedSuccess()
        {
            
        }

        public void OnPlayerCreated()
        {
            Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        }

        public class UDP
        {
            private IPEndPoint endPoint;
            private UdpClient socket;
            private INetworkUnit networkUnit;

            public bool isConnected { get; private set; } = false;
            public ClientSession ClientSession { get; private set; }


            public UDP(IPEndPoint endPoint)
            {
                this.endPoint = endPoint;
            }

            public async Task Connect()
            {
                if (Client.instance.ClientConnectionState == CONNECTION_STATE.CONNECTING && Client.instance.ClientConnectionState == CONNECTION_STATE.CONNECTED)
                {
                    return;
                }
                socket = new UdpClient();

                try
                {
                    await SendHandshakeRequest(socket, endPoint);
                }
                catch (Exception e)
                {
                    Debug.LogError("Connection Error: " + e.ToString());
                }
                finally
                {
                }
            }
            async Task<UdpReceiveResult> ReceiveWithTimeout(UdpClient client, int timeout)
            {
                Task<UdpReceiveResult> receiveTask = client.ReceiveAsync();
                Task delayTask = Task.Delay(timeout);

                await Task.WhenAny(receiveTask, delayTask);

                if (receiveTask.IsCompleted)
                {
                    return await receiveTask;
                }
                else
                {
                    throw new TimeoutException("Timeout occurred while waiting for server response.");
                }
            }


            private async Task ReceiveHandshakeRespone(UdpClient udpClient)
            {
                try
                {
                    UdpReceiveResult result = await udpClient.ReceiveAsync();
                    HandShakeData handShakeData = await ReceivedHandshakeData(result);

                    if(handShakeData != null) {

                        long convId64 = handShakeData.Param1 << 32 | handShakeData.Param2;

                        using var kcpConv = KcpSocketTransport.CreateConversation(udpClient, endPoint, convId64, null);

                        kcpConv.Start();

                        networkUnit = new NetworkUnit(kcpConv.Connection, endPoint);

                        ClientSession = new ClientSession(networkUnit);

                        isConnected = true;

                        Debug.Log("Sync With Server! GoodJob! XD ");

                        await ClientSession!.RunAsync();
                    }
                     

                } catch (Exception e)
                {
                    Client.instance.ClientConnectionState = CONNECTION_STATE.DISCONNECTED;
                    Debug.LogException(e);
                    isConnected = false;
                    Debug.LogError("Uplink Fails!!!!");
                    ClientSession?.Dispose();
                    udpClient.Close();
                }
            }

            async Task SendHandshakeRequest(UdpClient udpClient, IPEndPoint serverEndPoint)
            {
                try
                {
                    Client.instance.ClientConnectionState = CONNECTION_STATE.CONNECTING;

                    // Create HandshakeData
                    byte[] handshakeData = CreatedHandshakeData();

                    // Sending To server
                    await udpClient.SendAsync(handshakeData, handshakeData.Length, serverEndPoint);
                }
                catch (Exception ex)
                {
                    print($"Error sending handshake request: {ex.Message}");
                    Client.instance.ClientConnectionState = CONNECTION_STATE.CONNECT_FAIL;
                } 
                finally
                {
                    await ReceiveHandshakeRespone(socket);
                }
            }

            private byte[] CreatedHandshakeData()
            {
                HandShakeData handShakeData = new HandShakeData()
                {
                    Head = HandShakeData.StartConversationHead,
                    Tail = HandShakeData.StartConversationTail,
                };
                byte[] buffer = ArrayPool<byte>.New(20);

                handShakeData.WriteTo(buffer);

                return buffer;
            }

            private async Task<HandShakeData> ReceivedHandshakeData(UdpReceiveResult result)
            {
                HandShakeData handShakeData = HandShakeData.ReadFrom(result.Buffer);

                return await Task.FromResult(handShakeData);
            }

            public async Task<Task> SendPacketAsync<TMessage>(CmdType type, TMessage message) where TMessage : IMessage 
            {
                NetPacket packet = new NetPacket()
                {
                    CmdType = type,
                    Head = Memory<byte>.Empty,
                    Body = message.ToByteArray()
                };

                await ClientSession.SendAsync(packet);

                return Task.CompletedTask;
            }

            public async Task<Task> SendPacketAsync<TMessage>(CmdType type, TMessage message,Action callback) where TMessage : IMessage
            {
                NetPacket packet = new NetPacket()
                {
                    CmdType = type,
                    Head = Memory<byte>.Empty,
                    Body = message.ToByteArray()
                };

                await ClientSession.SendAsync(packet);

                callback();

                return Task.CompletedTask;
            }

            public async Task<Task> SendPacketAsync(CmdType type)
            {
                NetPacket packet = new NetPacket()
                {
                    CmdType = type,
                };

                await ClientSession.SendAsync(packet);

                return Task.CompletedTask;
            }

            public IEnumerator SendPacketAsyncIEnumerator(CmdType type)
            {
                NetPacket packet = new NetPacket()
                {
                    CmdType = type,
                };

                // สร้าง Task และใช้ await ภายใน
                ValueTask task = ClientSession.SendAsync(packet);
                while (!task.IsCompleted)
                {
                    yield return null; // รอให้ Task เสร็จสมบูรณ์
                }
            }

            public async Task<Task> SendPacketAsync(CmdType type,Action callback)
            {
                NetPacket packet = new NetPacket()
                {
                    CmdType = type,
                };

                await ClientSession.SendAsync(packet);

                callback();

                return Task.CompletedTask;
            }

            [Command("coop")]
            private static void JoinCoop(string action,long id)
            {
                switch (action) {
                    case "join":
                        Debug.Log("Send Request to id: " + id);

                        Client.NET.SendPacketAsync(CmdType.PlayerEnterMpReq, new PlayerEnterMpReq { ToHost = id });

                        break;
                    case "leave":
                        break;
                }
            }

        }
    }
}
