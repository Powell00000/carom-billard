
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Assets.Code.Gameplay
{
    internal class InputController : MonoBehaviour
    {
        [Inject] private GameplayController gameplayCtrl;

        public System.Action OnLeftButtonReleased;
        public System.Action OnLeftButtonHold;

        public float GetAxisCustom(string axisName)
        {
            if (Input.GetMouseButton(1))
            {
                return Input.GetAxis(axisName);
            }
            else
            {
                return 0;
            }
        }

        private void Update()
        {
            if (gameplayCtrl.CurrentState != GameplayController.GameState.Playing)
            {
                return;
            }

            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            if (Input.GetMouseButton(0))
            {
                OnLeftButtonHold?.Invoke();
            }

            if (Input.GetMouseButtonUp(0))
            {
                OnLeftButtonReleased?.Invoke();
            }
        }
    }
}
