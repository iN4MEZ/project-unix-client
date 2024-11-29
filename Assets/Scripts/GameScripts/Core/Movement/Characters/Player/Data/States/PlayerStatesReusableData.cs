using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NMX
{
    public class PlayerStatesReusableData
    {

        public Vector2 MovementInput {  get; set; }

        public float MovementSpeedModifier { get; set; }

        public float MovementDecelerationForce { get; set; }

        public bool ShouldWalk { get; set; }

        public bool InAvatarChangeTransition { get; set; }
        public bool isCombat { get; set; }

        public int ComboIndex { get; set; }

        public IState CurrentState { get; set; }


        public Dictionary<int, bool> AnimatorBoolStates = new Dictionary<int, bool>();

        public int CurrentStateHash;
        public float NormalizedTime;


        private Vector3 currentTargetRotation;
        private Vector3 timeToReachTargetRotation;
        private Vector3 dampedTargetRotationCurrentVelocity;
        private Vector3 dampedTargetRoationPassedTime;

        public ref Vector3 CurrentTargetRotation
        {
            get
            {
                return ref currentTargetRotation;
            }
        }
        public ref Vector3 TimeToReachTargetRotation
        {
            get
            {
                return ref timeToReachTargetRotation;
            }
        }
        public ref Vector3 DampedTargetRotationCurrentVelocity
        {
            get
            {
                return ref dampedTargetRotationCurrentVelocity;
            }
        }
        public ref Vector3 DampedTargetRoationPassedTime
        {
            get
            {
                return ref dampedTargetRoationPassedTime;
            }
        }
    }
}
