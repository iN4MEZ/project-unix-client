using NMX.GameCore.Network.Client;
using NMX.GameCore.Network.Protocol;
using NMX.Protocal;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using QFSW.QC;

namespace NMX
{
    public class ServerCommandHandler
    {

        public delegate Task PacketHandler(NetPacket packet);

        public Dictionary<CmdType, PacketHandler> packetHandlers { get; private set; }

        public void InitPacket()
        {
            packetHandlers = new Dictionary<CmdType, PacketHandler>();
            packetHandlers[CmdType.EnterGameRsp] = EnterGameRsp;
            packetHandlers[CmdType.PlayerBreathingRsp] = BreathingRsp;
            packetHandlers[CmdType.GetTeamLineupDataRsp] = GetTeamLineupData;
            packetHandlers[CmdType.GetAvatarDataRsp] = GetAvatarDataRspHandle;
            packetHandlers[CmdType.EnterSceneRsp] = EnterSceneRsp;
            packetHandlers[CmdType.ClientInitNotify] = ClientInitNotify;
            packetHandlers[CmdType.PlayerTokenSc] = PlayerTokenScs;
            packetHandlers[CmdType.PlayerEnterSceneNotify] = PlayerEnterSceneNotifyHandle;
            packetHandlers[CmdType.ChestInteractionRsp] = ChestInteractionRspHandle;
            packetHandlers[CmdType.SceneChestUpdateNotify] = SceneUpdateNotify;
            packetHandlers[CmdType.PlayerSwitchAvatarRsp] = SwitchAvatarRspHandle;
            packetHandlers[CmdType.ScPlayerSync] = PlayerSync;
            packetHandlers[CmdType.PlayerBasicInfoReq] = PlayerBasicInfo;
            packetHandlers[CmdType.PlayerPositionNotify] = PlayerPosition;
            packetHandlers[CmdType.PlayerChangeSceneNotify] = PlayerChangeSceneNotifyHandle;
            packetHandlers[CmdType.PlayerEnterMpRsp] = EnterMpRsp;
            packetHandlers[CmdType.None] = None;
            packetHandlers[CmdType.EntityAppearNotify] = EntityAppear;
            packetHandlers[CmdType.SceneEntityMoveUpdateNotify] = SceneEntityMovingUpdateNotifyHandle;

            Debug.Log("Protocal Initialized!");
        }

        private async Task SceneEntityMovingUpdateNotifyHandle(NetPacket packet)
        {

            try
            {
                SceneEntityMovingUpdateNotify rsp = packet.DecodeBody<SceneEntityMovingUpdateNotify>();

                var ent = GameSceneManager.Instance.entitiesManager.FindEntityById(5);

                //Debug.Log(rsp.EInfo.Id);

                if (ent == null) {
                    return;
                }

                //GameSceneManager.Instance.entitiesManager.SetEntityPosition(5,
                //    new Vector3(rsp.NewPos.X, rsp.NewPos.Y, rsp.NewPos.Z));

                await Task.CompletedTask;
            }
            catch (Exception ex) { 
                //Debug.LogError(ex.ToString());
            }
        }

        private async Task EntityAppear(NetPacket packet)
        {
            GameSceneManager.Instance.entitiesManager.
                AppearEntity();

            await Task.CompletedTask;
        }

        private async Task EnterMpRsp(NetPacket packet)
        {
            Debug.Log("Mp Connected!");

            await Task.CompletedTask;
        }

        private async Task None(NetPacket packet) { }

        private async Task PlayerPosition(NetPacket packet)
        {
            PlayerPositionNotify rsp = packet.DecodeBody<PlayerPositionNotify>();
            Client.instance.Player.Rigidbody.MovePosition(new Vector3(rsp.Pos.X, rsp.Pos.Y, rsp.Pos.Z));

            await Task.CompletedTask;
        }

        [Command("setplayerpos")]
        public static void SetPlayerPos(Vector3 pos)
        {
            //Client.NET.SendPacketAsync(CmdType.PlayerTeleportRequest, new PlayerT)
        }

        private async Task PlayerBasicInfo(NetPacket packet)
        {
            //NetDataManager.Instance.PlayerGameData = new PlayerBasicInfoData { PlayerName = "" }
        }

        private async Task EnterGameRsp(NetPacket packet)
        {
            LoginRsp rsp = packet.DecodeBody<LoginRsp>();

            UIManager.instance.UpdateUidUI(rsp.Id);

            await Task.CompletedTask;
        }

        private async Task GetAvatarDataRspHandle(NetPacket packet)
        {
            GetAvatarDataRsp rsp = packet.DecodeBody<GetAvatarDataRsp>();

            PlayerAvatarManager manager = GameSceneManager.Instance.PlayerAvatarManager;

            foreach (var rspData in rsp.AvatarList)
            {
                manager.ScsAvatars.Add(new AvatarData { AvatarID = rspData.Id });
            }

            await Task.CompletedTask;
        }

        private async Task PlayerSync(NetPacket packet)
        {
            ScPlayerSync data = packet.DecodeBody<ScPlayerSync>();


            await Task.CompletedTask;
        }

        private async Task SwitchAvatarRspHandle(NetPacket packet)
        {
            PlayerSwitchActiveReq rsp = packet.DecodeBody<PlayerSwitchActiveReq>();

            PlayerAvatarManager manager = GameSceneManager.Instance.PlayerAvatarManager;

            manager.SetAvatarActive(rsp.Index - 1);
            

            await Task.CompletedTask;
        }

        private async Task SceneUpdateNotify(NetPacket packet)
        {
            Debug.Log("Chest Update Notify");

            await Task.CompletedTask;
        }

        private async Task ChestInteractionRspHandle(NetPacket packet)
        {
            Debug.Log("You open chest");
            await Task.CompletedTask;
        }

        private async Task PlayerEnterSceneNotifyHandle(NetPacket packet)
        {
            PlayerEnterSceneNotify rsp = packet.DecodeBody<PlayerEnterSceneNotify>();

            GameSceneManager.Instance.EnterSceneAsync(rsp.SceneInfo);

            await Task.CompletedTask;
        }

        private async Task PlayerChangeSceneNotifyHandle(NetPacket packet)
        {
            PlayerChangeSceneNotify rsp = packet.DecodeBody<PlayerChangeSceneNotify>();

            GameSceneManager.Instance.ChangeSceneAsync(rsp.SceneInfo);

            await Task.CompletedTask;
        }

        [Command("changescenecs")]
        private static void ChangeScene(uint id)
        {
            //GameSceneManager.Instance.ChangeSceneAsync(id, new Vector3(0, 0, 0));
        }


        private async Task PlayerTokenScs(NetPacket packet)
        {
            await Task.CompletedTask;
        }
        private async Task OnLogin(NetPacket packet)
        {
            LoginRsp rsp = packet.DecodeBody<LoginRsp>();
            UIManager.instance.UpdateUidUI(rsp.Id);
            Client.instance.OnConnectedSuccess();
            await Task.CompletedTask;
        }

        private async Task BreathingRsp(NetPacket packet)
        {
            PlayerBreathingRsp rsp = packet!.DecodeBody<PlayerBreathingRsp>();

            UIManager.instance.UpdateSvTime(rsp.ServerTime);

            await Task.CompletedTask;
        }

        private async Task EnterSceneRsp(NetPacket packet)
        {
            EnterSceneRsp rsp = packet!.DecodeBody<EnterSceneRsp>();

            //GameSceneManager.Instance.EnterSceneFirstState(rsp.Id, true);

            await Task.CompletedTask;
        }

        private async Task ClientInitNotify(NetPacket packet)
        {
            ClientInitNotify rsp = packet!.DecodeBody<ClientInitNotify>();

            GameObject sceneManager = Resources.Load("NMX/Assets/ManagerGameObjects/SceneEntity/SceneManager") as GameObject;

            GameObject smgo = GameObject.Instantiate(sceneManager);

            smgo.GetComponent<GameSceneManager>().Initialize();

            await Task.CompletedTask;
        }

        private async Task GetTeamLineupData(NetPacket packet)
        {
            GetTeamLineupDataRsp rsp = packet!.DecodeBody<GetTeamLineupDataRsp>();

            PlayerAvatarManager manager = GameSceneManager.Instance.PlayerAvatarManager;

            manager.ScsTeamLineupStartIndexSetter(rsp.TeamInfo.TeamIndex);

            foreach (var rspData in rsp.TeamInfo.AvatarList)
            {
                manager.ScsTeamLineup.Add(new AvatarData { AvatarID = rspData.Id });
                manager.ScsAvatarEntityInfo.Add(new EntityInfo { Id = rspData.Id });
            }

            await Task.CompletedTask;
        }
    }
}
