using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NMX
{
    public class PlayerLightStoppingState : PlayerStoppingState
    {
        public PlayerLightStoppingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();

            stateMachine.ReuseableData.MovementDecelerationForce = movementData.StopData.LightDecelerationForce;
        }
    }
}
