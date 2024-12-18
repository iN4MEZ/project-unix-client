﻿using NMX.GameCore.Network.Client;
using NMX.GameCore.Network.Protocol;
using NMX.Protocal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NMX
{
    public class PlayerAvatarManager : MonoBehaviour
    {
        //[field: SerializeField] public List<AvatarManager> PlayerAvatarEntityData { get; private set; }

        [field: SerializeField] public List<GameObject> TeamAvatar { get; private set; }

        [field: SerializeField] public List<GameObject> AvatarOnLoadData { get; private set; }

        [field: SerializeField] public List<AvatarData> ScsAvatars { get; private set; }

        [field: SerializeField] public List<AvatarData> ScsTeamLineup { get; private set; }

        [field: SerializeField] public List<Animator> Animators { get; private set; }

        public delegate void ChangeAvatarEvent();

        public event ChangeAvatarEvent OnChangeAvatar;

        public int ScsTeamLineupStartIndex { get; private set; }

        [field: SerializeField] public Player Player { get; private set; }


        public void LoadServerAvatarData()
        {

            foreach (var avatar in ScsAvatars)
            {
            }

            foreach (var team in ScsTeamLineup)
            {
                var resources = LocalStorageManager.Instance.GetAvatarDataById(team.AvatarID);

                TeamAvatar.Add(Resources.Load($"{resources.ResourcePath}") as GameObject);
            }

            Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

            if (Player != null) {
                AddSwitchAvatarInputAction();
            }
        }

        private IEnumerator LoadAvatarResourceAsync(string path)
        {
            ResourceRequest request = Resources.LoadAsync<GameObject>(path);
            yield return request;

            if (request.asset != null)
            {
                GameObject loadedObject = request.asset as GameObject;
                var loadedObjectGo = Instantiate(loadedObject);
                TeamAvatar.Add(loadedObjectGo);
            }
            else
            {
                Debug.LogError("Failed to load the resource.");
            }
        }


        private void AddSwitchAvatarInputAction()
        {
            Player.PlayerInput.PlayerActions.SwitchAvatar.performed += OnSwitchAvatar;

            AddOnAvatarChangeEvent();
        }

        private void RemoveSwitchAvatarInputAction()
        {
            Player.PlayerInput.PlayerActions.SwitchAvatar.performed -= OnSwitchAvatar;
        }

        private void AddOnAvatarChangeEvent()
        {
            OnChangeAvatar += OnChangeForStateAvatar;
            //OnChangeAvatar += OnChangePlayerPosition;
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


            if (AvatarOnLoadData[int.Parse(key)-1] != null)
            {
                if (Player.movementStateMachine.ReuseableData.CurrentState.GetType() == typeof(PlayerAttackState))
                {
                    return;
                }

                SetAvatarActive(int.Parse(key)-1);

                if (int.Parse(key)-1 == GetActiveAvatarIndex()) { return; }

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

            //AvatarManager changedEntity = GetActiveAvatarManager();

            //changedEntity.gameObject.transform.SetParent(GameSceneManager.Instance.EntityOutStateElement.transform);

            //StartCoroutine(SetAvatarBackToElement(changedEntity.gameObject, 2));
        }

        public void InitAvatar()
        {
            foreach (Transform activeAvatar in GameSceneManager.Instance.AvatarElement.transform)
            {
                if(AvatarOnLoadData.Count > 3) { return; }

                AvatarOnLoadData.Add(activeAvatar.gameObject);
            }

            UIManager.instance.UpdateActiveAvatarIcon(AvatarOnLoadData);

            DisableAllActiveAvatarExcept(ScsTeamLineupStartIndex);
            

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
                    return activeOnLoad.transform.GetChild(0).gameObject ?? throw new NullReferenceException();
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

            SaveAnimatorDataStates(currentActive.Animator);

            SyncAnimatorState(currentActive.Animator, targetEntity.GetComponentInChildren<AvatarManager>().Animator);

            if (currentActive != null)
            {

                currentActive.gameObject.SetActive(false);

            }

            if (targetEntity != null)
            {
                targetEntity.SetActive(true);
            }

            OnChangeAvatar?.Invoke();

            RestoreAnimatorDataStates(targetEntity.GetComponentInChildren<AvatarManager>().Animator);   

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

        private IEnumerator PlayStopAnimationAndSetActive(AvatarManager target)
        {
            if(target !=  null)
            {
                target.Animator.Play(Player.AnimationData.HardStopParameterHash);
                yield return new WaitForSeconds(1f);
                target.gameObject.SetActive(false);
            }
        }

        private void SaveAnimatorDataStates(Animator animator)
        {
            Player.movementStateMachine.ReuseableData.AnimatorBoolStates.Clear();
            foreach (AnimatorControllerParameter param in animator.parameters)
            {
                if (param.type == AnimatorControllerParameterType.Bool)
                {
                    Player.movementStateMachine.ReuseableData.AnimatorBoolStates[param.nameHash] = animator.GetBool(param.nameHash);
                }
            }

            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            Player.movementStateMachine.ReuseableData.CurrentStateHash = stateInfo.shortNameHash;
            Player.movementStateMachine.ReuseableData.NormalizedTime = stateInfo.normalizedTime;
        }

        private void RestoreAnimatorDataStates(Animator animator)
        {
            foreach (var entry in Player.movementStateMachine.ReuseableData.AnimatorBoolStates)
            {
                animator.SetBool(entry.Key, entry.Value);
            }

            animator.Play(Player.movementStateMachine.ReuseableData.CurrentStateHash, 0, Player.movementStateMachine.ReuseableData.NormalizedTime % 1f);
        }
        void OnChangePlayerPosition()
        {
            System.Random rn = new System.Random();

            Vector3 leftOrRight = (rn.Next(0,1) == 1) ? Vector3.right : Vector3.left;

            //Player.transform.Translate(leftOrRight * 2, Space.Self);
        }


        private void SyncAnimatorState(Animator oldAnimator, Animator newAnimator)
        {
            if (oldAnimator == null || newAnimator == null) return;

            // ดึงค่าของ Bool Parameters ทั้งหมด
            foreach (AnimatorControllerParameter param in oldAnimator.parameters)
            {
                if (param.type == AnimatorControllerParameterType.Bool)
                {
                    bool value = oldAnimator.GetBool(param.nameHash);
                    newAnimator.SetBool(param.nameHash, value);
                }
            }
        }


        private IEnumerator WaitForAnimationToEnd(Animator animator)
        {
            while (animator.IsInTransition(0))
            {
                yield return null;
            }
        }

        public void ScsTeamLineupStartIndexSetter(int index)
        {
            ScsTeamLineupStartIndex = index;
        }


        private void Update()
        {

        }
    }
}
