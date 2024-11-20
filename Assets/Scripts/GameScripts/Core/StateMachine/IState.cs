using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NMX
{
    public interface IState
    {
        public void Enter();

        public void HandleInput();

        public void PhysicsUpdate();

        public void Update();

        public void LateUpdate();

        public void Exit();

        public void OnAnimationEnterEvent();

        public void OnAnimationExitEvent();

        public void OnAnimationTransitionEvent();

    }
}
