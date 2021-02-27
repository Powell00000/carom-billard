using UnityEngine;

namespace Assets.Code.Gameplay
{
    public class Ball : MonoBehaviour
    {
        [SerializeField]
        Rigidbody rgbd;

        private float radius;

        private void Awake()
        {
            Initialize();
        }

        public virtual void Initialize()
        {
            CalculateRadius();
        }

        private void CalculateRadius()
        {
            radius = transform.localScale.y / 2;
        }

        public void AddForce(Vector3 force)
        {
            rgbd.AddForce(force, ForceMode.Impulse);
        }

        [ContextMenu("Push")]
        private void Push()
        {
            AddForce(Vector3.forward);
        }
    }
}