using UnityEngine;

namespace Assets.Code.Gameplay
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        Camera cam;

        [SerializeField]
        Cinemachine.CinemachineFreeLook freeLook;

        [SerializeField]
        Ball ball;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
                ball.AddForce(cam.transform.forward);
        }
    }
}