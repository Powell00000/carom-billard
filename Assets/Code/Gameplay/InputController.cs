
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Assets.Code.Gameplay
{
    internal class InputController : ITickable
    {
        [Inject]
        private GameplayController gameplayCtrl;

        public System.Action OnLeftButtonClick;

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

        public void Tick()
        {
            if (gameplayCtrl.CurrentState != GameplayController.GameState.Playing)
            {
                return;
            }

            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                OnLeftButtonClick?.Invoke();
            }
        }
    }
}
