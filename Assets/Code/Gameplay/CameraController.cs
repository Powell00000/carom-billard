using Cinemachine;
using System;
using UnityEngine;

namespace Assets.Code.Gameplay
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        private Camera cam;

        [SerializeField]
        private CinemachineFreeLook freeLook;

        [SerializeField]
        private Ball ball;

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
            if (Input.GetMouseButtonDown(0))
            {
                ball.AddForce(cam.transform.forward * 4);
            }
        }
    }
}