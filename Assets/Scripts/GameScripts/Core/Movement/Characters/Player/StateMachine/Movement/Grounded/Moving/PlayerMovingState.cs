using NMX.GameCore.Network.Client;
using NMX.Protocal;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.InputSystem;

namespace NMX
{
    public class PlayerMovingState : PlayerGroundedState
    {


        public PlayerMovingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }
        public override void Enter()
        {
            base.Enter();

            StartAnimation(stateMachine.Player.AnimationData.MovingParameterHash);
        }

        public override void Exit()
        {
            base.Exit();

            StopAnimation(stateMachine.Player.AnimationData.MovingParameterHash);
        }

        public override void Update()
        {
            base.Update();
            
        }


        protected override void OnMove()
        {

        }

        protected override void OnCombatClickedStarted(InputAction.CallbackContext context)
        {
            base.OnCombatClickedStarted(context);
        }

        protected override void OnCombatClickedCanceled(InputAction.CallbackContext context)
        {
            base.OnCombatClickedCanceled(context);
        }
    }
}
