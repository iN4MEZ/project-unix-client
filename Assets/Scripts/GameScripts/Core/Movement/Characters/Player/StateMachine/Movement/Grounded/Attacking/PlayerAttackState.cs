using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NMX
{
    [Serializable]
    public class PlayerAttackState : PlayerGroundedState
    {

        private PlayerAvatarManager avatarManager;


        private bool shouldCancelAttack;

        private float delayClickTime = 0.1f;

        public PlayerAttackState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            avatarManager = stateMachine.Player.PlayerAvatarManager;
        }

        public override void Enter()
        {

            base.Enter();

            stateMachine.Player.PlayerInput.PlayerActions.Attack.started -= OnCombatClickedStarted;

            stateMachine.ReuseableData.MovementSpeedModifier = 0f;

            ResetVelocity();

            stateMachine.ReuseableData.ComboIndex = 0;

            StartAnimation(stateMachine.Player.AnimationData.AttackingParameterHash);

            stateMachine.Player.Rigidbody.isKinematic = true;

            stateMachine.Player.PlayerInput.DisableActionForCallbacks(stateMachine.Player.PlayerInput.PlayerActions.Attack, delayClickTime, () =>
            {

            });

        }

        protected override void OnMove()
        {
            //if (stateMachine.ReuseableData.MovementInput != Vector2.zero && stateMachine.ReuseableData.CurrentState.GetType() == typeof(PlayerIdlingState)
            //    && !stateMachine.ReuseableData.isCombat)
            //{
            //    return;
            //}

            //Debug.Log("Changetorun");
            //    stateMachine.ChangeState(stateMachine.RunningState);
        }

        public override void Update()
        {
            base.Update();

            OnMove();

        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }
        public override void Exit()
        {
            base.Exit();

            stateMachine.ReuseableData.ComboIndex = 0;

            ResetTriggerAnimation(Animator.StringToHash("DoCombo"));

            StopAnimation(stateMachine.Player.AnimationData.AttackingParameterHash);

            stateMachine.Player.Rigidbody.isKinematic = false;
        }

        public override void OnAnimationEnterEvent()
        {
            stateMachine.Player.PlayerInput.PlayerActions.Attack.started -= OnCombatClickedStarted;

            shouldCancelAttack = false;

            stateMachine.ReuseableData.ComboIndex++;

        }


        public override void OnAnimationTransitionEvent()
        {
            // Allow player to Attack
            stateMachine.Player.PlayerInput.PlayerActions.Attack.started += OnCombatClickedStarted; 

            shouldCancelAttack = true;


        }
        public override void OnAnimationExitEvent()
        {
            if (!shouldCancelAttack) { return; }
            ResetTriggerAnimation(Animator.StringToHash("DoCombo"));

            if(!isAnimationTransition())
            {
                stateMachine.ChangeState(stateMachine.IdlingState);
            }

        }

        protected override void OnCombatClickedStarted(InputAction.CallbackContext context)
        {
            if (stateMachine.ReuseableData.CurrentState.GetType() == typeof(PlayerDashingState)) { return; }

            stateMachine.Player.PlayerInput.DisableActionForCallbacks(stateMachine.Player.PlayerInput.PlayerActions.Attack, delayClickTime, () =>
            {

            });
            TriggerAnimation(Animator.StringToHash("DoCombo"));
        }

        protected override void OnCombatClickedCanceled(InputAction.CallbackContext context)
        {
        }

        protected override void OnMovementCanceled(InputAction.CallbackContext context)
        {
            
        }

        private bool isAnimationTransition()
        {
            return avatarManager.GetActiveAvatarManager().Animator.IsInTransition(0);
        }
    }
}
