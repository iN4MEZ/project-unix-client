
using UnityEngine;
using UnityEngine.InputSystem;

namespace NMX
{
    public class PlayerRunningState : PlayerMovingState
    {
        private PlayerRunData runData;

        public PlayerRunningState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {

            runData = movementData.RunData;
        }

        public override void Enter()
        {
            base.Enter();

            stateMachine.ReuseableData.MovementSpeedModifier = runData.SpeedModifier;

            StartAnimation(stateMachine.Player.AnimationData.RunParameterHash);
        }

        public override void Exit()
        {
            base.Exit();

            StopAnimation(stateMachine.Player.AnimationData.RunParameterHash);
        }

        protected override void OnMovementCanceled(InputAction.CallbackContext context)
        {
            stateMachine.ChangeState(stateMachine.MediumStoppingState);
        }

        protected override void OnWalkToggleStarted(InputAction.CallbackContext context)
        {
            base.OnWalkToggleStarted(context);

            stateMachine.ChangeState(stateMachine.WalkingState);
        }

    }
}
