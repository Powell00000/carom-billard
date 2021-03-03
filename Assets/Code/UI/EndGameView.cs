using Assets.Code.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.UI
{
    internal class EndGameView : MonoBehaviour
    {
        [Zenject.Inject] private GameplayController gameplayCtrl;
        [Zenject.Inject] private StatsManager statsManager;

        [SerializeField] private TMPro.TMP_Text pointsLabel;
        [SerializeField] private TMPro.TMP_Text hitsLabel;
        [SerializeField] private TMPro.TMP_Text timeLabel;

        [SerializeField] private Button exitButton;
        [SerializeField] private Button restartButton;

        private void Start()
        {
            restartButton.onClick.AddListener(Restart);
            exitButton.onClick.AddListener(ExitToMainMenu);
        }

        private void OnDestroy()
        {
            restartButton.onClick.RemoveListener(Restart);
            exitButton.onClick.RemoveListener(ExitToMainMenu);
        }

        private void OnEnable()
        {
            pointsLabel.text = statsManager.PreviousStats.Score.ToString();
            hitsLabel.text = statsManager.PreviousStats.ShotsMade.ToString();
            timeLabel.text = statsManager.PreviousStats.FormattedGameplayTime;
        }

        private void Restart()
        {
            gameplayCtrl.Restart();
        }

        private void ExitToMainMenu()
        {
            gameplayCtrl.GoToMainMenu();
        }
    }
}
