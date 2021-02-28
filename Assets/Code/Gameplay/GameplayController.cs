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
            Waiting
        }

        private Ball[] balls;

        [SerializeField]
        private CameraController cameraCtrl;

        private SimpleStateMachine<GameState> gameplayState;

        public System.Action OnGameStart;
        public System.Action OnGameEnd;

        public GameState CurrentState => gameplayState.CurrentState;

        public void Initialize()
        {
            SceneManager.LoadScene("UI", LoadSceneMode.Additive);
            gameplayState = new SimpleStateMachine<GameState>(GameState.None);
            balls = FindObjectsOfType<Ball>();
            cameraCtrl.BallHit += StoreData;
            StartGame();
        }

        private void StartGame()
        {
            gameplayState.ChangeState(GameState.Playing);
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

            cameraCtrl.Revert();
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

        private void FixedUpdate()
        {
            if (gameplayState.CurrentState == GameState.None || gameplayState.CurrentState == GameState.Pause)
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
                gameplayState.ChangeState(GameState.Playing);
            }
        }
    }
}
