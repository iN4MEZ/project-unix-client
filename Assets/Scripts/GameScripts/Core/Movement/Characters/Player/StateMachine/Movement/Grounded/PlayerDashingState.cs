using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NMX
{
    public class PlayerDashingState : PlayerGroundedState
    {

        private PlayerDashedData dashData;

        private float startTime;

        private int consecutiveDashesUsed;

        private bool shouldKeepRotating;

        public PlayerDashingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            dashData = movementData.DashedData;
        }

        public override void Enter()
        {
            base.Enter();

            stateMachine.ReuseableData.MovementSpeedModifier = dashData.SpeedModifier;

            AddForceOnTransitionFromStationaryState();

            shouldKeepRotating = stateMachine.ReuseableData.MovementInput != Vector2.zero;

            UpdateCosecutiveDashed();

            startTime = Time.time;

            StartAnimation(stateMachine.Player.AnimationData.DashParameterHash);

        }

        public override void Exit()
        {
            base.Exit();

            StopAnimation(stateMachine.Player.AnimationData.DashParameterHash);
        }

        public override void Update()
        {
            base.Update();
        }

        public override void OnAnimationTransitionEvent()
        {

            if(stateMachine.ReuseableData.MovementInput == Vector2.zero )
            {
                stateMachine.ChangeState(stateMachine.HardStoppingState);
                return;
            }

            stateMachine.ChangeState(stateMachine.SprintingState);

        }


        private void UpdateCosecutiveDashed()
        {
            if(!IsConsecutive())
            {
                consecutiveDashesUsed = 0;
            }
            ++consecutiveDashesUsed;

            if(consecutiveDashesUsed == dashData.ConsecutiveDashesLimitAmout)
            {
                consecutiveDashesUsed = 0;

                stateMachine.Player.PlayerInput.DisableActionFor(stateMachine.Player.PlayerInput.PlayerActions.Dash,dashData.DashLimitReachedCooldown);
            }
        }

        private bool IsConsecutive()
        {
            return Time.time < startTime + dashData.TimeToBeConsideredConsecutive;
        }

        private void AddForceOnTransitionFromStationaryState()
        {
            if(stateMachine.ReuseableData.MovementInput != Vector2.zero)
            {
                return;
            }

            Vector3 characterRotationDirection = stateMachine.Player.transform.forward;

            characterRotationDirection.y = 0;

            stateMachine.Player.Rigidbody.velocity = characterRotationDirection * GetMovementSpeed();
        }

        protected override void OnDashStarted(InputAction.CallbackContext context)
        {
           
        }

        protected override void OnMovementCanceled(InputAction.CallbackContext context)
        {
            
        }

    }
}
