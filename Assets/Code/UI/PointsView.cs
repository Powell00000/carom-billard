using Assets.Code.Gameplay;
using UnityEngine;

namespace Assets.Code.UI
{
    internal class PointsView : MonoBehaviour
    {
        [Zenject.Inject] private PointsManager pointsManager;

        [SerializeField] private TMPro.TMP_Text pointsLabel;

        private void Start()
        {
            pointsManager.PointsChanged += OnPointsChanged;
            OnPointsChanged(0);
        }

        private void OnDestroy()
        {
            pointsManager.PointsChanged -= OnPointsChanged
        }

        private void OnPointsChanged(int currentPoints)
        {
            pointsLabel.text = currentPoints.ToString();
        }
    }
}
