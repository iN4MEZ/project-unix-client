using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NMX
{
    public class PlayerStoppingState : PlayerGroundedState
    {
        public PlayerStoppingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();

            stateMachine.ReuseableData.MovementSpeedModifier = 0f;

            StartAnimation(stateMachine.Player.AnimationData.StoppingParameterHash);
        }

        public override void Exit()
        {
            base.Exit();

            StopAnimation(stateMachine.Player.AnimationData.StoppingParameterHash);
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            RotateTowardsTargetRotation();

            if(!IsMovingHorizontally())
            {
                return;
            }

            DecelerateHorizontally();
        }

        public override void OnAnimationTransitionEvent()
        {
            stateMachine.ChangeState(stateMachine.IdlingState);
        }

        protected override void OnMovementCanceled(InputAction.CallbackContext context)
        {

        }

        protected override void AddInputActionsCallback()
        {
            base.AddInputActionsCallback();

            stateMachine.Player.PlayerInput.PlayerActions.Movement.started += OnMovementStarted;
        }

        protected override void RemoveInputActionsCallback()
        {
            base.RemoveInputActionsCallback();

            stateMachine.Player.PlayerInput.PlayerActions.Movement.started -= OnMovementStarted;
        }

        private void OnMovementStarted(InputAction.CallbackContext context)
        {
            OnMove();
        }
    }
}
