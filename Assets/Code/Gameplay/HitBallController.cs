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
        [Inject] private PointsManager pointsManager;

        [SerializeField] private MainBall playingBall;

        private SaveableStruct<Vector3> savedForce;
        private int colorsCount;

        public Action BallHit;
        public Action<float, float> ForceChanged;

        private float currentForce = 0;
        private const float maxForce = 40f;

        public void Initialize()
        {
            colorsCount = Enum.GetValues(typeof(Ball.BallColor)).Length;
            inputCtrl.OnLeftButtonReleased += HitBall;
            inputCtrl.OnLeftButtonHold += IncrementForce;
            gameplayCtrl.OnAllStopped += CheckColorsHit;
        }

        private void IncrementForce()
        {
            currentForce += Time.deltaTime * maxForce;
            SetForce(currentForce);
        }

        private void SetForce(float current)
        {
            currentForce = Mathf.Clamp(current, 0, maxForce);
            ForceChanged?.Invoke(currentForce, maxForce);
        }

        private void CheckColorsHit()
        {
            if (gameplayCtrl.CurrentState != GameplayController.GameState.Waiting)
            {
                return;
            }

            if (playingBall.ColorsHitCount == colorsCount - 1)
            {
                //all other colors hit
                pointsManager.AddPoints(1);
            }
            playingBall.ClearHits();
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
            savedForce = new SaveableStruct<Vector3>(cameraCtrl.LookDirection * currentForce);
            BallHit?.Invoke();
            playingBall.AddVelocity(savedForce.SavedValue);
            SetForce(0);
        }

        public void Dispose()
        {
            inputCtrl.OnLeftButtonReleased -= HitBall;
        }
    }
}
