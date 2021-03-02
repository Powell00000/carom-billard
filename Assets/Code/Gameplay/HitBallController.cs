﻿using Assets.Code.Saveable;
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
        [Inject] private StatsManager statsManager;

        [SerializeField] private MainBall playingBall;

        private SaveableStruct<Vector3> savedForce;
        private int colorsCount;
        private float currentForce = 0;
        private const float maxForce = 40f;
        private int hitsMade;

        public Action BallHit;
        public Action<float, float> ForceChanged;
        public int HitsMade => hitsMade;

        public void Initialize()
        {
            inputCtrl.OnLeftButtonReleased += OnLeftButtonReleased;
            inputCtrl.OnLeftButtonHold += IncrementForce;
            gameplayCtrl.OnAllStopped += CheckColorsHit;
            gameplayCtrl.OnGameEnd += OnGameEnd;
            gameplayCtrl.OnGameStart += Init;
            Init();
        }

        private void Init()
        {
            savedForce = null;
            colorsCount = Enum.GetValues(typeof(Ball.BallColor)).Length;
            hitsMade = 0;
        }

        private void OnGameEnd()
        {
            statsManager.SetShotsMade(hitsMade);
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

            HitBall();
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

        private void OnLeftButtonReleased()
        {
            savedForce = new SaveableStruct<Vector3>(cameraCtrl.LookDirection * currentForce);
            HitBall();
            hitsMade++;
        }

        private void HitBall()
        {
            playingBall.AddVelocity(savedForce.SavedValue);
            SetForce(0);
            BallHit?.Invoke();
        }

        public void Dispose()
        {
            inputCtrl.OnLeftButtonReleased -= OnLeftButtonReleased;
            inputCtrl.OnLeftButtonHold -= IncrementForce;
            gameplayCtrl.OnAllStopped -= CheckColorsHit;
            gameplayCtrl.OnGameEnd -= OnGameEnd;
        }
    }
}
