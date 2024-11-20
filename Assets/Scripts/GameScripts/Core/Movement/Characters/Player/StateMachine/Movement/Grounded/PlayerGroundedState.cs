using UnityEngine;
using UnityEngine.InputSystem;

namespace NMX
{
    public class PlayerGroundedState : PlayerMovementState
    {
        private SlopeData slopeData;


        public PlayerGroundedState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            slopeData = stateMachine.Player.ColliderUtilities.SlopeData;

        }

        public override void Enter()
        {
            base.Enter();

            StartAnimation(stateMachine.Player.AnimationData.GroundedParameterHash);

        }

        public override void Update()
        {
            base.Update();
        }

        public override void Exit()
        {
            base.Exit();

            StopAnimation(stateMachine.Player.AnimationData.GroundedParameterHash);
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            Float();


        }

        protected override void AddInputActionsCallback()
        {
            base.AddInputActionsCallback();

            stateMachine.Player.PlayerInput.PlayerActions.Movement.canceled += OnMovementCanceled;

            stateMachine.Player.PlayerInput.PlayerActions.Dash.started += OnDashStarted;

            stateMachine.Player.PlayerInput.PlayerActions.Attack.started += OnCombatClickedStarted;

            stateMachine.Player.PlayerInput.PlayerActions.Attack.canceled += OnCombatClickedCanceled;
        }

        protected override void RemoveInputActionsCallback()
        {
            base.AddInputActionsCallback();

            stateMachine.Player.PlayerInput.PlayerActions.Movement.canceled -= OnMovementCanceled;

            stateMachine.Player.PlayerInput.PlayerActions.Dash.started -= OnDashStarted;

            stateMachine.Player.PlayerInput.PlayerActions.Attack.started -= OnCombatClickedStarted;

            stateMachine.Player.PlayerInput.PlayerActions.Attack.canceled -= OnCombatClickedCanceled;
        }

        protected virtual void OnCombatClickedCanceled(InputAction.CallbackContext context)
        {

        }

        protected virtual void OnCombatClickedStarted(InputAction.CallbackContext context)
        {

            if (stateMachine.ReuseableData.CurrentState.GetType() == typeof(PlayerDashingState)) { return; }

            stateMachine.ChangeState(stateMachine.AttackState);
        }

        protected virtual void OnDashStarted(InputAction.CallbackContext context)
        {
            stateMachine.ChangeState(stateMachine.DashingState);
        }


        protected virtual void OnMove()
        {
            if (stateMachine.ReuseableData.ShouldWalk)
            {
                stateMachine.ChangeState(stateMachine.WalkingState);
                return;
            }
            stateMachine.ChangeState(stateMachine.RunningState);
        }
        private void Float()
        {
            if(stateMachine.Player.ColliderUtilities.CapsuleCollidersData == null ) {



                return;
            }

            Vector3 capsuleColliderCenterInWorldSpace = stateMachine.Player.ColliderUtilities.CapsuleCollidersData.Collider.bounds.center;

            Ray downwardRayFromCapsuleCenter = new Ray(capsuleColliderCenterInWorldSpace, Vector3.down);

            if (Physics.Raycast(downwardRayFromCapsuleCenter, out RaycastHit hit, slopeData.FloatRayDistance, stateMachine.Player.LayerData.GroundLayer, QueryTriggerInteraction.Ignore))
            {
                float groundAngle = Vector3.Angle(hit.normal, -downwardRayFromCapsuleCenter.direction);

                //float slopeSpeedModifier = SetSpeedModifierOnAngle(groundAngle);


                //if (slopeSpeedModifier == 0)
                //{
                //    return;
                //}

                float distanceToFloatingPoint = stateMachine.Player.ColliderUtilities.CapsuleCollidersData.ColliderCenterInLocalSpace.y * stateMachine.Player.transform.localScale.y - hit.distance;

                if (distanceToFloatingPoint == 0)
                {
                    return;
                }
                float amountToLift = distanceToFloatingPoint * slopeData.StepReachForce - GetPlayerVericalVelocity().y;

                Vector3 liftForce = new Vector3(0, amountToLift, 0);

                stateMachine.Player.Rigidbody.AddForce(liftForce, ForceMode.VelocityChange);
            }
        }

        private float SetSpeedModifierOnAngle(float angle)
        {
            float slopeSpeedAngle = movementData.SlopeSpeedAngle.Evaluate(angle);

            stateMachine.ReuseableData.MovementSpeedModifier = slopeSpeedAngle;

            return slopeSpeedAngle;
        }
    }
}
