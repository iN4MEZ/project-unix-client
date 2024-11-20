using QFSW.QC;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NMX
{
    public class UIManager : MonoBehaviour
    {
        public Text ServerTimeText;
        public Text ClientTimeText;
        public Text PlayerPhysicText;
        public Text PlayerUidText;

        public Text ActiveAvatarHpValueText;

        private Player Player;

        public GameObject ActiveAvatarPrefabs;

        public Canvas MainUICanvas {  get; private set; }
        public Canvas LoadingUICanvas {  get; private set; }


        public static UIManager instance;

        public GameObject DialogueCanvasBox;
        public Text DialogueTextBox;
        public Text DialogueNameText;

        public Text StateMachineInfo;


        // Assets/Resources/UI/Game/Avatar/Prefabs/AvatarActiveUI.prefab


        private void Awake()
        {
            instance = this;
        }


        public void InitializeUI()
        {
            Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

            SetActiveAllDynamic(false);
        }

        public void SetActiveAllDynamic(bool active)
        {
            foreach (Transform child in gameObject.transform)
            {
                Canvas[] cv = child.gameObject.GetComponentsInChildren<Canvas>();
                foreach (Canvas c in cv)
                {
                    if (c != null)
                    {
                        c.enabled = active;
                    }
                }
            }
        }

        private void Start()
        {
            GameObject uiLoadingGate = GameObject.FindGameObjectWithTag("LoadingUI");

            LoadingUICanvas = uiLoadingGate.GetComponentInChildren<Canvas>();

            MainUICanvas = GameObject.FindGameObjectWithTag("MainUIInGame").GetComponent<Canvas>();
            MainUICanvas.enabled = false;
        }

        [Command]
        public static void EnableUI(bool a)
        {
            Canvas canvas = GameObject.FindGameObjectWithTag("MainUIInGame").GetComponent<Canvas>();
            canvas.enabled = a;
        }

        public void UpdateActiveAvatarIcon(List<GameObject> data)
        {
            foreach (GameObject avatar in data)
            {
                CreateAvatarSwitchIcon(avatar.GetComponent<AvatarManager>());
            }
        }

        void CreateAvatarSwitchIcon(AvatarManager data)
        {
            GameObject avatarUI = Instantiate(ActiveAvatarPrefabs, GameObject.FindGameObjectWithTag("ActiveAvatarLayout").transform);

            Text at = avatarUI.GetComponentInChildren<Text>();

            RawImage am = avatarUI.GetComponentInChildren<RawImage>();

            am.texture = Resources.Load(data.SOData.ResourceData.IconPath) as Texture2D;
            at.text = data.SOData.Data.AvatarName;
        }

        public void UpdateActiveAvatarHpValueUI(AvatarManager activeData)
        {
            ActiveAvatarHpValueText.text = $"HP: {activeData.CurrentHp}/{activeData.SOData.Data.AvatarMaxHealth}";
        }

        public void UpdateUidUI(int myid)
        {
            PlayerUidText.text = "ID: " + myid.ToString();
        }

        public void UpdateSvTime(uint svtime)
        {
            ServerTimeText.text = "ST: " + DateTimeUtils.UnixSecondsToDateTime(svtime);
        }

        private void Update()
        {
            if(Player != null)
            {
                PlayerPhysicText.text = $"Position: {Player.Rigidbody.position}\r\nVelocity: {Player.Rigidbody.velocity}";
                ClientTimeText.text = "ClientTime: " + DateTimeUtils.UnixSecondsToDateTime((uint)DateTimeOffset.Now.ToUnixTimeSeconds());
            }
        }
    }
}
