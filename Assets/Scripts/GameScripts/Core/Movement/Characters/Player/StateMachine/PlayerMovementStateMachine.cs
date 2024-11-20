using UnityEngine;

namespace NMX
{
    public class PlayerMovementStateMachine : StateMachine
    {
        public PlayerIdlingState IdlingState { get; }

        public PlayerStatesReusableData ReuseableData { get; }

        public PlayerWalkingState WalkingState { get; }

        public PlayerRunningState RunningState { get; }

        public PlayerSprintingState SprintingState { get; }

        public PlayerDashingState DashingState { get; }

        public PlayerLightStoppingState LightStoppingState { get; }

        public PlayerMediumStoppingState MediumStoppingState { get; }

        public PlayerHardStoppingState HardStoppingState { get; }

        public PlayerAttackState AttackState { get; }


        public Player Player { get; }




        public PlayerMovementStateMachine(Player player)
        {
            Player = player;

            ReuseableData = new PlayerStatesReusableData();

            IdlingState = new PlayerIdlingState(this);

            WalkingState = new PlayerWalkingState(this);

            RunningState = new PlayerRunningState(this);

            DashingState = new PlayerDashingState(this);

            SprintingState = new PlayerSprintingState(this);

            LightStoppingState = new PlayerLightStoppingState(this);

            MediumStoppingState = new PlayerMediumStoppingState(this);

            HardStoppingState = new PlayerHardStoppingState(this);

            AttackState = new PlayerAttackState(this);  
        }
    }
}
