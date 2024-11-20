
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NMX
{
    public class PlayerSprintingState : PlayerMovingState
    {
        private PlayerSprintData sprintData;

        private bool keepSprinting;

        private float startTime;

        public PlayerSprintingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            sprintData = movementData.SprintData;
        }

        public override void Enter()
        {
            stateMachine.ReuseableData.MovementSpeedModifier = sprintData.SpeedModifier;

            base.Enter();

            StartAnimation(stateMachine.Player.AnimationData.SprintParameterHash);

            startTime = Time.time;
        }

        public override void Update()
        {
            base.Update();

            if (keepSprinting) { return; }

            if(Time.time < startTime + sprintData.SprintToRunTime) {
                return;
            }

            StopSprinting();
        }

        public override void Exit()
        {
            base.Exit();

            keepSprinting = false;

            StopAnimation(stateMachine.Player.AnimationData.SprintParameterHash);
        }

        protected override void OnMovementCanceled(InputAction.CallbackContext context)
        {

            stateMachine.ChangeState(stateMachine.HardStoppingState);

            //base.OnMovementCanceled(context);

        }

        private void StopSprinting()
        {
            if(stateMachine.ReuseableData.MovementInput == Vector2.zero)
            {

                stateMachine.ChangeState(stateMachine.IdlingState);

                return;

            }
            stateMachine.ChangeState(stateMachine.RunningState);
        }

        protected override void AddInputActionsCallback()
        {
            base.AddInputActionsCallback();

            stateMachine.Player.PlayerInput.PlayerActions.Sprint.performed += OnSprintPerformed;
        }

        protected override void RemoveInputActionsCallback()
        {
            base.RemoveInputActionsCallback();

            stateMachine.Player.PlayerInput.PlayerActions.Sprint.performed -= OnSprintPerformed;
        }

        private void OnSprintPerformed(InputAction.CallbackContext context)
        {
            keepSprinting = true;
        }

    }
}
