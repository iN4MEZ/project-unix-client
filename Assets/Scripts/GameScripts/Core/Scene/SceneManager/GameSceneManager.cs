using NMX.GameCore.Network.Client;
using NMX.GameCore.Network.Protocol;
using NMX.Protocal;
using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NMX
{
    [Serializable]
    public class GameSceneManager : MonoBehaviour
    {
        [field: SerializeField] public SceneData SceneData { get; private set; }

        [field: SerializeField] public EntityManager entitiesManager { get; private set; }

        [field: SerializeField] public PlayerAvatarManager PlayerAvatarManager { get; private set; }

        [field: SerializeField] public Transform SceneEntityTransform { get; private set; }

        [field: SerializeField] public GameObject AvatarElement { get; private set; }
        [field: SerializeField] public GameObject EntityElement { get; private set; }

        [field: SerializeField] public GameObject EntityOutStateElement { get; private set; }

        [field: SerializeField] public AsyncOperation currentLoadingSceneAsyncOp { get; private set; }

        public static GameSceneManager Instance;

        private bool useCamBrain = false;


        public void Initialize()
        {
            Instance = this;

            SceneData = new SceneData();

            SceneData.Initialize();

            InitLoadingUIData();

            DebugSetting();


            if (Instance != this)
            {
                Destroy(Instance);
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);

            Debug.Log("Initialized Instance GameSceneManager!");
        }

        private void DebugSetting()
        {
            Application.targetFrameRate = 144;
        }

        private void InitLoadingUIData()
        {
            LoadingUI.Instance.InitLoadingUIData();
        }

        public void RegisterServerDataToSceneEntityData(SceneInfo sceneInfo)
        {
            foreach (EntityInfo entity in sceneInfo.EntityList) {
                var monsterData = LocalStorageManager.Instance.GetMonsterDataById(entity.Id);
                if (monsterData != null) {

                    GameObject loadedObject = Resources.Load(monsterData.ResourcePath) as GameObject;

                    SceneData.entities[entity] = loadedObject;
                }
            }
        }

        private async Task InitPlayer(Vector3 InitPos)
        {
            // Camera
            UnityEngine.Object cameraResource = Resources.Load("NMX/Assets/Entitys/Player/GameObjects/Cam/PlayerCamera");
            var camObj = Instantiate((GameObject)cameraResource);

            if(useCamBrain)
            {
                // Camera Brain
                UnityEngine.Object cameraBrainResource = Resources.Load("NMX/Assets/Entitys/Player/GameObjects/Cam/Camera");
                var camBrainObj = Instantiate((GameObject)cameraResource);
            }

            UnityEngine.Object playerResource = Resources.Load("NMX/Assets/Entitys/Player/GameObjects/Player/Player") as GameObject;

            await Client.NET.SendPacketAsync(CmdType.PlayerBasicInfoReq);

            Instantiate((GameObject)playerResource, InitPos, Quaternion.identity, entitiesManager.gameObject.transform);

            CameraZoom.instance.AddCamRef();

            Client.instance.OnPlayerCreated();

            AvatarElement = new GameObject("AvatarElement");
            EntityElement = new GameObject("EntityElement");

            EntityOutStateElement = new GameObject("EntityOutStateElement");

            AvatarElement.transform.SetParent(SceneEntityTransform);
            EntityElement.transform.SetParent(SceneEntityTransform);
            EntityOutStateElement.transform.SetParent(SceneEntityTransform);

            Debug.Log("Initialized Instance Player!");

        }

        private void CreateInGameUIGameObject()
        {
            UnityEngine.Object uiResource = Resources.Load("NMX/Assets/ManagerGameObjects/UIManager/GameObjects/UI_Element") as GameObject;
            
            var uiObj = Instantiate((GameObject)uiResource);

            Debug.Log("Instantiate In Game UI!");

        }

        public void LoadEntityAndObject()
        {
            InstatiateEntity();

            entitiesManager.LoadFactoryEntityIntroGame();

            foreach(var avatar in PlayerAvatarManager.AvatarOnLoadData)
            {
                avatar.GetComponent<AvatarManager>().InitializeAvatarAnimation();
                PlayerAvatarManager.Animators.Add(avatar.GetComponent<AvatarManager>().Animator);
            }

        }

        private void LoadOnlyEntityAndObject()
        {
            foreach (Transform ensTrans in EntityElement.transform)
            {
                Destroy(ensTrans.gameObject);
            }
            foreach (var entity in SceneData.entities.Values)
            {
                var index = Instantiate(entity, Vector3.zero, Quaternion.identity, EntityElement.transform);

                //entitiesManager.AddEntityGameObjectToFactory(index);
            }
            foreach (var chestObject in SceneData.chestObjects)
            {
                var index = Instantiate(chestObject, EntityElement.transform);

                //entitiesManager.AddEntityGameObjectToFactory(index);
            }

            //entitiesManager.LoadFactoryEntityIntroGame();

        }

        private void InstatiateEntity()
        {
            if (PlayerAvatarManager != null)
            {
                UIManager.instance.InitializeUI();
                UIManager.instance.SetActiveAllDynamic(true);

                for (int i = 0; i < PlayerAvatarManager.TeamAvatar.Count; i++)
                {

                    var index = Instantiate(PlayerAvatarManager.TeamAvatar[i], AvatarElement.transform);

                    Debug.Log("Created Entity : " + PlayerAvatarManager.TeamAvatar[i].gameObject.name);

                    var ateinfo = PlayerAvatarManager.ScsTeamLineup[i].ScsAvatarEntityInfo;

                    entitiesManager.AddEntityGameObjectToFactory(index, ateinfo);
                }
                PlayerAvatarManager.InitAvatar();
            }

            for (int i = 0; i < SceneData.entities.Values.Count; i++)
            {
                var entity = SceneData.entities.Values.ElementAt(i);

                var entityData = SceneData.entities.Keys.ElementAt(i);

                Vector3 bornPos = new Vector3(entityData.ApInfo.InitPos.X, entityData.ApInfo.InitPos.Y, entityData.ApInfo.InitPos.Z);

                var index = Instantiate(entity, bornPos, Quaternion.identity, EntityElement.transform);

                entitiesManager.AddEntityGameObjectToFactory(index, entityData);
            }

            foreach (var chestObject in SceneData.chestObjects)
            {
                var index = Instantiate(chestObject, EntityElement.transform);

                //entitiesManager.AddEntityGameObjectToFactory(index);
            }
        }

        public async void ChangeSceneAsync(SceneInfo sceneInfo)
        {

            await Client.NET.SendPacketAsync(CmdType.EnterScenePreStateReq);

            entitiesManager.entities.Clear();

            this.SceneData.Id = sceneInfo.SceneId;
            StartCoroutine(LoadSceneAsync(sceneInfo)); // Sv dev

            Vector3 initPos = new Vector3(sceneInfo.InitPos.X, sceneInfo.InitPos.Y, sceneInfo.InitPos.Z);

            Client.instance.Player.Rigidbody.MovePosition(initPos);

            await Client.NET.SendPacketAsync(GameCore.Network.Protocol.CmdType.EnterScenePostStateReq);
            LoadOnlyEntityAndObject();
        }

        public async void EnterSceneAsync(SceneInfo sceneInfo)
        {

            RegisterServerDataToSceneEntityData(sceneInfo);

            this.SceneData.Id = sceneInfo.SceneId;

            Vector3 initPos = new Vector3(sceneInfo.InitPos.X, sceneInfo.InitPos.Y, sceneInfo.InitPos.Z);

            entitiesManager = GetComponentInChildren<EntityManager>();

            await InitPlayer(initPos);

            PlayerAvatarManager = GetComponentInChildren<PlayerAvatarManager>();

            StartCoroutine(LoadSceneAsync(sceneInfo)); // Sv dev

        }

        private async void SendRequestAvatarData()
        {
            await Client.NET.SendPacketAsync(CmdType.GetTeamLineupDataReq);

            await Client.NET.SendPacketAsync(CmdType.GetAvatarDataReq);

            await Client.NET.SendPacketAsync(CmdType.EnterScenePreStateReq);

            await Client.NET.SendPacketAsync(CmdType.EnterScenePostStateReq);

        }

        private IEnumerator LoadSceneAsync(SceneInfo sceneInfo)
        {

            if (SceneManager.GetActiveScene().buildIndex != sceneInfo.SceneId) {

                LoadingUI.Instance.gameObject.SetActive(true);

                currentLoadingSceneAsyncOp = SceneManager.LoadSceneAsync((int)sceneInfo.SceneId);

                CreateInGameUIGameObject();

                while (!currentLoadingSceneAsyncOp.isDone)
                {
                    float processVal = Mathf.Clamp01(currentLoadingSceneAsyncOp.progress / 0.9f);

                    LoadingUI.Instance.LoadingProcessSlider.value = processVal;
                    yield return null;
                }

                SendRequestAvatarData();

                yield return StartCoroutine(Client.NET.ClientSession.serverCommandHandler.WaitTaskProceed(CmdType.GetTeamLineupDataRsp));

                yield return StartCoroutine(Client.NET.ClientSession.serverCommandHandler.WaitTaskProceed(CmdType.GetAvatarDataRsp));



                PlayerAvatarManager.LoadServerAvatarData();

                LoadEntityAndObject();

                Client.instance.Player.InitializePlayer();

            }


            if (currentLoadingSceneAsyncOp.isDone)
            {
                OnClientFinishLoaded();

                LoadingUI.Instance.gameObject.SetActive(false);
                UIManager.instance.MainUICanvas.enabled = true;
            }

        }


        private async void OnClientFinishLoaded()
        {
            await Client.NET.SendPacketAsync(GameCore.Network.Protocol.CmdType.EnterSceneFinishStateReq);
        }
    }
}
