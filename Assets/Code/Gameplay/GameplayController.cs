using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Assets.Code.Gameplay
{
    internal class GameplayController : MonoBehaviour, IInitializable
    {
        public enum GameState
        {
            None,
            Pause,
            Playing,
            Replay,
            Waiting,
            GameEnded
        }

        [Inject] private HitBallController hitBallCtrl;

        private Ball[] balls;
        private SimpleStateMachine<GameState> gameplayState;

        private bool checkMovement = false;

        private bool ShouldUpdateBalls =>
            CurrentState == GameState.Playing
            || CurrentState == GameState.Replay
            || CurrentState == GameState.Waiting;

        public System.Action OnGameStart;
        public System.Action OnGameEnd;
        public System.Action OnAllStopped;
        public System.Action<GameState> OnStateChanged;
        public GameState CurrentState => gameplayState.CurrentState;

        public void Initialize()
        {
            Application.targetFrameRate = 60;
            SceneManager.LoadScene("UI", LoadSceneMode.Additive);
            gameplayState = new SimpleStateMachine<GameState>(GameState.None, StateChanged);
            balls = FindObjectsOfType<Ball>();
            hitBallCtrl.BallHit += BallHit;
            StartGame();
        }

        private void StateChanged(GameState currentState)
        {
            OnStateChanged?.Invoke(currentState);
            switch (currentState)
            {
                case GameState.None:
                    break;
                case GameState.Pause:
                    break;
                case GameState.Playing:
                    break;
                case GameState.Replay:
                    break;
                case GameState.Waiting:
                    break;
                case GameState.GameEnded:
                    break;
                default:
                    break;
            }
        }

        private void StartGame()
        {
            gameplayState.ChangeState(GameState.Playing);
        }

        public void EndGame()
        {
            OnGameEnd?.Invoke();
            gameplayState.ChangeState(GameState.GameEnded);
        }

        private void FixedUpdate()
        {
            if (!ShouldUpdateBalls)
            {
                return;
            }
            for (int i = 0; i < balls.Length; i++)
            {
                balls[i].CustomUpdate();
            }
            CheckIfAnyMoving();
        }

        [ContextMenu("Replay")]
        public void Replay()
        {
            if (CurrentState != GameState.Playing)
            {
                return;
            }

            gameplayState.ChangeState(GameState.Replay);

            for (int i = 0; i < balls.Length; i++)
            {
                balls[i].Revert();
            }

            hitBallCtrl.Revert();
        }

        private void BallHit()
        {
            switch (CurrentState)
            {
                case GameState.None:
                    break;
                case GameState.Pause:
                    break;
                case GameState.Playing:
                    StoreData();
                    break;
                case GameState.Replay:
                    break;
                case GameState.Waiting:
                    break;
                case GameState.GameEnded:
                    break;
                default:
                    break;
            }
            checkMovement = true;
        }

        private void StoreData()
        {
            if (CurrentState != GameState.Playing)
            {
                return;
            }

            for (int i = 0; i < balls.Length; i++)
            {
                balls[i].Store();
            }
        }

        private void CheckIfAnyMoving()
        {
            if (!checkMovement)
            {
                return;
            }

            bool anyMoving = false;
            for (int i = 0; i < balls.Length; i++)
            {
                anyMoving |= balls[i].IsMoving;
            }

            if (anyMoving)
            {
                switch (gameplayState.CurrentState)
                {
                    case GameState.None:
                        break;
                    case GameState.Pause:
                        break;
                    case GameState.Playing:
                        gameplayState.ChangeState(GameState.Waiting);
                        break;
                    case GameState.Replay:
                        break;
                    case GameState.Waiting:
                        break;
                    default:
                        break;
                }
            }
            else
            {
                OnAllStopped?.Invoke();
                gameplayState.ChangeState(GameState.Playing);
                checkMovement = false;
            }
        }
    }
}
