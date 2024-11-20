using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NMX
{
    public class PlayerInput : MonoBehaviour
    {
        public PlayerInputAction InputActions {  get; private set; }

        public PlayerInputAction.PlayerActions PlayerActions { get; private set; }

        private void Awake()
        {
            InputActions = new PlayerInputAction();

            PlayerActions = InputActions.Player;
        }

        private void OnEnable()
        {
            InputActions.Enable();
        }

        private void OnDisable()
        {
            InputActions.Disable();
        }

        public void DisableActionFor(InputAction action,float seconds)
        {
            StartCoroutine(DisableActionTime(action,seconds));
        }
        public void DisableActionForCallbacks(InputAction action, float seconds,Action callback)
        {
            StartCoroutine(DisableActionTime(action, seconds));

            callback();
        }

        private IEnumerator DisableActionTime(InputAction action,float seconds)
        {
            action.Disable();
            yield return new WaitForSeconds(seconds);

            action.Enable();
        }

        public void DisableAction(InputAction action)
        {
            action.Disable();
        }
        
        public void EnableAction(InputAction action)
        {
            action.Enable();
        }

    }
}
