using Assets.Code.Gameplay;
using UnityEngine;

namespace Assets.Code.UI
{
    internal class TimeView : MonoBehaviour
    {
        [Zenject.Inject] private GameplayController gameplayCtrl;
        [SerializeField] private TMPro.TMP_Text timeLabel;
        private void Update()
        {
            timeLabel.text = gameplayCtrl.GameplayTimeSpan.ToString(@"hh\:mm\:ss");
        }
    }
}
