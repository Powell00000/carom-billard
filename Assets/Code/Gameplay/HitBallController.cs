using Assets.Code.Saveable;
using System;
using UnityEngine;
using Zenject;

namespace Assets.Code.Gameplay
{
    internal class HitBallController : MonoBehaviour, IInitializable, IRevertable, IDisposable
    {
        [Inject] private GameplayController gameplayCtrl;
        [Inject] private CameraController cameraCtrl;
        [Inject] private InputController inputCtrl;

        [SerializeField] private Ball playingBall;

        private SaveableStruct<Vector3> savedForce;

        public Action BallHit;

        public void Initialize()
        {
            inputCtrl.OnLeftButtonClick += HitBall;
        }

        public void Revert()
        {
            if (savedForce == null)
            {
                return;
            }

            playingBall.AddVelocity(savedForce.SavedValue);
        }

        public void Store()
        {
            //nothing
        }

        private void Update()
        {
            if (gameplayCtrl.CurrentState != GameplayController.GameState.Playing)
            {
                return;
            }

            playingBall.DrawExtrapolatedLine(cameraCtrl.LookDirection);
        }

        private void HitBall()
        {
            savedForce = new SaveableStruct<Vector3>(cameraCtrl.LookDirection * 8);
            BallHit?.Invoke();
            playingBall.AddVelocity(savedForce.SavedValue);
        }

        public void Dispose()
        {
            inputCtrl.OnLeftButtonClick -= HitBall;
        }
    }
}
