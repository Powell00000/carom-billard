using Cinemachine;
using UnityEngine;

namespace Assets.Code.Gameplay
{
    public class CameraController : MonoBehaviour
    {
        [Zenject.Inject] private GameplayController gameplayCtrl;
        [Zenject.Inject] private InputController inputCtrl;

        [SerializeField] private Camera cam;
        [SerializeField] private CinemachineFreeLook freeLook;

        private Vector3 lookDirection;

        public Vector3 LookDirection => lookDirection;

        private void Start()
        {
            CinemachineCore.GetInputAxis = inputCtrl.GetAxisCustom;
        }

        private void Update()
        {
            if (gameplayCtrl.CurrentState != GameplayController.GameState.Playing)
            {
                return;
            }

            lookDirection = cam.transform.forward.WithY(0);
        }
    }
}