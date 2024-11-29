using NMX.GameCore.Network.Client;
using NMX.Protocal;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NMX
{
    public class PlayerMovementState : IState
    {

        private DateTime _lastSendTime = DateTime.MinValue;

        private readonly TimeSpan _sendInterval = TimeSpan.FromMilliseconds(50); // ส่งทุก 50ms

        protected PlayerMovementStateMachine stateMachine;

        protected PlayerGroundedData movementData;


        public virtual void Enter()
        {
            //Debug.Log("Enter " + GetType().Name);

            stateMachine.ReuseableData.CurrentState = stateMachine.currentState;

            stateMachine.ReuseableData.isCombat = false;

            AddInputActionsCallback();
        }

        public PlayerMovementState(PlayerMovementStateMachine playerMovementStateMachine)
        {
            stateMachine = playerMovementStateMachine;

            movementData = stateMachine.Player.Data.GroundedData;

            InitializeData();
        }

        private void InitializeData()
        {
            stateMachine.ReuseableData.TimeToReachTargetRotation = movementData.BaseRotationData.TargetRotationReachTime;
        }

        public virtual void Exit()
        {
            //Debug.Log("Exit " + GetType().Name);

            RemoveInputActionsCallback();
        }

        public virtual void HandleInput()
        {
            ReadMovementInput();
        }

        public virtual void OnAnimationEnterEvent()
        {
            
        }

        public virtual void OnAnimationExitEvent()
        {
            
        }

        public virtual void OnAnimationTransitionEvent()
        {
            
        }

        public virtual void PhysicsUpdate()
        {
            Move();
        }
        public virtual void Update()
        {
            if(stateMachine.ReuseableData.MovementSpeedModifier == 0)
            {
                return;
            }

            if ((DateTime.Now - _lastSendTime) < _sendInterval)
                return; // ข้ามการส่งถ้ายังไม่ถึงเวลา

            _lastSendTime = DateTime.Now;

            SendMovingInfo();
        }

        public virtual void LateUpdate()
        {
            if(stateMachine.Player.PlayerAvatarManager.GetActiveAvatarModel() == null)
            {
                return;
            }

            if (stateMachine.ReuseableData.InAvatarChangeTransition)
            {
                return;
            }

            if (stateMachine.ReuseableData.CurrentState.GetType() != typeof(PlayerAttackState))
            {
                stateMachine.Player.PlayerAvatarManager.GetActiveAvatarModel().transform.position = stateMachine.Player.gameObject.transform.position;
                stateMachine.Player.PlayerAvatarManager.GetActiveAvatarModel().transform.rotation = stateMachine.Player.gameObject.transform.rotation;

            }
            else
            {
                stateMachine.Player.gameObject.transform.position = stateMachine.Player.PlayerAvatarManager.GetActiveAvatarModel().transform.position;
                stateMachine.Player.gameObject.transform.rotation = stateMachine.Player.PlayerAvatarManager.GetActiveAvatarModel().transform.rotation;
            }
        }

        protected void StartAnimation(int animationHash)
        {

            var activeAvatarManager = stateMachine.Player.PlayerAvatarManager.GetActiveAvatarManager();
            if (activeAvatarManager == null) return;

            Animator animator = activeAvatarManager.Animator;

            if (!animator.GetBool(animationHash))
            {
                animator.SetBool(animationHash, true);
            }
        }
        
        
        protected void StopAnimation(int animationHash)
        {
            var activeAvatarManager = stateMachine.Player.PlayerAvatarManager.GetActiveAvatarManager();
            if (activeAvatarManager == null) return;

            Animator animator = activeAvatarManager.Animator;

            // ตรวจสอบว่าตัวละครกำลังเล่นแอนิเมชันนั้นอยู่ก่อนหยุด
            if (animator.GetBool(animationHash))
            {
                animator.SetBool(animationHash, false);
            }
        }

        protected void TriggerAnimation(int animationHash)
        {
            if (stateMachine.Player.PlayerAvatarManager.GetActiveAvatarManager() == null)
            {
                return;
            }

            stateMachine.Player.PlayerAvatarManager.GetActiveAvatarManager().Animator.SetTrigger(animationHash);
        }

        protected void ResetTriggerAnimation(int animationHash)
        {
            if (stateMachine.Player.PlayerAvatarManager.GetActiveAvatarManager() == null)
            {
                return;
            }

            stateMachine.Player.PlayerAvatarManager.GetActiveAvatarManager().Animator.ResetTrigger(animationHash);
        }

        protected virtual void AddInputActionsCallback()
        {
            stateMachine.Player.PlayerInput.PlayerActions.WalkToggle.started += OnWalkToggleStarted;
        }

        protected virtual void RemoveInputActionsCallback()
        {
            stateMachine.Player.PlayerInput.PlayerActions.WalkToggle.started -= OnWalkToggleStarted;
        }
        protected virtual void OnWalkToggleStarted(InputAction.CallbackContext context)
        {
            stateMachine.ReuseableData.ShouldWalk = !stateMachine.ReuseableData.ShouldWalk;
        }
        protected virtual void OnMovementCanceled(InputAction.CallbackContext context)
        {
            if(stateMachine.ReuseableData.CurrentState.GetType() == typeof(PlayerDashingState) ) { return; }

            stateMachine.ChangeState(stateMachine.IdlingState);
        }

        private void Move()
        {
            // if Movement input Null Or Player Not moving
            if (stateMachine.ReuseableData.MovementInput == Vector2.zero || stateMachine.ReuseableData.MovementSpeedModifier == 0f && !stateMachine.ReuseableData.isCombat) { return; }


            Vector3 movementDirection = GetMovementDirection();

            float targetRotationYAngle = Rotate(movementDirection);

            Vector3 targetRotationDirection = GetTargetRotationDirection(targetRotationYAngle);

            float moveSpeed = GetMovementSpeed();

            Vector3 currentPlayerHorizontalVelocity = GetPlayerHorizontalVelocity();

            stateMachine.Player.Rigidbody.AddForce(targetRotationDirection * moveSpeed - currentPlayerHorizontalVelocity, ForceMode.VelocityChange);
        }

        protected void RotateTowardsTargetRotation()
        {
            float currentYAngle = stateMachine.Player.Rigidbody.rotation.eulerAngles.y;

            if (currentYAngle == stateMachine.ReuseableData.CurrentTargetRotation.y)
            {
                return;
            }

            float smoothYAngle = Mathf.SmoothDampAngle(currentYAngle, stateMachine.ReuseableData.CurrentTargetRotation.y, ref stateMachine.ReuseableData.DampedTargetRotationCurrentVelocity.y, stateMachine.ReuseableData.TimeToReachTargetRotation.y - stateMachine.ReuseableData.DampedTargetRoationPassedTime.y);

            stateMachine.ReuseableData.DampedTargetRoationPassedTime.y += Time.fixedDeltaTime;

            Quaternion targetRotation = Quaternion.Euler(0f, smoothYAngle, 0f);

            stateMachine.Player.Rigidbody.MoveRotation(targetRotation);
        }

        protected void RotateImmediately()
        {
            Quaternion targetRotation = Quaternion.Euler(0, stateMachine.ReuseableData.CurrentTargetRotation.y,0);
            stateMachine.Player.Rigidbody.MoveRotation(targetRotation);
        }

        private float AddCameraRotationToAngle(float angle)
        {
            angle += stateMachine.Player.MainCameraTranform.eulerAngles.y;
            if (angle > 360f)
            {
                angle -= 360f;
            }

            return angle;
        }
        protected bool IsMovingHorizontally(float minimumMagnitude = 0.1f)
        {
            Vector3 playerHorizontalVelocity = GetPlayerHorizontalVelocity();

            Vector2 playerHorizontalMovement = new Vector2(playerHorizontalVelocity.x, playerHorizontalVelocity.y);

            return playerHorizontalMovement.magnitude > minimumMagnitude;
        }

        protected float Rotate(Vector3 direction)
        {
            float directionAngle = UpdateTargetRotation(direction);

            RotateTowardsTargetRotation();

            return directionAngle;
        }
        protected Vector3 GetTargetRotationDirection(float targetAngle)
        {
            return Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        }

        protected float UpdateTargetRotation(Vector3 direction, bool shouldCosiderCameraRotation = true)
        {
            float directionAngle = GetDirectionAngle(direction);

            if (shouldCosiderCameraRotation)
            {
                directionAngle = AddCameraRotationToAngle(directionAngle);
            }

            //Debug.Log(directionAngle);

            if (directionAngle != stateMachine.ReuseableData.CurrentTargetRotation.y)
            {
                UpdateTargetRotationData(directionAngle);
            }

            return directionAngle;
        }

        private void UpdateTargetRotationData(float targetAngle)
        {
            stateMachine.ReuseableData.CurrentTargetRotation.y = targetAngle;

            stateMachine.ReuseableData.DampedTargetRoationPassedTime.y = 0f;
        }

        protected void ResetVelocity()
        {
            stateMachine.Player.Rigidbody.velocity = Vector3.zero;
        }

        private float GetDirectionAngle(Vector3 direction)
        {
            float directionAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            if (directionAngle < 0f)
            {
                directionAngle += 360f;
            }

            return directionAngle;
        }

        protected float GetMovementSpeed()
        {

            return movementData.BaseSpeed * stateMachine.ReuseableData.MovementSpeedModifier;
        }

        protected Vector3 GetMovementDirection()
        {
            return new Vector3(stateMachine.ReuseableData.MovementInput.x,0f,stateMachine.ReuseableData.MovementInput.y);
        }

        protected Vector3 GetPlayerHorizontalVelocity()
        {
            Vector3 playerHorizontalVelocity = stateMachine.Player.Rigidbody.velocity;

            playerHorizontalVelocity.y = 0f;

            return playerHorizontalVelocity;
        }
        protected Vector3 GetPlayerVericalVelocity()
        {
            return new Vector3(0, stateMachine.Player.Rigidbody.velocity.y, 0);
        }

        protected void DecelerateHorizontally()
        {
            Vector3 playerHorizontalVelocity = GetPlayerHorizontalVelocity();

            stateMachine.Player.Rigidbody.AddForce(-playerHorizontalVelocity * stateMachine.ReuseableData.MovementDecelerationForce,ForceMode.Acceleration);
        }


        private void ReadMovementInput()
        {
            stateMachine.ReuseableData.MovementInput = stateMachine.Player.PlayerInput.PlayerActions.Movement.ReadValue<Vector2>();
        }

        protected AnimationClip GetCurrentAnimationClipInfo()
        {
            return stateMachine.Player.PlayerAvatarManager.GetActiveAvatarManager().Animator.GetCurrentAnimatorClipInfo(0)[0].clip;
        }


        protected MovingInfo CreateMovingInfo()
        {
            return new MovingInfo()
            { Pos = new Vector { X = stateMachine.Player.transform.position.x , Y = stateMachine.Player.transform.position.y , Z = stateMachine.Player.transform.position.z},
            Rot = new Rotation { Q = stateMachine.Player.transform.rotation.w, X = stateMachine.Player.transform.rotation.x, Y = stateMachine.Player.transform.rotation.y, Z = stateMachine.Player.transform.rotation.z}
            };
        }

        private async Task SendMovingInfo()
        {
            try
            {

                await Client.NET.SendPacketAsync(GameCore.Network.Protocol.CmdType.PlayerMovingReq,
                    new EntityMoving
                    { EntityId = stateMachine.Player.PlayerAvatarManager.GetActiveAvatarManager().EntityId, MoveInfo = CreateMovingInfo() });

            }
            catch (Exception e)
            {
                
            }
        }
    }
}
