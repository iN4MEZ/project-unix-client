using NMX.GameCore.Network.Client;
using NMX.GameCore.Network.Protocol;
using NMX.Protocal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NMX
{
    public class PlayerAvatarManager : MonoBehaviour
    {
        //[field: SerializeField] public List<AvatarManager> PlayerAvatarEntityData { get; private set; }

        [field: SerializeField] public List<GameObject> TeamAvatar { get; private set; }

        [field: SerializeField] public List<GameObject> AvatarOnLoadData { get; private set; }

        [field: SerializeField] public List<AvatarData> Avatars { get; private set; }

        [field: SerializeField] public List<Animator> Animators { get; private set; }

        public delegate void ChangeAvatarEvent();

        public event ChangeAvatarEvent OnChangeAvatar;

        [field: SerializeField] public Player Player { get; private set; }

        private void Awake()
        {

            //Client.instance.udp.GetTeamDataPacket();

        }

        private void Start()
        {
            AddSwitchAvatarInputAction();
        }

        public async void LoadServerAvatarData()
        {

            await Client.NET.SendPacketAsync(CmdType.GetAvatarDataReq);

            await Client.NET.SendPacketAsync(CmdType.GetTeamDataReq);

            CreateAvatarGameObject();

            Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        }

        private void CreateAvatarGameObject()
        {
            TeamAvatar.Add(Resources.Load($"NMX/Assets/Entitys/Avatar/Prefabs/GameObjects/10001/TestAvatar") as GameObject);
            TeamAvatar.Add(Resources.Load($"NMX/Assets/Entitys/Avatar/Prefabs/GameObjects/10007/UnityChan") as GameObject);
            TeamAvatar.Add(Resources.Load($"NMX/Assets/Entitys/Avatar/Prefabs/GameObjects/10005/Elysia") as GameObject);
        }


        private void AddSwitchAvatarInputAction()
        {
            Player.PlayerInput.PlayerActions.SwitchAvatar.performed += OnSwitchAvatar;
            AddOnAvatarChangeEvent();
            //OnChangeAvatar += OnChangeForUpdateCamera;
        }

        private void RemoveSwitchAvatarInputAction()
        {
            Player.PlayerInput.PlayerActions.SwitchAvatar.performed -= OnSwitchAvatar;
        }

        private void AddOnAvatarChangeEvent()
        {
            OnChangeAvatar += OnChangeForStateAvatar;
            OnChangeAvatar += OnChangePlayerPosition;
        }

        private void RemoveOnAvatarChangeEvent()
        {
            OnChangeAvatar -= OnChangeForStateAvatar;
            OnChangeAvatar -= OnChangePlayerPosition;
        }

        private async void OnSwitchAvatar(InputAction.CallbackContext context)
        {
            string key = context.control.name;

            Player.movementStateMachine.ReuseableData.InAvatarChangeTransition = true;

            //Animators[int.Parse(key) - 2].SetBool(Animator.StringToHash("Stopping"),true);


            if (AvatarOnLoadData[int.Parse(key)-1] != null)
            {
                if (Player.movementStateMachine.ReuseableData.CurrentState.GetType() == typeof(PlayerAttackState))
                {
                    return;
                }

                await Client.NET.SendPacketAsync(CmdType.PlayerSwitchAvatarReq, new PlayerSwitchActiveReq { Index = int.Parse(key)});

            }
        }

        IEnumerator SetAvatarBackToElement(GameObject changedEntity, float time)
        {
            yield return new WaitForSeconds(time);
            changedEntity.transform.SetParent(GameSceneManager.Instance.AvatarElement.transform);
        }

        private void OnChangeForStateAvatar()
        {

            GameObject changedEntity = GetActiveAvatarManager().gameObject;

            changedEntity.transform.SetParent(GameSceneManager.Instance.EntityOutStateElement.transform);

            StartCoroutine(SetAvatarBackToElement(changedEntity, 2));

            switch (Player.movementStateMachine.ReuseableData.CurrentState.GetType()) 
            {
                case Type t when t == typeof(PlayerRunningState):
                    Player.movementStateMachine.ChangeState(Player.movementStateMachine.RunningState);
                    break;
                case Type t when t == typeof(PlayerSprintingState):
                    Player.movementStateMachine.ChangeState(Player.movementStateMachine.SprintingState);
                    break;
                case Type t when t == typeof(PlayerIdlingState):
                    Player.movementStateMachine.ChangeState(Player.movementStateMachine.IdlingState);
                    break;

            }
        }

        public void InitAvatar()
        {
            foreach (Transform activeAvatar in GameSceneManager.Instance.AvatarElement.transform)
            {
                if(AvatarOnLoadData.Count > 3) { return; }

                AvatarOnLoadData.Add(activeAvatar.gameObject);
            }

            UIManager.instance.UpdateActiveAvatarIcon(AvatarOnLoadData);

            foreach (GameObject activeOnLoad in AvatarOnLoadData)
            {
                activeOnLoad.SetActive(false);
            }

            DisableAllActiveAvatarExcept(0);
            SetAvatarActive(0);

        }
        public AvatarManager GetActiveAvatarManager()
        {
            foreach (GameObject activeOnLoad in AvatarOnLoadData)
            {
                if (activeOnLoad.activeSelf)
                {
                    return activeOnLoad.GetComponent<AvatarManager>();
                }
            }
            return null;
        }

        public GameObject GetActiveAvatarModel()
        {
            foreach (GameObject activeOnLoad in AvatarOnLoadData)
            {
                if (activeOnLoad.activeSelf)
                {
                    return activeOnLoad.transform.GetChild(0).gameObject;
                }
            }
            return null;
        }

        public int GetActiveAvatarIndex()
        {
            for(int i = 0; i < AvatarOnLoadData.Count; i++)
            {
                if (AvatarOnLoadData[i].activeSelf)
                {
                    return i;
                }
            }
            return 0;
        }

        private void UpdateActiveAvatarHpValueUI(AvatarManager activeData)
        {
            UIManager.instance.UpdateActiveAvatarHpValueUI(activeData);
        }

        public void SetAvatarActive(int index)
        {
            Player.movementStateMachine.Player.PlayerInput.DisableActionFor(Player.PlayerInput.PlayerActions.SwitchAvatar, 1);

            AvatarManager currentActive = GetActiveAvatarManager();
            GameObject targetEntity = AvatarOnLoadData[index];

            if(currentActive != null)
            {
                PlayerStoppingStateAnimation(currentActive.Animator);
                StartCoroutine(WaitForAnimationToEnd(currentActive.Animator));
                currentActive.gameObject.SetActive(false);

            }

            if (targetEntity != null)
            {
                targetEntity.SetActive(true);
            }

            //DisableAllActiveAvatarExcept(index);

            OnChangeAvatar?.Invoke();
            Player.movementStateMachine.ReuseableData.InAvatarChangeTransition = false;
            UpdateActiveAvatarHpValueUI(targetEntity.GetComponent<AvatarManager>());
        }

        public void DisableAllActiveAvatarExcept(int exceptIndex)
        {
            for (int i = 0; i < AvatarOnLoadData.Count; i++)
            {
                if(i != exceptIndex && AvatarOnLoadData[i] != null)
                {
                    AvatarOnLoadData[i].SetActive(false);
                }
            }
        }


        private void PlayerStoppingStateAnimation(Animator animator)
        {
            animator.SetBool(Player.movementStateMachine.Player.AnimationData.StoppingParameterHash, true);
        }
        void OnChangePlayerPosition()
        {
            System.Random rn = new System.Random();

            Vector3 leftOrRight = (rn.Next(0,1) == 1) ? Vector3.right : Vector3.left;

            //Player.transform.Translate(leftOrRight * 2, Space.Self);
        }

        private IEnumerator WaitForAnimationToEnd(Animator animator)
        {
            while (animator.IsInTransition(0))
            {
                yield return null;
            }
        }


        private void Update()
        {

            if(Input.GetKeyDown(KeyCode.G)) {
                PlayerStoppingStateAnimation(GetActiveAvatarManager().Animator);
            }
        }
    }
}
