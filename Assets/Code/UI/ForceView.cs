using Assets.Code.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.UI
{
    internal class ForceView : MonoBehaviour
    {
        [Zenject.Inject] private HitBallController hitBallCtrl;

        [SerializeField] private Image forceImage;

        private void Start()
        {
            hitBallCtrl.ForceChanged += OnForceChanged;
        }

        private void OnEnable()
        {
            OnForceChanged(0, 1);
        }

        private void OnDestroy()
        {
            hitBallCtrl.ForceChanged -= OnForceChanged;
        }

        private void OnForceChanged(float currentForce, float maxForce)
        {
            forceImage.fillAmount = currentForce / maxForce;
        }
    }
}
