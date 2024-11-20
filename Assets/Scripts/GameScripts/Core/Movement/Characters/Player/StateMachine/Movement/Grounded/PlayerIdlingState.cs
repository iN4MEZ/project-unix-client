using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NMX
{
    public class PlayerIdlingState : PlayerGroundedState
    {
        public PlayerIdlingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }

        public override void Enter()
        {
            stateMachine.ReuseableData.MovementSpeedModifier = 0f;

            ResetVelocity();

            base.Enter();

            StartAnimation(stateMachine.Player.AnimationData.IdleParameterHash);

        }

        public override void Exit()
        {
            base.Exit();

            StopAnimation(stateMachine.Player.AnimationData.IdleParameterHash);
        }

        public override void Update()
        {
            base.Update();

            if(stateMachine.ReuseableData.MovementInput == Vector2.zero)
            {
                return;
            }

            OnMove();
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            if (!IsMovingHorizontally())
            {
                return;
            }

            ResetVelocity();
        }
    }
}
