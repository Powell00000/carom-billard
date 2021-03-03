using Zenject;

namespace Assets.Code.Gameplay
{
    internal class PointsManager : IInitializable
    {
        [Inject] private GameplayController gameplayCtrl;
        [Inject] private StatsManager statsManager;

        private int currentPoints;

        public int CurrentPoints => currentPoints;
        public System.Action<int> PointsChanged;

        public void AddPoints(int amount)
        {
            SetPoints(currentPoints + amount);
        }

        private void SetPoints(int points)
        {
            currentPoints = points;
            PointsChanged?.Invoke(currentPoints);
        }

        public void Initialize()
        {
            gameplayCtrl.OnGameStart += Init;
            gameplayCtrl.OnGameEnd += OnGameEnd;
            Init();
        }

        private void Init()
        {
            SetPoints(0);
        }

        private void OnGameEnd()
        {
            statsManager.SetScore(currentPoints);
        }
    }
}
