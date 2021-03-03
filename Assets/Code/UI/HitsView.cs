using Assets.Code.Gameplay;
using System;
using UnityEngine;

namespace Assets.Code.UI
{
    internal class HitsView : MonoBehaviour
    {
        [Zenject.Inject] private HitBallController hitBallCtrl;
        [SerializeField] private TMPro.TMP_Text hitsLabel;

        private void OnEnable()
        {
            hitBallCtrl.BallHit += RefreshLabel;
            RefreshLabel();
        }

        private void RefreshLabel()
        {
            hitsLabel.text = hitBallCtrl.HitsMade.ToString();
        }

        private void OnDisable()
        {
            hitBallCtrl.BallHit -= RefreshLabel;
        }
    }
}
