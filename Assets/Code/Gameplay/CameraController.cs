using Assets.Code.Saveable;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Code.Gameplay
{
    public class CameraController : MonoBehaviour, IRevertable
    {
        [Zenject.Inject]
        private GameplayController gameplayCtrl;

        [SerializeField]
        private Camera cam;

        [SerializeField]
        private CinemachineFreeLook freeLook;

        [SerializeField]
        private Ball ball;

        private SaveableStruct<Vector3> savedForce;
        private Vector3 currentForceDirection;

        public System.Action BallHit;

        private void Start()
        {
            CinemachineCore.GetInputAxis = GetAxisCustom;
        }

        private float GetAxisCustom(string axisName)
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

            currentForceDirection = cam.transform.forward.WithY(0);

            ball.SetForceDirection(currentForceDirection);
            if (EventSystem.current.IsPointerOverGameObject())
                return;
            if (Input.GetMouseButtonDown(0))
            {
                savedForce = new SaveableStruct<Vector3>(currentForceDirection * 8);
                BallHit?.Invoke();
                ball.AddVelocity(savedForce.SavedValue);
            }
        }

        public void Revert()
        {
            if (savedForce == null)
                return;
            ball.AddVelocity(savedForce.SavedValue);
        }

        public void Store()
        {
            //nothing
        }
    }
}