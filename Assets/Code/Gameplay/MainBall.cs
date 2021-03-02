using System.Collections.Generic;

namespace Assets.Code.Gameplay
{
    internal class MainBall : Ball
    {
        private HashSet<BallColor> colorsHit;

        public int ColorsHitCount => colorsHit.Count;
        public override void Initialize()
        {
            colorsHit = new HashSet<BallColor>();
            base.Initialize();
        }

        protected override void OtherBallHit(ref Ball otherBall)
        {
            colorsHit.Add(otherBall.color);
            base.OtherBallHit(ref otherBall);
        }

        public void ClearHits()
        {
            colorsHit.Clear();
        }
    }
}
