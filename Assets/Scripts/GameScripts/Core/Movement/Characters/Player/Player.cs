using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace NMX
{
    [RequireComponent(typeof(PlayerInput))]
    public class Player : MonoBehaviour
    {
        [field: Header("References")]
        [field: SerializeField] public PlayerSO Data { get; private set; }
        public PlayerInput PlayerInput { get; private set; }
        public Rigidbody Rigidbody { get; private set; }


        [field: Header("Collitions")]
        [field: SerializeField] public CapsuleCollidersUtillities ColliderUtilities { get; private set; }

        [field: SerializeField] public PlayerLayerData LayerData { get; private set; }


        public Transform MainCameraTranform;

        [field: Header("Animations")]

        [field: SerializeField] public PlayerAnimationData AnimationData { get; private set; }

        public PlayerMovementStateMachine movementStateMachine { get; private set; }

        public PlayerAvatarManager PlayerAvatarManager { get; private set; }

        private void Awake()
        {

            PlayerAvatarManager = GetComponent<PlayerAvatarManager>();

            Rigidbody = GetComponent<Rigidbody>();

            PlayerInput = GetComponent<PlayerInput>();

            movementStateMachine = new PlayerMovementStateMachine(this);

        }
        void Start()
        {
            MainCameraTranform = Camera.main.transform;
            ColliderUtilities.Initialize(gameObject);
            AnimationData.Initialize();

            movementStateMachine.ChangeState(movementStateMachine.IdlingState);
        }

        private void OnValidate()
        {
            ColliderUtilities.Initialize(gameObject);
            ColliderUtilities.CalculateCapsuleColliderDimension();
        }

        void Update ()
        {
            movementStateMachine.HandleInput();

            movementStateMachine.Update();

            //UpdateColliderPositionToActiveAvatar();

        }

        private void FixedUpdate()
        {
            movementStateMachine.PhysicsUpdate();
        }

        private void LateUpdate()
        {
            movementStateMachine.LateUpdate();
        }

        public void OnMovementStateAnimationEnterEvent()
        {
            movementStateMachine.OnAnimationEnterEvent();
        }
        
        public void OnMovementStateAnimationExitEvent()
        {
            movementStateMachine.OnAnimationExitEvent();
        }

        public void OnMovementStateAnimationTransitionEvent()
        {
            movementStateMachine.OnAnimationTransitionEvent();
        }

    }
}
