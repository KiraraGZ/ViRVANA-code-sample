using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Magia.Player.Utilities
{
    public class PlayerInput : MonoBehaviour
    {
        public ThirdPersonInputAction InputActions { get; private set; }
        public ThirdPersonInputAction.PlayerActions PlayerActions { get; private set; }
        public ThirdPersonInputAction.PauseActions PauseActions { get; private set; }
        public ThirdPersonInputAction.MenuActions MenuActions { get; private set; }

        public void Initialize()
        {
            InputActions = new();

            PlayerActions = InputActions.Player;
            MenuActions = InputActions.Menu;
            PauseActions = InputActions.Pause;

            SwitchToMenuInput();
        }

        public void Dispose()
        {
            InputActions = null;
        }

        public void SwitchToMenuInput()
        {
            MenuActions.Enable();
            PlayerActions.Disable();
            PauseActions.Disable();
        }

        public void SwitchToPlayerInput()
        {
            PlayerActions.Enable();
            PauseActions.Disable();
            MenuActions.Disable();
        }

        public void SwitchToPauseInput()
        {
            PauseActions.Enable();
            PlayerActions.Disable();
            MenuActions.Disable();
        }

        public void DisableActionForSeconds(InputAction action, float duration)
        {
            StartCoroutine(DisableAction(action, duration));
        }

        private IEnumerator DisableAction(InputAction action, float duration)
        {
            action.Disable();

            yield return new WaitForSeconds(duration);

            action.Enable();
        }
    }
}
