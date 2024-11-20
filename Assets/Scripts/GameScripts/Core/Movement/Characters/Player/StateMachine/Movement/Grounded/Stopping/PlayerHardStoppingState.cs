using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NMX
{
    public class PlayerHardStoppingState : PlayerStoppingState
    {
        public PlayerHardStoppingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();

            StartAnimation(stateMachine.Player.AnimationData.HardStopParameterHash);

            stateMachine.ReuseableData.MovementDecelerationForce = movementData.StopData.HardDecelerationForce;
        }

        public override void Exit()
        {
            base.Exit();

            StopAnimation(stateMachine.Player.AnimationData.HardStopParameterHash);
        }

        protected override void OnMove()
        {

            if (stateMachine.ReuseableData.ShouldWalk)
            {
                return;
            }

            stateMachine.ChangeState(stateMachine.RunningState);
        }
    }
}
