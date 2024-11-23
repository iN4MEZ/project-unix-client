﻿using NMX.GameCore.Network.Client;
using NMX.GameCore.Network.Protocol;
using NMX.Protocal;
using System;
using System.Collections;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

        private GameObject LoadingUI;

        private Slider processSlider;

        private bool useCamBrain = false;

        public void Initialize()
        {
            Instance = this;

            SceneData = new SceneData();

            SceneData.Initialize();

            LoadingUI = GameObject.FindGameObjectWithTag("LoadingUI");
            Canvas canvasLoadingUi = LoadingUI.GetComponentInChildren<Canvas>();

            foreach (Transform t in canvasLoadingUi.gameObject.transform)
            {
                if (t.gameObject.GetComponent<Slider>() != null)
                {
                    processSlider = t.gameObject.GetComponent<Slider>();
                }
            }

            Application.targetFrameRate = 144;

            if (Instance != this)
            {
                Destroy(Instance);
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(LoadingUI);

            Debug.Log("Initialized Instance GameSceneManager!");
        }

        public void RegisterServerDataToSceneEntityData(SceneInfo sceneInfo)
        {
            foreach (EntityInfo entity in sceneInfo.EntityList) {
                var monsterData = LocalStorageManager.Instance.GetMonsterDataById(entity.Id);
                if (monsterData != null) {

                    monsterData.BornInitPos = new Vector3(entity.ApInfo.InitPos.X, entity.ApInfo.InitPos.Y, entity.ApInfo.InitPos.Z);

                    GameObject loadedObject = Resources.Load(monsterData.ResourcePath) as GameObject;

                    SceneData.entities[monsterData] = loadedObject;
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

        private void CreateUiGameObject()
        {
            UnityEngine.Object uiResource = Resources.Load("NMX/Assets/ManagerGameObjects/UIManager/GameObjects/UI_Element") as GameObject;
            
            var uiObj = Instantiate((GameObject)uiResource);

            Debug.Log("Instantiate UI!");

        }

        public void LoadEntityAndObject(EntityInfo entityInfo)
        {
            InstatiateEntity();

            entitiesManager.LoadFactoryEntityIntroGame(entityInfo);

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

                entitiesManager.AddEntityGameObjectToFactory(index);
            }
            foreach (var chestObject in SceneData.chestObjects)
            {
                var index = Instantiate(chestObject, EntityElement.transform);

                entitiesManager.AddEntityGameObjectToFactory(index);
            }

            //entitiesManager.LoadFactoryEntityIntroGame();

        }

        private void InstatiateEntity()
        {
            if (PlayerAvatarManager != null)
            {
                UIManager.instance.InitializeUI();
                UIManager.instance.SetActiveAllDynamic(true);
                foreach (var entity in PlayerAvatarManager.TeamAvatar)
                {
                    var index = Instantiate(entity, AvatarElement.transform);

                    Debug.Log("Created Entity : "+ entity.gameObject.name);

                    entitiesManager.AddEntityGameObjectToFactory(index);
                }
                PlayerAvatarManager.InitAvatar();
            }

            for (int i = 0; i < SceneData.entities.Values.Count; i++)
            {
                var entity = SceneData.entities.Values.ElementAt(i);
                var entityData = SceneData.entities.Keys.ElementAt(i);
                var index = Instantiate(entity, entityData.BornInitPos, Quaternion.identity, EntityElement.transform);


                entitiesManager.AddEntityGameObjectToFactory(index);
            }

            foreach (var chestObject in SceneData.chestObjects)
            {
                var index = Instantiate(chestObject, EntityElement.transform);

                entitiesManager.AddEntityGameObjectToFactory(index);
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
            await Client.NET.SendPacketAsync(CmdType.GetAvatarDataReq);

            await Client.NET.SendPacketAsync(CmdType.EnterScenePreStateReq);

            StartCoroutine(Client.NET.SendPacketAsyncIEnumerator(CmdType.GetTeamDataReq));

            CreateUiGameObject();

        }

        private IEnumerator LoadSceneAsync(SceneInfo sceneInfo)
        {

            if (SceneManager.GetActiveScene().buildIndex != sceneInfo.SceneId) {

                LoadingUI.gameObject.SetActive(true);

                currentLoadingSceneAsyncOp = SceneManager.LoadSceneAsync((int)sceneInfo.SceneId);

                SendRequestAvatarData();

                while (!currentLoadingSceneAsyncOp.isDone)
                {
                    float processVal = Mathf.Clamp01(currentLoadingSceneAsyncOp.progress / 0.9f);

                    processSlider.value = processVal;
                    yield return null;
                }

                yield return StartCoroutine(Client.NET.clientSession.WaitForPacketProcessed(CmdType.GetAvatarDataRsp));

                StartCoroutine(Client.NET.SendPacketAsyncIEnumerator(CmdType.EnterScenePostStateReq));

                PlayerAvatarManager.LoadServerAvatarData();

                var einfo = new EntityInfo();

                LoadEntityAndObject(einfo);

            }


            if (currentLoadingSceneAsyncOp.isDone)
            {
                OnClientFinishLoaded();

                LoadingUI.gameObject.SetActive(false);
                UIManager.instance.MainUICanvas.enabled = true;
            }

        }


        private async void OnClientFinishLoaded()
        {
            await Client.NET.SendPacketAsync(GameCore.Network.Protocol.CmdType.EnterSceneFinishStateReq);
        }
    }
}
